namespace Fitbit.Models
{
    using Newtonsoft.Json;

    public class ActivitiesStats
    {
        [JsonProperty("best")]
        public BestStats Best { get; set; }

        [JsonProperty("lifetime")]
        public LifetimeStats Lifetime { get; set; }
    }
}