﻿namespace Fitbit.Portable.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Fitbit.Api.Portable;
    using Fitbit.Models;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class WeightTests
    {
        public Fixture fixture { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            fixture = new Fixture();
        }

        [Test]
        [Category("Portable")]
        public void GetWeightAsync_DateRangePeriod_ThreeMonths()
        {
            FitbitClient client = fixture.Create<FitbitClient>();
            Assert.That(
                new AsyncTestDelegate(async () => await client.GetWeightAsync(DateTime.Now, DateRangePeriod.ThreeMonths)),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
            );
        }

        [Test]
        [Category("Portable")]
        public void GetWeightAsync_DateRangePeriod_SixMonths()
        {
            FitbitClient client = fixture.Create<FitbitClient>();
            Assert.That(
                new AsyncTestDelegate(async () => await client.GetWeightAsync(DateTime.Now, DateRangePeriod.SixMonths)),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
            );
        }

        [Test]
        [Category("Portable")]
        public void GetWeightAsync_DateRangePeriod_OneYear()
        {
            FitbitClient client = fixture.Create<FitbitClient>();
            ;
            Assert.That(
                new AsyncTestDelegate(async () => await client.GetWeightAsync(DateTime.Now, DateRangePeriod.OneYear)),
                Throws.InstanceOf<ArgumentOutOfRangeException>()
            );
        }

        [Test]
        [Category("Portable")]
        public void GetWeightAsync_DateRangePeriod_Max()
        {
            FitbitClient client = fixture.Create<FitbitClient>();

            Assert.That(
                    new AsyncTestDelegate(async () => await client.GetWeightAsync(DateTime.Now, DateRangePeriod.Max)),
                    Throws.InstanceOf<ArgumentOutOfRangeException>()
                );
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_OneDay_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/1d.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), DateRangePeriod.OneDay);

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_SevenDay_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/7d.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), DateRangePeriod.SevenDays);

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_OneWeek_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/1w.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), DateRangePeriod.OneWeek);

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_ThirtyDays_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/30d.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), DateRangePeriod.ThirtyDays);

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_OneMonth_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/1m.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), DateRangePeriod.OneMonth);

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5));

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetWeightAsync_TimeSpan_Success()
        {
            FitbitClient fitbitClient = SetupFitbitClient("https://api.fitbit.com/1/user/-/body/log/weight/date/2012-03-05/2012-03-06.json");

            Weight response = await fitbitClient.GetWeightAsync(new DateTime(2012, 3, 5), new DateTime(2012, 3, 6));

            ValidateWeight(response);
        }

        [Test]
        [Category("Portable")]
        public void GetWeightAsync_DateRange_Span_Too_Large()
        {
            FitbitClient fitbitClient = Helper.CreateFitbitClient(() => new HttpResponseMessage(), (r, c) => { });
            DateTime basedate = DateTime.Now;

            Assert.That(
                    new AsyncTestDelegate(async () => await fitbitClient.GetWeightAsync(basedate.AddDays(-35), basedate)),
                    Throws.InstanceOf<ArgumentOutOfRangeException>()
                );
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_Empty_String()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            Assert.That(
                new TestDelegate(() => deserializer.GetWeight(string.Empty)),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_Null_String()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            Assert.That(
                new TestDelegate(() => deserializer.GetWeight(null)),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_WhiteSpace()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            Assert.That(
                new TestDelegate(() => deserializer.GetWeight("         ")),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_Weight()
        {
            string content = SampleDataHelper.GetContent("GetWeight.json");
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            Weight weight = deserializer.GetWeight(content);

            ValidateWeight(weight);
        }

        private FitbitClient SetupFitbitClient(string url)
        {
            string content = SampleDataHelper.GetContent("GetWeight.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual(url, message.RequestUri.AbsoluteUri);
            });

            return Helper.CreateFitbitClient(responseMessage, verification);
        }

        private void ValidateWeight(Weight weight)
        {
            Assert.IsNotNull(weight);

            Assert.AreEqual(2, weight.Weights.Count);

            WeightLog log = weight.Weights.First();
            Assert.IsNotNull(log);

            Assert.AreEqual(new DateTime(2012, 3, 5), log.Date);
            Assert.AreEqual(1330991999000, log.LogId);
            Assert.AreEqual(73f, log.Weight);
            Assert.AreEqual(23.57f, log.Bmi);
            Assert.AreEqual(new DateTime(2012, 3, 5, 23, 59, 59).TimeOfDay, log.Time.TimeOfDay);

            weight.Weights.Remove(log);
            log = weight.Weights.First();

            Assert.IsNotNull(log);

            Assert.AreEqual(new DateTime(2012, 3, 5), log.Date);
            Assert.AreEqual(1330991999000, log.LogId);
            Assert.AreEqual(72.5f, log.Weight);
            Assert.AreEqual(22.57f, log.Bmi);
            Assert.AreEqual(new DateTime(2012, 3, 5, 21, 10, 59).TimeOfDay, log.Time.TimeOfDay);
        }
    }
}