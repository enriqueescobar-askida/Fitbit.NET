namespace Fitbit.Models
{
    using System;

    using Newtonsoft.Json;

    public class DistanceStats
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}