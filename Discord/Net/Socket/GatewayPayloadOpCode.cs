namespace Discord.Net.Socket
{
    public enum GatewayPayloadOpCode
    {
        Dispatch = 0,
        HeartBeat = 1,
        Identify = 2,
        PresenceUpdate = 3,
        VoiceStateUpdate = 4,
        Resume = 6,
        Reconnect = 7,
        RequestGuildMembers = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatAck = 11
    }
}
