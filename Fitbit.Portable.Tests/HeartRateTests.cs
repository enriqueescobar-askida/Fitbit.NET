using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fitbit.Api.Portable;
using Fitbit.Api.Portable.Models;
using Fitbit.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Fitbit.Portable.Tests
{
    [TestFixture]
    public class HeartRateTests
    {
        [Test]
        [Category("Portable")]
        public async Task GetHeartRateTimeSeriesAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetHeartRateTimeSeries.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                string uri = $"https:/" + $"/api.fitbit.com/1.1/user/-/activities/heart/date/{DateTime.Today:yyyy-MM-dd}/1d.json";
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual(uri, message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            HeartActivitiesTimeSeries response = await fitbitClient.GetHeartRateTimeSeries(DateTime.Today, DateRangePeriod.OneDay);
            ValidateHeartRateTimeSeriesData(response);
        }

        [Test]
        [Category("Portable")]
        public void GetHeartRateTimeSeriesAsync_Errors()
        {
            Func<HttpResponseMessage> responseMessage = Helper.CreateErrorResponse(HttpStatusCode.BadRequest);
            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            Func<Task<HeartActivitiesTimeSeries>> result = () => fitbitClient.GetHeartRateTimeSeries(DateTime.MinValue, DateRangePeriod.OneDay);

            result.ShouldThrow<FitbitException>().Which.ApiErrors.Count.Should().Be(1);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_HeartRateTimeSeries()
        {
            //assemble
            string content = SampleDataHelper.GetContent("GetHeartRateTimeSeries.json");
            JsonDotNetSerializer seralizer = new JsonDotNetSerializer();

            //act
            HeartActivitiesTimeSeries stats = seralizer.GetHeartActivitiesTimeSeries(content);

            //assert
            ValidateHeartRateTimeSeriesData(stats);
        }

        [Test]
        [Category("Portable")]
        public async Task GetHeartRateIntradayTimeSeriesAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetHeartRateIntradayTimeSeries1.1.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                string uri = $"https:/" + $"/api.fitbit.com/1.1/user/-/activities/heart/date/{DateTime.Today:yyyy-MM-dd}/{DateTime.Today:yyyy-MM-dd}/15min/time/00:00:00/23:59:59.json";
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual(uri, message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            HeartActivitiesIntraday response = await fitbitClient.GetHeartRateIntraday(DateTime.Today, HeartRateResolution.fifteenMinute);
            ValidateHeartRateIntradayTimeSeriesData(response);
        }

        [Test]
        [Category("Portable")]
        public void GetHeartRateIntradayTimeSeriesAsync_Errors()
        {
            Func<HttpResponseMessage> responseMessage = Helper.CreateErrorResponse(HttpStatusCode.BadRequest);
            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            Func<Task<HeartActivitiesIntraday>> result = () => fitbitClient.GetHeartRateIntraday(DateTime.MinValue, HeartRateResolution.fifteenMinute);

            result.ShouldThrow<FitbitException>().Which.ApiErrors.Count.Should().Be(1);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_HeartRateIntradayTimeSeries()
        {
            //assemble
            string content = SampleDataHelper.GetContent("GetHeartRateIntradayTimeSeries1.1.json");
            DateTime date = DateTime.Parse("2017-08-21"); //hardcoded because extension expects a date. In any other use case, a date would be available
            JsonDotNetSerializer seralizer = new JsonDotNetSerializer();

            //act
            HeartActivitiesIntraday stats = seralizer.GetHeartRateIntraday(date, content);

            //assert
            ValidateHeartRateIntradayTimeSeriesData(stats);
        }

        private void ValidateHeartRateIntradayTimeSeriesData(HeartActivitiesIntraday activity)
        {
            //Activities Heart Intraday
            HeartActivitiesIntraday actIntraday = activity;

            actIntraday.Dataset.Count().Should().Be(96); //Dataset count

            actIntraday.Dataset[0].Time.TimeOfDay.Should().Be(new TimeSpan(0,0,0,0)); //First entry
            actIntraday.Dataset[0].Value.Should().Be(57);

            actIntraday.Dataset[95].Time.TimeOfDay.Should().Be(new TimeSpan(0, 23,45, 0)); //Last entry
            actIntraday.Dataset[95].Value.Should().Be(47);

            actIntraday.DatasetInterval.Should().Be(15); //Dataset interval

            actIntraday.DatasetType.Should().Be("minute"); //Dataset Type

            //Activities Heart
            IntradayActivitiesHeart act = activity.ActivitiesHeart;

            act.DateTime.Should().Be(new DateTime(2017, 8, 21)); //DateTime

            act.HeartRateZones.Count().Should().Be(4); //Zones Count

            act.HeartRateZones[0].CaloriesOut.Should().Be(2071.96748); //First zone
            act.HeartRateZones[0].Max.Should().Be(92);
            act.HeartRateZones[0].Min.Should().Be(30);
            act.HeartRateZones[0].Minutes.Should().Be(1387);
            act.HeartRateZones[0].Name.Should().Be("Out of Range");

            act.HeartRateZones[3].CaloriesOut.Should().Be(186.84666); //Last zone
            act.HeartRateZones[3].Max.Should().Be(220);
            act.HeartRateZones[3].Min.Should().Be(156);
            act.HeartRateZones[3].Minutes.Should().Be(14);
            act.HeartRateZones[3].Name.Should().Be("Peak");

            act.CustomHeartRateZones.Count().Should().Be(0); //Empty CustomHeart Rate Zones

            act.Value.Should().Be(55.44); //Value

        }

        private void ValidateHeartRateTimeSeriesData(HeartActivitiesTimeSeries activities)
        {
            List<HeartActivities> act = activities.HeartActivities;

            act.Count().Should().Be(7);

            HeartActivities firstAct = act[0];
            firstAct.DateTime.Should().Be(new DateTime(2017, 08, 15));

            firstAct.CustomHeartRateZones.Count().Should().Be(0); //Empty Custom Heart Rate Zones

            firstAct.HeartRateZones.Count().Should().Be(4); //Heart Rate Zones Count

            firstAct.HeartRateZones[0].CaloriesOut.Should().Be(2257.6846); //First zone
            firstAct.HeartRateZones[0].Max.Should().Be(92);
            firstAct.HeartRateZones[0].Min.Should().Be(30);
            firstAct.HeartRateZones[0].Minutes.Should().Be(1264);
            firstAct.HeartRateZones[0].Name.Should().Be("Out of Range");

            firstAct.HeartRateZones[3].CaloriesOut.Should().Be(287.33136); //Last zone
            firstAct.HeartRateZones[3].Max.Should().Be(220);
            firstAct.HeartRateZones[3].Min.Should().Be(156);
            firstAct.HeartRateZones[3].Minutes.Should().Be(21);
            firstAct.HeartRateZones[3].Name.Should().Be("Peak");

            firstAct.RestingHeartRate.Should().Be(51);
        }
    }
}
