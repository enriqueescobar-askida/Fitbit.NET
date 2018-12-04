namespace Fitbit.Models
{
    using Newtonsoft.Json;

    public class BestStats
    {
        [JsonProperty("total")]
        public BestTotals Total { get; set; }

        [JsonProperty("tracker")]
        public BestTotals Tracker { get; set; }
    }
}