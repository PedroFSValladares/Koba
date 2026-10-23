using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record ResumeEvent : EventBase
{
    [JsonPropertyName("d")]
    public ResumeData Data { get; set; }
    public ResumeEvent(string botToken, string sessionId, int? sequencialNumber) : base(6, null, null)
    {
        Data = new ResumeData
        {
            BotToken = botToken,
            SessionId = sessionId,
            SequencialNumber = sequencialNumber
        };
    }
}

public record ResumeData
{
    [JsonPropertyName("token")]
    public string BotToken {get; init; }
    [JsonPropertyName("session_id")]
    public string SessionId {get; init; }
    [JsonPropertyName("seq")]
    public int? SequencialNumber { get; init; }
};