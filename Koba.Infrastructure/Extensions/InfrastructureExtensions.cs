using Koba.Contracts.Interfaces;
using Koba.Infrastructure.Gateway;
using Koba.Infrastructure.Socket;
using Koba.Infrastructure.Socket.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Koba.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddDiscordGateway(this IServiceCollection services, IConfiguration configuration)
    {
        SessionState cache = new SessionState();
        services.AddScoped<IRawSocket, SocketClient>()
            .AddSingleton<IDiscordGateway, DiscordGateway>(x => new DiscordGateway(
                x.GetRequiredService<IRawSocket>(), x.GetRequiredService<ILogger<DiscordGateway>>(), cache, x.GetRequiredService<IOptions<GatewayConfiguration>>()));
        
        return services;
    }
}