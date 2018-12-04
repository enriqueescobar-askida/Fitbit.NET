namespace Fitbit.Portable.Tests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Fitbit.Api.Portable;
    using Fitbit.Models;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class WaterTests
    {
        [Test] [Category("Portable")]
        public async Task GetWaterAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetWater-WaterData.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/foods/log/water/date/2015-01-12.json", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            WaterData response = await fitbitClient.GetWaterAsync(new DateTime(2015, 1, 12));

            ValidateWaterData(response);
        }

        [Test] [Category("Portable")]
        public void GetWaterAsync_Errors()
        {
            Func<HttpResponseMessage> responseMessage = Helper.CreateErrorResponse();
            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            Func<Task<Food>> result = () => fitbitClient.GetFoodAsync(new DateTime(2015, 1, 12));

            result.ShouldThrow<FitbitException>();
        }

        [Test] [Category("Portable")]
        public async Task PostWaterLogAsync_Success()
        {
            string content = SampleDataHelper.GetContent("LogWater-WaterLog.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Post, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/foods/log/water.json?amount=300&date=2015-01-12", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            WaterLog response = await fitbitClient.LogWaterAsync(new DateTime(2015, 1, 12), new WaterLog { Amount = 300 });

            Assert.AreEqual(300, response.Amount);
        }

        [Test] [Category("Portable")]
        public async Task DeleteWaterLogAsync_Success()
        {
            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Delete, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/foods/log/water/1234.json", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            await fitbitClient.DeleteWaterLogAsync(1234);

            //Assert.IsTrue(response.Success);
        }

        [Test] [Category("Portable")]
        public void Can_Deserialize_Water_Data_Json()
        {
            string content = SampleDataHelper.GetContent("GetWater-WaterData.json");

            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            WaterData result = deserializer.Deserialize<WaterData>(content);

            ValidateWaterData(result);
        }

        [Test] [Category("Portable")]
        public void Can_Deserialize_Water_Log_Json()
        {
            string content = SampleDataHelper.GetContent("LogWater-WaterLog.json");

            JsonDotNetSerializer deserializer = new JsonDotNetSerializer { RootProperty = "waterLog"};

            WaterLog result = deserializer.Deserialize<WaterLog>(content);

            Assert.IsNotNull(result);
            Assert.AreEqual(508728882, result.LogId);
            Assert.AreEqual(300, result.Amount);
        }

        private void ValidateWaterData(WaterData result)
        {
            WaterLog firstWaterLog = result.Water.FirstOrDefault();

            Assert.IsNotNull(result);
            Assert.AreEqual(1300, result.Summary.Water);
            Assert.AreEqual(5, result.Water.Count);

            Assert.IsNotNull(firstWaterLog);
            Assert.AreEqual(200, firstWaterLog.Amount);
            Assert.AreEqual(508693835, firstWaterLog.LogId);
        }
    }
}
