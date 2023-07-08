using Discord.Enuns;

namespace Discord.Models {
    public class CommandBase {
        public string id { get; set; }
        public string name { get; set; }
        public string application_id { get; set; }
        public string guild_id { get; set; }
        public CommandType type { get; set; }
        public string description { get; set; }
        public Dictionary<string, string>? name_localizations { get; set; }
        public CommandOptions? options { get; set; }
        public string default_member_permissions { get; set; }
        public bool? dm_permission { get; set; }
        public bool? default_permission { get; set; }
        public bool? nsfw { get; set; }
        public string version { get; set; }
    }

    public class CommandOptions {
        public CommandOptionType type { get; set; }
        public string name { get; set; }
        public Dictionary<string, string>? name_localizations { get; set; }
        public string description { get; set; }
        public Dictionary<string, string>? description_localizations { get; set; }
        public bool? required { get; set; }
        public Choice[]? choices { get; set; }
        public CommandOptions[]? options { get; set; }
        public ChannelType[]? channel_types { get; set; }
        public double? min_value { get; set; }
        public double? max_value { get; set;}
        public int? min_lenght { get; set; }
        public int? max_lenght { get; set; }
        public bool? autocomplete { get; set; }
    }

    public class Choice {
        public string name { get; set; }
        public Dictionary<string, string>? name_localizations { get; set; }
        public dynamic value { get; set; }
    }
}
