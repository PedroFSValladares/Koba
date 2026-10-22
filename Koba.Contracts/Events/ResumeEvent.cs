using System.Text.Json.Serialization;

namespace Koba.Contracts.Events;

public record ResumeEvent : EventBase
{
    [JsonPropertyName("d")]
    public ResumeData Data { get; set; }
    public ResumeEvent(string botToken, string session_id, int sequencialNumber) : base(6, null, null)
    {
        Data = new ResumeData
        {
            BotToken = botToken,
            SessionId = session_id,
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
    public int SequencialNumber { get; init; }
};