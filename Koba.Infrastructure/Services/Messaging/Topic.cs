using System.Threading.Channels;

namespace Koba.Infrastructure.Services.Messaging;

internal class Topic<T> : IDisposable
{
    private Channel<T>? _channel;
    private Func<T, Task>? _consumerCallback;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Lock consumerLock = new(), channelLock = new();
    private Task workerTask;

    internal Topic()
    {
        _cancellationTokenSource = new CancellationTokenSource();
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