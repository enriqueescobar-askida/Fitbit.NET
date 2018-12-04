namespace Fitbit.Api.Portable.Models
{
    using System.Collections.Generic;

    public class SleepData
    {
        public List<SleepLog> Sleep { get; set; }
        public SleepSummary Summary { get; set; }
    }
}