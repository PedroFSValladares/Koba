using System.Threading.Channels;
using Koba.Infrastructure.Services.Messaging;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Gateway;

internal class MessageBuffer : IDisposable
{
    private readonly ILogger<MessageBuffer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Topic<string> _messageTopic;

    internal MessageBuffer(ILogger<MessageBuffer> logger, Func<string, Task> callback, ILoggerFactory loggerFactory)
    {
        ILogger<Topic<string>> topicLogger = loggerFactory.CreateLogger<Topic<string>>();
        _logger = logger;
        _messageTopic = new Topic<string>(topicLogger, callback, new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        _loggerFactory = loggerFactory;
    }

    internal void Publish(string message)
    {
        using var scope = _logger.BeginScope("[MESSAGE PUBLISHING]");
        _logger.LogInformation("Nova mensagem cacheada: {message}", message);
        _messageTopic.CreateAndAddToChannel(message);
    }

    public void Dispose()
    {
        _messageTopic.Dispose();
    }
}