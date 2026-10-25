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
        using var scope = _logger.BeginScope("[TOPIC WORKER]");
        _logger.LogInformation("Iniciando o Worker");
        try
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
                await InvokeCallback(item);
            
        }
        catch (OperationCanceledException e)
        {
            _logger.LogInformation("Execução do Worker interrompida, despachando ultimas mensagens...");
            while (_channel?.Reader.TryRead(out T data) ?? false)
                await InvokeCallback(data);
            _logger.LogInformation("Pronto. Channel esvaziado.");
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

    private async Task InvokeCallback(T data)
    {
        try
        {
            await _consumerCallback?.Invoke(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Não foi possível despachar a mensagem {message}. Motivo: {reason}", data?.ToString(), ex.Message);
        }
    }

    public void Dispose()
    {
        _channel?.Writer.Complete(new ChannelClosedException("Canal não está mais aberto para recebimento de mensagens."));
        _cancellationTokenSource.Cancel();
    }
}