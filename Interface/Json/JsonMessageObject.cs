using System;
using Newtonsoft;
using Newtonsoft.Json;

namespace Interface.Json
{
    [JsonObject]
    public class JsonMessageObject
    {
        [JsonProperty("UserId")]
        public Int32 UserId { get; set; }

        [JsonProperty("DateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("Data")]
        public String Data { get; set; }
    }
}
