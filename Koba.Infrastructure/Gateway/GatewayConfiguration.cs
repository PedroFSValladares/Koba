using Koba.Contracts.Entities;
using Koba.Contracts.Enuns;
using Koba.Contracts.Events;

namespace Koba.Infrastructure.Gateway;

internal class GatewayConfiguration
{
    internal string GatewayToken { get; set; }
    internal Presence InitialPresence  { get; set; }
    internal DiscordIntents Intent { get; set; }
    internal int LargeThreshold { get; set; }
    internal bool UseCompression  { get; set; }
    internal AppProperties AppProperties  { get; set; }
    internal string ReceiveMessageTopicName { get; set; }
    internal string SendMessageTopicName { get; set; }
}