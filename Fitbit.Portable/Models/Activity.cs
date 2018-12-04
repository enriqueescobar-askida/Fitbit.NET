namespace Fitbit.Models
{
    using System.Collections.Generic;

    public class Activity
    {
        public ActivitySummary Summary { get; set; }
        public List<ActivityLog> Activities { get; set; }
        public ActivityGoals Goals { get; set; }

    }
}
