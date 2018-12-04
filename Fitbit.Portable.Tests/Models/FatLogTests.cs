namespace Fitbit.Portable.Tests.Models
{
    using System;

    using Fitbit.Models;

    using NUnit.Framework;

    [TestFixture]
    public class FatLogTests
    {
        [Test] [Category("Portable")]
        public void FatLog_DateTime_Mash()
        {
            DateTime date = new DateTime(2014, 10, 2);
            DateTime time = new DateTime(1970, 1, 1, 12, 45, 56);
            FatLog fatlog = new FatLog();
            fatlog.Date = date;
            fatlog.Time = time;

            Assert.AreEqual(new DateTime(2014, 10, 2, 12, 45, 56), fatlog.DateTime);
        }
    }
}
