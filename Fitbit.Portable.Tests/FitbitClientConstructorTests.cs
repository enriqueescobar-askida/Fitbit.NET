namespace Fitbit.Portable.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Fitbit.Api.Portable;
    using Fitbit.Api.Portable.OAuth2;

    using NUnit.Framework;

    [TestFixture]
    public class FitbitClientConstructorTests
    {
        [Test]
        [Category("constructor")]
        public void Most_Basic()
        {
            FitbitAppCredentials credentials = new FitbitAppCredentials() { ClientId = "SomeID", ClientSecret = "THE Secret ;)" };
            OAuth2AccessToken accessToken = new OAuth2AccessToken() { Token = "", TokenType = "", ExpiresIn = 3600, RefreshToken = ""};

            FitbitClient sut = new FitbitClient(credentials, accessToken);

            Assert.IsNotNull(sut.HttpClient);
        }

        [Test]
        [Category("constructor")]
        public void Use_Custom_HttpClient_Factory()
        {
            FitbitClient sut = new FitbitClient(mh => { return new HttpClient(); });

            Assert.IsNotNull(sut.HttpClient);
        }

        [Test]
        [Category("constructor")]
        public void Can_Instantiate_Without_Any_Interceptors()
        {
            FitbitAppCredentials credentials = new FitbitAppCredentials() { ClientId = "SomeID", ClientSecret = "THE Secret ;)" };
            OAuth2AccessToken accessToken = new OAuth2AccessToken() { Token = "", TokenType = "", ExpiresIn = 3600, RefreshToken = "" };

            //Ensure not even the auto-token-refresh interceptor is active
            FitbitClient sut = new FitbitClient(credentials, accessToken, false);

            Assert.IsNotNull(sut.HttpClient);
        }

        [Test]
        [Category("constructor")]
        public void Can_Use_Interceptors_Without_Autorefresh()
        {
            FitbitAppCredentials credentials = new FitbitAppCredentials() { ClientId = "SomeID", ClientSecret = "THE Secret ;)" };
            OAuth2AccessToken accessToken = new OAuth2AccessToken() { Token = "", TokenType = "", ExpiresIn = 3600, RefreshToken = "" };

            //Registere an interceptor, but disable the auto-token-refresh interceptor
            FitbitClient sut = new FitbitClient(credentials, accessToken, new InterceptorCounter(), false);

            Assert.IsNotNull(sut.HttpClient);
        }
    }
}
