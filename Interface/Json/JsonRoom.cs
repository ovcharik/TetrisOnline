using System;
using System.Collections.Generic;
using Newtonsoft;
using Newtonsoft.Json;

namespace Interface.Json
{
    [JsonObject]
    public class JsonRoom
    {
        [JsonProperty("RoomId")]
        public Int32 RoomId { get; set; }

        [JsonProperty("CreatorId")]
        public Int32 CreatorId { get; set; }

        [JsonProperty("Capacity")]
        public Int32 Capacity { get; set; }

        [JsonProperty("Name")]
        public String Name { get; set; }

        [JsonProperty("Members")]
        public List<Int32> Members { get; set; }

        [JsonProperty("Watchers")]
        public List<Int32> Watchers { get; set; }

        [JsonProperty("isGameStarted")]
        public Boolean isGameStarted { get; set; }
    }
}
