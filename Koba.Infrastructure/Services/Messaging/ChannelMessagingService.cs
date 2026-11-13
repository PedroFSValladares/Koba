using System.Collections.Concurrent;
using System.Threading.Channels;
using Koba.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Services.Messaging;

public class ChannelMessagingService<T> : IMessagingService<T>
{
    private readonly ConcurrentDictionary<string, Topic<T>> Topics = [];
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
        var topic = GetOrCreateTopic(topicName);

        topic.AddConsumer(callback);
    }

    public async Task PublishAsync(string topicName, T data, CancellationToken token)
    {
        using var scope = _logger.BeginScope("[PUBLISH]");
        _logger.LogInformation("Publicando mensagem {data}", data?.ToString());
        var topic = GetOrCreateTopic(topicName);

        await topic.AddMessageAsync(data, token);
    }

    private Topic<T> GetOrCreateTopic(string topicName)
    {
        return Topics.GetOrAdd(topicName, x =>
        {
            var topicLogger = _loggerFactory.CreateLogger<Topic<T>>();
            return new Topic<T>(topicLogger, new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropWrite
            });
        });
    }
}