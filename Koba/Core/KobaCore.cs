using Discord.Net.Rest;
using DotNetEnv;
using Koba.DiscordEvents;
using Koba.EventHandler;
using Koba.Heart;
using Koba.Queue;
using Koba.BotSocket;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Web;
using System.Net.WebSockets;
using Koba.Logging;
using System.Text.Json;
using Koba.Enuns;
using Koba.Models;
using Koba.DiscordContext;

namespace Koba.Core
{
    public class KobaCore
    {
        //public static readonly QueueManager appQueues = new();
        
        private readonly QueueManager appQueues;
        private readonly Dictionary<string, object> appCache;

        private readonly RestClient restClient;
        private readonly DiscordSocketClient botSocket;
        private readonly DiscordEventParser eventParser;
        private readonly DiscordEventHandler eventHandler;
        private readonly HeartBeater heartBeater;
        private readonly DiscordContextBuilder contextBuilder;
        //private readonly ILogger logger;
        private readonly ILogger logger;
        private readonly string logPath;

        public KobaCore() {
            logPath = Path.Combine(
            Environment.ProcessPath.Replace(
                Path.GetFileName(Environment.ProcessPath), ""),
            "logs",
            $"{DateOnly.FromDateTime(DateTime.Now).ToString().Replace('/', '-')}.txt");
            
            logger = LoggerFactory.Create(builder => {
                builder.AddSimpleConsole(configure => {
                    configure.TimestampFormat = "[HH:mm:ss] ";
                    configure.SingleLine = true;
                });
                builder.AddProvider(new BotLoggerProvider(new StreamWriter(logPath, true)));
            }).CreateLogger<KobaCore>();

            appQueues = new QueueManager();
            appCache = new Dictionary<string, object>();
            restClient = new RestClient();
            botSocket = new DiscordSocketClient();
            eventParser = new DiscordEventParser(botSocket);
            eventHandler = new DiscordEventHandler(eventParser, appCache);
            heartBeater = new HeartBeater(appCache, eventParser, logger);
            contextBuilder = new DiscordContextBuilder();
        }

        public async Task Run() {
            //appQueues.CreateQueue<string>("SocketMessagesReceived");
            //appQueues.CreateQueue<string>("SocketMessagesToSend");
            
            logger.LogInformation("Iniciando aplicação...");

            var request = new RestRequest(@"https://discord.com/api/v10/gateway/bot", Method.Get);
            request.AddHeader("Authorization", $"Bot {Env.GetString("BOT_TOKEN")}");
            var response = restClient.Get(request);

            if(response.IsSuccessStatusCode == false) {
                logger.LogError("Falha ao obter informações do gateway.");
                logger.LogError(response.ErrorMessage);
                return;
            }

            var responseContent = JsonSerializer.Deserialize<GatewayInfo>(response.Content);

            var uriBuilder = new UriBuilder(responseContent.url);
            var parameters = HttpUtility.ParseQueryString("");
            parameters["v"] = "10";
            parameters["encoding"] = "json";
            uriBuilder.Query = parameters.ToString();
            appCache.Add("GatewayUrl", uriBuilder.Uri);

            botSocket.OnMessageReceived += eventParser.ParseEvent;
            botSocket.OnComunicationFailed += SocketError;
            eventParser.OnEventDispached += eventHandler.ReceiveEvent;
            eventHandler.HelloEventReceived += heartBeater.StartHeartbeat;
            eventHandler.HeartbeatAckReceived += heartBeater.HeartBeatAckReceived;
            eventHandler.UnknowEventReceived += LogUnknowEvent;
            eventHandler.HeartbeatRequested += heartBeater.Beat;
            eventHandler.ReadyEventReceived += StoreBotInfo;
            heartBeater.HeartbeatTimedOut += ResetConnection;
            heartBeater.ConnectionSucceed += SendIdentify;

            logger.LogInformation("Conectando ao gateway...");

            await botSocket.ConnecToGatewayAsync(uriBuilder.Uri, CancellationToken.None);
        }

        private void StoreBotInfo(DiscordEvent discordEvent) {
            appCache.Add("ResumeGatewayUrl", discordEvent.EventData.RootElement.GetProperty("resume_gateway_url").GetString());
            appCache.Add("SessionId", discordEvent.EventData.RootElement.GetProperty("session_id").GetString());

            contextBuilder.SetBotUser(discordEvent.EventData.RootElement.Deserialize<DiscordUser>());
        }

        private void SendIdentify() {
            var indetfyObject = new {
                op = DiscordEventOpCode.Identify,
                d = new {
                    token = Env.GetString("BOT_TOKEN"),
                    intents = DiscordIntents.MESSAGE_CONTENT,
                    properties = new {
                        os = "windows",
                        browser = "KobaLibrary",
                        device = "KobaLibrary"
                    }
                }
            };
            eventParser.SerializeAndSend(indetfyObject).GetAwaiter();
            logger.LogInformation("Realizando login no Gateway...");
        }

        private void ResetConnection() {
            logger.LogWarning("Reiniciando conexão...");
            botSocket.StopAsync(CancellationToken.None).GetAwaiter();
            heartBeater.Stop();

            botSocket.ConnecToGatewayAsync((Uri)appCache["GatewayUrl"] ,CancellationToken.None).GetAwaiter();
        }

        private void LogUnknowEvent(DiscordEvent discordEvent) {
            logger.LogWarning($"Evento desconhecido recebido:");
            logger.LogWarning($"{discordEvent.ToString()}");
        }

        private void SocketError(ClientWebSocket socket, Exception exception) {
            logger.LogError(exception.Message);
            Environment.Exit(0);
        }
    }
}
