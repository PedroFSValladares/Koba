using System.Collections.Concurrent;
using System.Threading.Channels;
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
        _logger.LogInformation("Subscrição recebida para o tópico {topicName}.", topicName);
        Topic<T> topic = GetOrCreateTopic(topicName);
        
        topic.AddConsumer(callback);
    }

    public void Publish(string topicName, T data)
    {
        using var scope = _logger.BeginScope("PUBLISH");
        _logger.LogInformation("Publicando mensagem {data}", data?.ToString());
        Topic<T> topic = GetOrCreateTopic(topicName);
        
        topic.CreateAndAddToChannel(data);
    }

    private Topic<T> GetOrCreateTopic(string topicName)
    {
        return Topics.GetOrAdd(topicName, x =>
        {
            ILogger<Topic<T>> topicLogger = _loggerFactory.CreateLogger<Topic<T>>();
            return new Topic<T>(topicLogger, topicConfiguration: new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropWrite
            });
        });
    }

}