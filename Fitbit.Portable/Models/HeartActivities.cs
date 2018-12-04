namespace Fitbit.Models
{
    using System;
    using System.Collections.Generic;

    public class HeartActivities
    {
        public DateTime DateTime { get; set; }
        public List<HeartRateZone> HeartRateZones { get; set; }
        public List<HeartRateZone> CustomHeartRateZones { get; set; }
        public int RestingHeartRate { get; set; }
    }
}
