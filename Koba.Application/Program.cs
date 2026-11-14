using Koba.Application.Bot;
using Koba.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
    {
        services.AddDiscordGateway(hostContext.Configuration)
            .AddDiscordServices(hostContext.Configuration);
        services.AddHostedService<Worker>();
    }
);

var app = builder.Build();
app.Run();