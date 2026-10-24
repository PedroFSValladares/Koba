using System.Collections.Concurrent;
using Koba.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services.Messaging;

public class ChannelMessagingService<T> : IMessagingService<T>
{
    private readonly ConcurrentDictionary<string ,Topic<T>> Topics = [];
    private readonly ILogger<ChannelMessagingService<T>> _logger;

    public ChannelMessagingService(ILogger<ChannelMessagingService<T>> logger)
    {
        _logger = logger;
    }
    
    public void Subscribe(string topicName, Func<T, Task> callback)
    {
        using var scope = _logger.BeginScope("SUBSCRIBE");
        _logger.LogInformation("Subscrição recebida para o tópico {topicName}.", topicName);
        
        Topic<T> topic = Topics.GetOrAdd(topicName, new Topic<T>());
        topic.AddConsumer(callback);
    }

    public void Publish(string topicName, T data)
    {
        using var scope = _logger.BeginScope("PUBLISH");
        _logger.LogInformation("Publicando mensagem {data}", data?.ToString());
        
        Topic<T> topic = Topics.GetOrAdd(topicName, new Topic<T>());
        topic.CreateAndAddToChannel(data);
    }

}