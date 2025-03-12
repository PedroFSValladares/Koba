using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Koba.Models
{
    public class DiscordUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

        [JsonPropertyName("avatar")]
        public string AvatarId { get; set; }

        [JsonPropertyName("bot")]
        public bool IsBot { get; set; }

        [JsonIgnore]
        public bool CurrentUser { get; set; }
    }
}
