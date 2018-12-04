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
    public class ActivityLogTests
    {
        [Test]
        [Category("Portable")]
        public async Task GetActivityLogsListAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetActivityLogsList.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/activities/list.json?afterDate=2017-01-01&sort=asc&limit=20&offset=0", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            ActivityLogsList response = await fitbitClient.GetActivityLogsListAsync(null, new DateTime(2017, 1, 1));
            ValidateActivity(response.Activities);
        }

        [Test]
        [Category("Portable")]
        public void GetActivityLogsListAsync_Errors()
        {
            Func<HttpResponseMessage> responseMessage = Helper.CreateErrorResponse(HttpStatusCode.BadRequest);
            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            Func<Task<ActivityLogsList>> result = () => fitbitClient.GetActivityLogsListAsync(null, new DateTime(2017, 1, 1));

            result.ShouldThrow<FitbitRequestException>().Which.ApiErrors.Count.Should().Be(1);
        }

        [Test]
        [Category("Portable")]
        public void ActivityLogsList_JsonParse_Errors()
        {
            string content = SampleDataHelper.GetContent("GetActivityLogsList2.json");
            JsonSerializerSettings settings = new JsonSerializerSettings() { DateParseHandling = DateParseHandling.DateTimeOffset };
            JsonDotNetSerializer serializer = new JsonDotNetSerializer(settings) { RootProperty = "activities" };

            try
            {
                serializer.Deserialize<ActivityLogsList>(content);
            }
            catch (FitbitParseException e){
                // Success
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_ActivityLogsList()
        {
            string content = SampleDataHelper.GetContent("GetActivityLogsList.json");
            JsonSerializerSettings settings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTimeOffset };
            ActivityLogsList logList = JsonConvert.DeserializeObject<ActivityLogsList>(content, settings);

            ValidateActivity(logList.Activities);
        }

        [Test]
        [Category("Portable")]
        public async Task Get_Multiple_ActivityLogsList()
        {
            ActivityLogsList response = await GetActivityLogsList2(new DateTime(2017, 9, 4));
            response.Activities.Count.Should().Be(2);
        }

        [Test]
        [Category("Portable")]
        public async Task Check_TimeZone_ActivityLogsList()
        {
            ActivityLogsList response = await GetActivityLogsList2(new DateTime(2017, 9, 4));
            const string origOffset = "-07:00:00";

            response.Activities[0].StartTime.Offset.ToString().Should().Be(origOffset);
            response.Activities[1].StartTime.Offset.ToString().Should().Be(origOffset);
        }

        private async Task<ActivityLogsList> GetActivityLogsList2(DateTime date)
        {
            string content = SampleDataHelper.GetContent("GetActivityLogsList2.json");
            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/activities/list.json?afterDate=2017-09-04&sort=asc&limit=20&offset=0", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            ActivityLogsList response = await fitbitClient.GetActivityLogsListAsync(date);
            return response;
        }

        private void ValidateActivity(List<Activities> stats)
        {
            Activities stat = stats.First();

            stat.ActiveDuration.Should().Be(2764000);
            stat.ActivityLevel.First().Minutes.Should().Be(0);
            stat.ActivityLevel.First().Name.Should().Be("sedentary");
            stat.ActivityName.Should().Be("Walk");
            stat.ActivityTypeId.Should().Be(90013);
            stat.AverageHeartRate.Should().Be(108);
            stat.Calories.Should().Be(375);
            stat.Duration.Should().Be(2764000);
            stat.ElevationGain.Should().Be(3.048);

            HeartRateZone zone1 = stat.HeartRateZones[0];
            zone1.CaloriesOut.Should().Be(default(double));
            zone1.Max.Should().Be(95);
            zone1.Min.Should().Be(30);
            zone1.Minutes.Should().Be(3);
            zone1.Name.Should().Be("Out of Range");

            HeartRateZone zone4 = stat.HeartRateZones[3];
            zone4.CaloriesOut.Should().Be(default(double));
            zone4.Max.Should().Be(220);
            zone4.Min.Should().Be(162);
            zone4.Minutes.Should().Be(0);
            zone4.Name.Should().Be("Peak");

            stat.LastModified.Should().Be(new DateTime(2017, 01, 01, 5, 3, 50));
            stat.LogId.Should().Be(5390522508);
            stat.LogType.Should().Be("auto_detected");

            stat.ManualValuesSpecified.Calories.Should().Be(false);
            stat.ManualValuesSpecified.Distance.Should().Be(false);
            stat.ManualValuesSpecified.Steps.Should().Be(false);

            stat.OriginalDuration.Should().Be(2764000);
            stat.OriginalStartTime.Should().Be(new DateTime(2017, 1, 1, 4, 14, 06));
            stat.StartTime.Should().Be(new DateTime(2017, 1, 1, 4, 14, 06));
            stat.Steps.Should().Be(5138);
            stat.TcxLink.Should().Be("https://api.fitbit.com/1/user/-/activities/5390522508.tcx");
        }
    }
}

