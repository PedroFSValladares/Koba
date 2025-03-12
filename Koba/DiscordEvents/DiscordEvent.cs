using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Koba.DiscordEvents
{
    public class DiscordEvent
    {
        [JsonPropertyName("op")]
        public int? OpCode { get; set; }

        [JsonPropertyName("d")]
        public JsonDocument? EventData { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public string? EventName { get; set; }

        public override string ToString() {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions {
                WriteIndented = true
            });
        }
    }
}
