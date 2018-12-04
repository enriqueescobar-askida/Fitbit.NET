namespace Fitbit.Portable.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Fitbit.Api.Portable;
    using Fitbit.Models;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    [TestFixture]
    public class GoalsTests
    {
        public Fixture fixture { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            fixture = new Fixture();
        }

        [Test] [Category("Portable")]
        public void SetGoalsAsync_NoGoalsSet()
        {
            FitbitClient client = fixture.Create<FitbitClient>();
            Assert.That(new AsyncTestDelegate(async () => await client.SetGoalsAsync()), Throws.ArgumentException);

        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_CaloriesOutSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("caloriesOut=2000");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(caloriesOut: 2000);

            Assert.IsNotNull(response);
        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_DistanceSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("distance=8.5");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(distance: 8.5M);

            Assert.IsNotNull(response);
        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_FloorsSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("floors=20");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(floors: 20);

            Assert.IsNotNull(response);
        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_StepsSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("steps=10000");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(steps: 10000);

            Assert.IsNotNull(response);
        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_ActiveMinuitesSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("activeMinutes=50");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(activeMinutes: 50);

            Assert.IsNotNull(response);
        }

        [Test] [Category("Portable")]
        public async Task SetGoalsAsync_AllSet()
        {
            FitbitClient fitbitClient = SetupFitbitClient("caloriesOut=2000&distance=8.5&floors=20&steps=10000&activeMinutes=50");

            ActivityGoals response = await fitbitClient.SetGoalsAsync(2000, 8.5M, 20, 10000, 50);

            Assert.IsNotNull(response);
        }

        public FitbitClient SetupFitbitClient(string expectedBody)
        {
            string content = SampleDataHelper.GetContent("ActivityGoals.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>(async (message, token) =>
            {
                Assert.AreEqual(HttpMethod.Post, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/activities/goals/daily.json", message.RequestUri.AbsoluteUri);

                string body = await message.Content.ReadAsStringAsync();
                Assert.AreEqual(true, body.Equals(expectedBody));
            });

            return Helper.CreateFitbitClient(responseMessage, verification);
        }
    }
}
