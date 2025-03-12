using Koba.Models;
//using Koba.Services.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Koba.Services.Persistence;
using Discord;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, Services) => {
        Services.AddSingleton<DiscordClient>();
    });

IConfiguration config = new ConfigurationBuilder()
.AddJsonFile("AppSettings.json")
.AddEnvironmentVariables()
.Build();
AppSettings settings = config.GetRequiredSection("AppSettings").Get<AppSettings>();

var Client = new DiscordClient(new ClientConfiguration(settings.Token, 10));
await Client.StartClientAsync();
await Client.LoginAsync();

await Client.AddCommandAsync(new Discord.Models.CommandBase {
    name = "diga",
    type = Discord.Enuns.CommandType.CHAT_INPUT,
    description = "Diz alguma coisa"
});

await Task.Delay(-1);
/*

builder.ConfigureServices(services =>
    services.AddSingleton<IDiscordClient, DiscordClient>(x => 
        x.ConfigureClient(settings.Token))
    .AddScoped<ITestService, TestService>()
    );

builder.ConfigureServices(services =>
    services.AddSingleton<IDiscordClient, DiscordClient>(x =>
        x.InitWithoutConfig(settings.Token))
    .AddScoped<IDataBaseService, DataBaseService>());

var host = builder.Build();

IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;
IDiscordClient client = provider.GetRequiredService<IDiscordClient>();

//await client.Run();
//await host.RunAsync();
*/

