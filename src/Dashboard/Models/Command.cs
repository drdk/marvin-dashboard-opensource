using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DR.Marvin.Dashboard.Models
{
    public class Command
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CommandType Type { get; set; }
        public string Urn { get; set; }
        public string Username { get; set; }
    }

    public enum CommandType
    {
        Unknow = 0,
        Cancel = 1,
        Pause = 2,
        Resume = 3
    }
}