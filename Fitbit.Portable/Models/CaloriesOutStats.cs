namespace Fitbit.Models
{
    using System;

    using Newtonsoft.Json;

    public class CaloriesOutStats
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}