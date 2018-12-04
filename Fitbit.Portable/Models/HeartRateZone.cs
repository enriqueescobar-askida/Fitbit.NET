namespace Fitbit.Models
{
    using Newtonsoft.Json;

    public class HeartRateZone
    {
        [JsonProperty(PropertyName = "caloriesOut")]
        public double CaloriesOut { get; set; }

        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }

        [JsonProperty(PropertyName = "minutes")]
        public int Minutes { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
