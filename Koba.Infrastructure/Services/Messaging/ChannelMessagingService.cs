using System.Collections.Concurrent;
using Koba.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services.Messaging;

public class ChannelMessagingService<T> : IMessagingService<T>
{
    private readonly ConcurrentDictionary<string ,Topic<T>> Topics = [];
    private readonly ILogger<ChannelMessagingService<T>> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public ChannelMessagingService(ILogger<ChannelMessagingService<T>> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
    }
    
    public void Subscribe(string topicName, Func<T, Task> callback)
    {
        using var scope = _logger.BeginScope("SUBSCRIBE");
        ILogger<Topic<T>> topicLogger = _loggerFactory.CreateLogger<Topic<T>>();
        _logger.LogInformation("Subscrição recebida para o tópico {topicName}.", topicName);
        
        Topic<T> topic = Topics.GetOrAdd(topicName, new Topic<T>(topicLogger));
        topic.AddConsumer(callback);
    }

    public void Publish(string topicName, T data)
    {
        using var scope = _logger.BeginScope("PUBLISH");
        ILogger<Topic<T>> topicLogger = _loggerFactory.CreateLogger<Topic<T>>();
        _logger.LogInformation("Publicando mensagem {data}", data?.ToString());
        
        Topic<T> topic = Topics.GetOrAdd(topicName, new Topic<T>(topicLogger));
        topic.CreateAndAddToChannel(data);
    }

}