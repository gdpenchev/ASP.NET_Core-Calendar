namespace Schedule.Data.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    public class MeetingRoom
    {
        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("capacity")]
        public long Capacity { get; set; }

        [JsonProperty("availableFrom")]
        public string AvailableFrom { get; set; }

        [JsonProperty("availableTo")]
        public string AvailableTo { get; set; }

        [JsonProperty("schedule")]
        public List<Schedules> Schedule { get; set; }
    }
}
