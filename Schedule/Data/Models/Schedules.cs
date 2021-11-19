namespace Schedule.Data.Models
{
    using System;
    using Newtonsoft.Json;
    public class Schedules
    {
        [JsonProperty("from")]
        public DateTime From { get; set; } 

        [JsonProperty("to")]
        public DateTime To { get; set; } 
    }
}
