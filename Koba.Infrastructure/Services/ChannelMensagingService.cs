using System.Threading.Channels;
using Koba.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services;

public class ChannelMensagingService<T> : IMensagingService<T>
{
    
    private readonly Dictionary<string ,Func<T, Task>> Handlers = [];
    private readonly Channel<T> PublishedMessagesChannel;
    private readonly ILogger<ChannelMensagingService<T>> _logger;

    public ChannelMensagingService(ILogger<ChannelMensagingService<T>> logger)
    {
        _logger = logger;
    }
    
    public void Subscribe(Func<T, Task> callback, string topicName)
    {
        using var scope = _logger.BeginScope("SUBSCRIBE ASYNC");
        _logger.LogInformation("Subscrição assincrona recebida.");
        Handlers[topicName] += callback;
    }

    public void Publish(T data, string topicName)
    {
        using var scope = _logger.BeginScope("PUBLISH SYNC");
        _logger.LogInformation("Publicando mensagem {data}", data?.ToString());

        if (!Handlers.TryGetValue(topicName, out var handler))
            throw new InvalidOperationException("Tópico não cadastrado.");
        
        handler.Invoke(data);
    }
}