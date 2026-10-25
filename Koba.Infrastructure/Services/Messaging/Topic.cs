using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services.Messaging;

internal class Topic<T> : IAsyncDisposable
{
    private readonly Channel<T> _channel;
    private Func<T, Task>? _consumerCallback;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Lock consumerLock = new();
    private readonly ILogger<Topic<T>> _logger;
    private Task workerTask;

    internal Topic(ILogger<Topic<T>> logger, Func<T, Task>? consumer = null, BoundedChannelOptions? topicConfiguration = null)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _consumerCallback = consumer;
        _logger = logger;
        _channel = Channel.CreateBounded<T>(topicConfiguration ?? new BoundedChannelOptions(1000));
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
            workerTask = Task.Run(DoWork, _cancellationTokenSource.Token);
        }
    }

    internal async Task AddMessage(T data, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(data, cancellationToken);
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

    public async ValueTask DisposeAsync()
    {
        using var scope = _logger.BeginScope("[TOPIC SHUTDOWN]");
        
        _logger.LogInformation("Tentando desligamento suave.");

        try
        {
            _channel.Writer.Complete();
            if (_consumerCallback is not null)
                await workerTask.WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (TimeoutException)
        {
            _logger.LogWarning("Timeout no encerramento do Worker, forçando desligamento");
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Dispose já em execução.");
        }
        finally
        {
            if (_cancellationTokenSource is { IsCancellationRequested: false })
            {
                await _cancellationTokenSource.CancelAsync();
                _cancellationTokenSource.Dispose();
            }
        }
    }
}