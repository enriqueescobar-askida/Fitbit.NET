﻿namespace Fitbit.Portable.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Fitbit.Api.Portable;
    using Fitbit.Models;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class IntradayTimeSeriesTests
    {
        [Test]
        [Category("Portable")]
        public async Task GetIntraDayTimeSeriesCaloriesIntensityMetsAsync_Success()
        {
            DateTime expectedResult = new DateTime(2015, 3, 20, 0, 1, 0);

            string content = SampleDataHelper.GetContent("IntradayActivitiesCalories.json");
            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                message.Method.Should().Be(HttpMethod.Get);
                message.RequestUri.AbsoluteUri.Should().Be("https://api.fitbit.com/1/user/-/activities/calories/date/2015-03-20/1d.json");
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);
            IntradayData response = await fitbitClient.GetIntraDayTimeSeriesAsync(IntradayResourceType.CaloriesOut, new DateTime(2015, 3, 20), new TimeSpan(24, 0, 0));

            response.DataSet[1].Time.Should().Be(expectedResult);
            response.DataSet[1].METs.Should().Be("10");
            response.DataSet[1].Level.Should().Be("0");
            response.DataSet[1].Value.Should().Be("1.1857000589370628");
        }

        [Test]
        [Category("Portable")]
        public async Task GetIntraDayTimeSeriesCaloriesIntensityMetsAsync_ReturnsNullIfMissingDateTime()
        {
            string content = SampleDataHelper.GetContent("IntradayActivitiesCaloriesMissingDateTime.json");
            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                message.Method.Should().Be(HttpMethod.Get);
                message.RequestUri.AbsoluteUri.Should().Be("https://api.fitbit.com/1/user/-/activities/calories/date/2015-03-20/1d.json");
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);
            IntradayData response = await fitbitClient.GetIntraDayTimeSeriesAsync(IntradayResourceType.CaloriesOut, new DateTime(2015, 3, 20), new TimeSpan(24, 0, 0));

            response.Should().Be(null);
        }

        [Test]
        [Category("Portable")]
        public async Task GetIntraDayTimeSeriesStepsAsync_Success()
        {
            DateTime expectedResult = new DateTime(2016, 3, 8, 0, 1, 0);

            string content = SampleDataHelper.GetContent("IntradayActivitiesSteps.json");
            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                message.Method.Should().Be(HttpMethod.Get);
                message.RequestUri.AbsoluteUri.Should().Be("https://api.fitbit.com/1/user/-/activities/steps/date/2016-03-08/1d.json");
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);
            IntradayData response = await fitbitClient.GetIntraDayTimeSeriesAsync(IntradayResourceType.Steps, new DateTime(2016, 3, 8), new TimeSpan(24, 0, 0));

            response.DataSet[1].Time.Should().Be(expectedResult);
            response.DataSet[1].Value.Should().Be("2");

        }
    }
}
