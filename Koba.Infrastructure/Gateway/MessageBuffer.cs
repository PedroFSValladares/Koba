using System.Threading.Channels;
using Koba.Infrastructure.Services.Messaging;
using Microsoft.Extensions.Logging;

namespace Koba.Infrastructure.Gateway;

internal class MessageBuffer : IAsyncDisposable
{
    private readonly ILogger<MessageBuffer> _logger;
    private readonly Topic<string> _messageTopic;
    private readonly CancellationTokenSource _cancellationTokenSource;

    internal MessageBuffer(ILogger<MessageBuffer> logger, Func<string, Task> callback, ILoggerFactory loggerFactory)
    {
        ILogger<Topic<string>> topicLogger = loggerFactory.CreateLogger<Topic<string>>();
        _logger = logger;
        _messageTopic = new Topic<string>(topicLogger, new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        _cancellationTokenSource = new CancellationTokenSource();
    }

    internal async Task PublishAsync(string message, CancellationToken token)
    {
        using var scope = _logger.BeginScope("[MESSAGE PUBLISHING]");
        _logger.LogInformation("Nova mensagem cacheada: {message}", message);
        await _messageTopic.AddMessageAsync(message, token);
    }

    public async ValueTask DisposeAsync()
    {
        using var scope = _logger.BeginScope("[DISPOSING]");
        _logger.LogInformation("Encerrando buffer.");
        await _messageTopic.DisposeAsync();
    }
}