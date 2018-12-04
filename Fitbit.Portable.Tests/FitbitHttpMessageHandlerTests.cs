namespace Fitbit.Portable.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Threading;
    using Fitbit.Api.Portable;
    using NUnit.Framework;
    using Fitbit.Api.Portable.OAuth2;
    using Fitbit.Portable.Tests.Helpers;
    using Moq;
    [TestFixture]
    public class FitbitHttpMessageHandlerTests
    {
        FitbitAppCredentials dummyCredentials = new FitbitAppCredentials();
        OAuth2AccessToken dummyToken = new OAuth2AccessToken();

        [Test]
        [Category("Portable")]
        [Category("Interceptor")]

        public async Task CanSnifftHttpRequests()
        {
            //arrenge
            InterceptorCounter messageHandler = new InterceptorCounter();
            FitbitClient sut = new FitbitClient(dummyCredentials, dummyToken, messageHandler);

            //Act
            HttpResponseMessage r = await sut.HttpClient.GetAsync("https://dev.fitbit.com/");

            //Assert
            Assert.AreEqual(1, messageHandler.RequestCount);
            Assert.AreEqual(1, messageHandler.ResponseCount);
        }

        [Test]
        [Category("Portable")]
        [Category("Interceptor")]
        public async Task CanInterceptHttpRequestAndFakeResponse()
        {
            const int EXPECT_ONE_REQUEST = 1;
            HttpResponseMessage fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unused);
            ResponseFaker responseFaker = new ResponseFaker(fakeResponse);

            //arrenge
            FitbitClient sut = new FitbitClient(dummyCredentials, dummyToken, responseFaker);

            //Act
            HttpResponseMessage actualResponse = await sut.HttpClient.GetAsync("https://dev.fitbit.com/");

            Assert.AreSame(fakeResponse, actualResponse);
            //Ensure that the response handler is still invoked, even though we short circuited the request
            Assert.AreEqual(EXPECT_ONE_REQUEST, responseFaker.ResponseCount);
        }


        [Test]
        [Category("Portable")]
        [Category("Interceptor")]
        [Category("OAuth2")]
        public async Task Correctly_Detects_Stale_Token_Refreshes_And_Retries_Original_Request()
        {
            OAuth2AccessToken originalToken = new OAuth2AccessToken() { Token = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE0MzAzNDM3MzUsInNjb3BlcyI6Indwcm8gd2xvYyB3bnV0IHdzbGUgd3NldCB3aHIgd3dlaSB3YWN0IHdzb2MiLCJzdWIiOiJBQkNERUYiLCJhdWQiOiJJSktMTU4iLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJpYXQiOjE0MzAzNDAxMzV9.z0VHrIEzjsBnjiNMBey6wtu26yHTnSWz_qlqoEpUlpc" };
            OAuth2AccessToken refreshedToken = new OAuth2AccessToken() { Token = "Refreshed" };

            //mocking our implementation of token manager. This test is concerned with ensuring the wiring is done correctly. Not the actual refresh process.
            Mock<ITokenManager> fakeManager = new Mock<ITokenManager>();
            fakeManager.Setup(m => m.RefreshTokenAsync(It.IsAny<FitbitClient>())).Returns(() => Task.Run(() => refreshedToken));

            //we shortcircuit the request to fake an expired token on the first request, and assuming the token is different the second time we let the request through
            StaleTokenFaker fakeServer = new StaleTokenFaker();

            FitbitClient sut = new FitbitClient(dummyCredentials, originalToken, fakeServer, /*Explicity activate autorefresh, default is true*/true, fakeManager.Object);

            //Act
            HttpResponseMessage actualResponse = await sut.HttpClient.GetAsync("https://dev.fitbit.com/");

            //Assert
            Assert.AreEqual(refreshedToken, sut.AccessToken);
            //Ensure the client is updated with the refreshed token
            Assert.AreEqual(refreshedToken.Token, sut.HttpClient.DefaultRequestHeaders.Authorization.Parameter);
            fakeManager.Verify(m => m.RefreshTokenAsync(It.IsAny<FitbitClient>()), Times.Once);
            //Expecte two interceptions. First when we get the 401 refresh, and second when we retry after refreshing the stale token
            Assert.AreEqual(2, fakeServer.requestCount, "It looks like either the client did not retry after the token was refreshed, or the stale token was not detected");

        }


        [Test]
        [Category("Portable")]
        [Category("Interceptor")]
        [Category("OAuth2")]
        public void Can_Handle_Failed_Refresh_Operation()
        {
            const int EXPECT_TWO_COUNT_STALE_AND_RETRY = 2;
            OAuth2AccessToken originalToken = new OAuth2AccessToken() { Token = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE0MzAzNDM3MzUsInNjb3BlcyI6Indwcm8gd2xvYyB3bnV0IHdzbGUgd3NldCB3aHIgd3dlaSB3YWN0IHdzb2MiLCJzdWIiOiJBQkNERUYiLCJhdWQiOiJJSktMTU4iLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJpYXQiOjE0MzAzNDAxMzV9.z0VHrIEzjsBnjiNMBey6wtu26yHTnSWz_qlqoEpUlpc" };
            OAuth2AccessToken refreshedToken = new OAuth2AccessToken() { Token = "Refreshed" };

            //mocking our implementation of token manager. This test is concerned with ensuring the wiring is done correctly. Not the actual refresh process.
            Mock<ITokenManager> fakeManager = new Mock<ITokenManager>();
            fakeManager.Setup(m => m.RefreshTokenAsync(It.IsAny<FitbitClient>())).Returns(() => Task.Run(() => refreshedToken));

            //simulate failed refresh token.
            StaleTokenFaker fakeServer = new StaleTokenFaker(10);

            FitbitClient sut = new FitbitClient(dummyCredentials, originalToken, fakeServer, fakeManager.Object);

            //Act
            Task<HttpResponseMessage> r = sut.HttpClient.GetAsync("https://dev.fitbit.com/");


            Assert.Throws<System.AggregateException>(() => r.Wait());
        }

        [Test]
        [Category("Portable")]
        [Category("Interceptor")]
        [Category("OAuth2")]
        public async Task Disable_Automatic_Token_Refresh()
        {
            OAuth2AccessToken originalToken = new OAuth2AccessToken() { Token = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE0MzAzNDM3MzUsInNjb3BlcyI6Indwcm8gd2xvYyB3bnV0IHdzbGUgd3NldCB3aHIgd3dlaSB3YWN0IHdzb2MiLCJzdWIiOiJBQkNERUYiLCJhdWQiOiJJSktMTU4iLCJpc3MiOiJGaXRiaXQiLCJ0eXAiOiJhY2Nlc3NfdG9rZW4iLCJpYXQiOjE0MzAzNDAxMzV9.z0VHrIEzjsBnjiNMBey6wtu26yHTnSWz_qlqoEpUlpc" };
            OAuth2AccessToken refreshedToken = new OAuth2AccessToken() { Token = "Refreshed" };

            //mocking our implementation of token manager. This test is concerned with ensuring the wiring is done correctly. Not the actual refresh process.
            Mock<ITokenManager> fakeManager = new Mock<ITokenManager>();
            fakeManager.Setup(m => m.RefreshTokenAsync(It.IsAny<FitbitClient>())).Returns(() => Task.Run(() => refreshedToken));

            //we shortcircuit the request to return a stale token and ensure that the client lets the stale token response through
            StaleTokenFaker fakeServer = new StaleTokenFaker();

            FitbitClient sut = new FitbitClient(dummyCredentials, originalToken, fakeServer, false, fakeManager.Object);

            //Act
            HttpResponseMessage actualResponse = await sut.HttpClient.GetAsync("https://dev.fitbit.com/");

            //Assert
            Assert.AreEqual(fakeServer.staleTokenresponse, actualResponse);
        }

        public class StaleTokenFaker : IFitbitInterceptor
        {
            public HttpResponseMessage staleTokenresponse;
            public int requestCount = 0;
            private int desiredStaleTokenReplies;

            public StaleTokenFaker(int desiredStaleTokenReplyLimitCount = 1)
            {
                desiredStaleTokenReplies = desiredStaleTokenReplyLimitCount;
                //HttpContent staleTokenError = new HttpContent();

                staleTokenresponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(SampleDataHelper.GetContent("ApiError-Request-StaleToken.json"))
                };
            }

            public Task<HttpResponseMessage> InterceptRequest(HttpRequestMessage request, CancellationToken cancellationToken, FitbitClient client)
            {
                requestCount++;
                if (requestCount <= desiredStaleTokenReplies)
                    return Task.Run(() => staleTokenresponse);
                else
                    return null;
            }

            public async Task<HttpResponseMessage> InterceptResponse(Task<HttpResponseMessage> response, CancellationToken cancellationToken, FitbitClient client)
            {
                //let the pipeline continue
                return null;
            }
        }
    }
}
