namespace Fitbit.Models
{
    using Newtonsoft.Json;

    public class LifetimeStats
    {
        [JsonProperty("total")]
        public LifetimeTotals Total { get; set; }

        [JsonProperty("tracker")]
        public LifetimeTotals Tracker { get; set; }
    }
}