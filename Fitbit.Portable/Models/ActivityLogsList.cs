namespace Fitbit.Api.Portable.Models
{
    using System;
    using System.Collections.Generic;

    using Fitbit.Models;

    using Newtonsoft.Json;

    public class ActivityLogsList
    {
        [JsonProperty(PropertyName = "activities")]
        public List<Activities> Activities{ get; set; }
    }
}
