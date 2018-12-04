namespace Fitbit.Models
{
    using System;
    using System.Collections.Generic;

    public class TimeSeriesDataList
    {
        public List<Data> DataList { get; set; }

        public class Data
        {
            public DateTime DateTime { get; set; }
            public string Value { get; set; }
        }
    }
}