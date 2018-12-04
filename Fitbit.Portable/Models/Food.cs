namespace Fitbit.Models
{
    using System.Collections.Generic;

    public class Food
    {
        public FoodSummary Summary { get; set; }
        public List<FoodLog> Foods { get; set; }
        public FoodGoals Goals { get; set; }
    }
}