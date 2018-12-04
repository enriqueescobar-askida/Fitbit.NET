namespace Fitbit.Portable.Tests.Models
{
    using System.Collections.Generic;

    using Fitbit.Models;

    using NUnit.Framework;

    [TestFixture]
    public class ActivitySummaryTests
    {
        [Test] [Category("Portable")]
        public void GetDistances_EmptyList()
        {
            ActivitySummary model = new ActivitySummary();
            model.Distances = new List<ActivityDistance>();

            Dictionary<string, float> d = model.GetDistancesAsDictionary();
            Assert.IsNotNull(d);
            Assert.AreEqual(0, d.Count);
        }

        [Test] [Category("Portable")]
        public void GetDistances_NullList()
        {
            ActivitySummary model = new ActivitySummary();

            Dictionary<string, float> d = model.GetDistancesAsDictionary();
            Assert.IsNotNull(d);
            Assert.AreEqual(0, d.Count);
        }

        [Test] [Category("Portable")]
        public void GetDistances_InstantiatedList()
        {
            ActivitySummary model = new ActivitySummary();
            model.Distances = new List<ActivityDistance>();
            model.Distances.Add(new ActivityDistance { Distance = 12, Activity = "test#1" });
            model.Distances.Add(new ActivityDistance { Distance = 0.56F, Activity = "test#2" });

            Dictionary<string, float> d = model.GetDistancesAsDictionary();
            Assert.IsNotNull(d);
            Assert.AreEqual(2, d.Count);

            Assert.IsTrue(d.ContainsKey("test#1"));
            float ad = d["test#1"];
            Assert.AreEqual(12, ad);

            Assert.IsTrue(d.ContainsKey("test#2"));
            ad = d["test#2"];
            Assert.AreEqual(0.56F, ad);
        }
    }
}