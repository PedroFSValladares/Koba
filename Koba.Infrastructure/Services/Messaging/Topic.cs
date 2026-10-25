using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services.Messaging;

internal class Topic<T> : IDisposable
{
    private Channel<T>? _channel;
    private Func<T, Task>? _consumerCallback;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Lock consumerLock = new(), channelLock = new();
    private readonly ILogger<Topic<T>> _logger;
    private Task workerTask;

    internal Topic(ILogger<Topic<T>> logger)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        workerTask = Task.Run(DoWork, _cancellationTokenSource.Token);
        _logger = logger;
    }

    internal Topic(ILogger<Topic<T>> logger, Func<T, Task> consumer)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _consumerCallback = consumer;
        _logger = logger;
        workerTask = Task.Run(DoWork, _cancellationTokenSource.Token);
    }
    
    private async Task DoWork()
    {
        if(_consumerCallback is null)
            return;
        
        await foreach (var item in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
        {
            await _consumerCallback.Invoke(item);
        }
    }

    internal void AddConsumer(Func<T, Task> consumer)
    {
        lock (consumerLock)
        {
            if(_consumerCallback is not null)
                throw new InvalidOperationException("Já existe um consumidor para esse tópico");

            _consumerCallback = consumer;
        }
    }

    internal void CreateAndAddToChannel(T data)
    {
        lock (channelLock)
        {
            _channel ??= Channel.CreateUnbounded<T>();
        }
        _channel.Writer.TryWrite(data);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        workerTask.Dispose();
    }
}