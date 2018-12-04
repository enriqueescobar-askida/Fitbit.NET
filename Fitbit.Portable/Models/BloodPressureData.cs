namespace Fitbit.Models
{
    using System.Collections.Generic;

    public class BloodPressureData
    {
        public BloodPressureAverage Average { get; set; }
        public List<BloodPressure> BP { get; set; }
    }
}
