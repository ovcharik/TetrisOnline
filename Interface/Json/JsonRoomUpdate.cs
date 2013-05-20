using System;
using Newtonsoft.Json;

namespace Interface.Json
{
    [JsonObject]
    public class JsonRoomUpdate
    {
        [JsonProperty("UserId")]
        public Int32 UserId { get; set; }

        [JsonProperty("RoomId")]
        public Int32 RoomId { get; set; }
    }
}
