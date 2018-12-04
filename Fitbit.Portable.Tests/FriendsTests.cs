namespace Fitbit.Portable.Tests
{
    using System;
    using System.Collections.Generic;
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
    public class FriendsTests
    {
        [Test]
        [Category("Portable")]
        public async Task GetFriendsMultipleAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetFriends-Multiple.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/friends.json", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            List<UserProfile> response = await fitbitClient.GetFriendsAsync();

            Assert.AreEqual(3, response.Count);
            ValidateMultipleFriends(response);
        }

        [Test]
        [Category("Portable")]
        public async Task GetFriendsSingleAsync_Success()
        {
            string content = SampleDataHelper.GetContent("GetFriends-Single.json");

            Func<HttpResponseMessage> responseMessage = new Func<HttpResponseMessage>(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) };
            });

            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
                Assert.AreEqual("https://api.fitbit.com/1/user/-/friends.json", message.RequestUri.AbsoluteUri);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            List<UserProfile> response = await fitbitClient.GetFriendsAsync();

            ValidateSingleFriend(response);
        }

        [Test]
        [Category("Portable")]
        public void GetFriendsAsync_Failure_Errors()
        {
            Func<HttpResponseMessage> responseMessage = Helper.CreateErrorResponse();
            Action<HttpRequestMessage, CancellationToken> verification = new Action<HttpRequestMessage, CancellationToken>((message, token) =>
            {
                Assert.AreEqual(HttpMethod.Get, message.Method);
            });

            FitbitClient fitbitClient = Helper.CreateFitbitClient(responseMessage, verification);

            Func<Task<List<UserProfile>>> result = () => fitbitClient.GetFriendsAsync();

            result.ShouldThrow<FitbitException>();
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_Empty_String()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();
            Assert.That(
                new TestDelegate(() => deserializer.GetFriends(string.Empty)),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_Null_String()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            Assert.That(
                new TestDelegate(() => deserializer.GetFriends(null)),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Throws_Exception_With_WhiteSpace()
        {
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();
            Assert.That(
                new TestDelegate(() => deserializer.GetFriends("         ")),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_Friends_Multiple()
        {
            string content = SampleDataHelper.GetContent("GetFriends-Multiple.json");
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            List<UserProfile> friends = deserializer.GetFriends(content);

            Assert.IsNotNull(friends);
            Assert.IsTrue(friends.Count == 3);

            ValidateMultipleFriends(friends);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_Friends_Single()
        {
            string content = SampleDataHelper.GetContent("GetFriends-Single.json");
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            List<UserProfile> friends = deserializer.GetFriends(content);

            Assert.IsNotNull(friends);
            ValidateSingleFriend(friends);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_Friends_Single_NullDateOfBirth()
        {
            string content = SampleDataHelper.GetContent("GetFriends-Single-2.json");
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            List<UserProfile> friends = deserializer.GetFriends(content);

            Assert.IsNotNull(friends);
            ValidateSingleFriendNullDateOfBirth(friends);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_Friends_None()
        {
            string content = SampleDataHelper.GetContent("GetFriends-None.json");
            JsonDotNetSerializer deserializer = new JsonDotNetSerializer();

            List<UserProfile> friends = deserializer.GetFriends(content);

            Assert.IsNotNull(friends);
            Assert.IsTrue(friends.Count == 0);
        }

        private void ValidateMultipleFriends(List<UserProfile> friends)
        {
            UserProfile friend = friends.First();

            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_100_male.gif", friend.Avatar);
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_150_male.gif", friend.Avatar150);
            Assert.AreEqual("GB", friend.Country);
            Assert.AreEqual(DateTime.Parse("1970-01-01"), friend.DateOfBirth);
            Assert.AreEqual("Andy", friend.DisplayName);
            Assert.AreEqual("242XXX", friend.EncodedId);
            Assert.AreEqual("Andy Fullname", friend.FullName);
            Assert.AreEqual(Gender.MALE, friend.Gender);
            Assert.AreEqual(155, friend.Height);
            Assert.AreEqual("en_GB", friend.Locale);
            Assert.AreEqual(DateTime.Parse("2010-12-25"), friend.MemberSince);
            Assert.AreEqual("", friend.Nickname);
            Assert.AreEqual(3600000, friend.OffsetFromUTCMillis);
            Assert.AreEqual("SUNDAY", friend.StartDayOfWeek);
            Assert.AreEqual(0, friend.StrideLengthRunning);
            Assert.AreEqual(0, friend.StrideLengthWalking);
            Assert.AreEqual("Europe/London", friend.Timezone);
            Assert.AreEqual(84.7, friend.Weight);

            friend = friends.FirstOrDefault(x => x.Gender == Gender.FEMALE);

            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_100_female.gif", friend.Avatar);
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_150_female.gif", friend.Avatar150);
            Assert.AreEqual("GB", friend.Country);
            Assert.AreEqual(DateTime.Parse("1984-09-07"), friend.DateOfBirth);
            Assert.AreEqual("Laura", friend.DisplayName);
            Assert.AreEqual("24WXXX", friend.EncodedId);
            Assert.AreEqual("", friend.FullName);
            Assert.AreEqual(Gender.FEMALE, friend.Gender);
            Assert.AreEqual(165, friend.Height);
            Assert.AreEqual("en_GB", friend.Locale);
            Assert.AreEqual(DateTime.Parse("2013-02-01"), friend.MemberSince);
            Assert.AreEqual("", friend.Nickname);
            Assert.AreEqual(3600000, friend.OffsetFromUTCMillis);
            Assert.AreEqual("SUNDAY", friend.StartDayOfWeek);
            Assert.AreEqual(0, friend.StrideLengthRunning);
            Assert.AreEqual(0, friend.StrideLengthWalking);
            Assert.AreEqual("Europe/London", friend.Timezone);
            Assert.AreEqual(0, friend.Weight);

            friend = friends.Last();

            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_100_male.gif", friend.Avatar);
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_150_male.gif", friend.Avatar150);
            Assert.AreEqual("GB", friend.Country);
            Assert.AreEqual(DateTime.Parse("1978-09-24"), friend.DateOfBirth);
            Assert.AreEqual("Michael", friend.DisplayName);
            Assert.AreEqual("24NXXX", friend.EncodedId);
            Assert.AreEqual("", friend.FullName);
            Assert.AreEqual(Gender.MALE, friend.Gender);
            Assert.AreEqual(190.5, friend.Height);
            Assert.AreEqual("en_GB", friend.Locale);
            Assert.AreEqual(DateTime.Parse("2013-01-01"), friend.MemberSince);
            Assert.AreEqual("", friend.Nickname);
            Assert.AreEqual(3600000, friend.OffsetFromUTCMillis);
            Assert.AreEqual("SUNDAY", friend.StartDayOfWeek);
            Assert.AreEqual(0, friend.StrideLengthRunning);
            Assert.AreEqual(0, friend.StrideLengthWalking);
            Assert.AreEqual("Europe/London", friend.Timezone);
            Assert.AreEqual(0, friend.Weight);
        }

        private void ValidateSingleFriend(List<UserProfile> friends)
        {
            Assert.IsTrue(friends.Count == 1);

            UserProfile friend = friends.First();
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_100_female.gif", friend.Avatar);
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_150_female.gif", friend.Avatar150);
            Assert.AreEqual("GB", friend.Country);
            Assert.AreEqual(DateTime.Parse("1984-09-07"), friend.DateOfBirth);
            Assert.AreEqual("Laura", friend.DisplayName);
            Assert.AreEqual("24WXXX", friend.EncodedId);
            Assert.AreEqual("", friend.FullName);
            Assert.AreEqual(Gender.FEMALE, friend.Gender);
            Assert.AreEqual(165, friend.Height);
            Assert.AreEqual("en_GB", friend.Locale);
            Assert.AreEqual(DateTime.Parse("2013-02-01"), friend.MemberSince);
            Assert.AreEqual("", friend.Nickname);
            Assert.AreEqual(3600000, friend.OffsetFromUTCMillis);
            Assert.AreEqual("SUNDAY", friend.StartDayOfWeek);
            Assert.AreEqual(0, friend.StrideLengthRunning);
            Assert.AreEqual(0, friend.StrideLengthWalking);
            Assert.AreEqual("Europe/London", friend.Timezone);
            Assert.AreEqual(0, friend.Weight);
        }

        private void ValidateSingleFriendNullDateOfBirth(List<UserProfile> friends)
        {
            Assert.IsTrue(friends.Count == 1);

            UserProfile friend = friends.First();
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_100_female.gif", friend.Avatar);
            Assert.AreEqual("http://www.fitbit.com/images/profile/defaultProfile_150_female.gif", friend.Avatar150);
            Assert.AreEqual("GB", friend.Country);
            Assert.AreEqual(DateTime.MinValue, friend.DateOfBirth);
            Assert.AreEqual("Laura", friend.DisplayName);
            Assert.AreEqual("24WXXX", friend.EncodedId);
            Assert.AreEqual("", friend.FullName);
            Assert.AreEqual(Gender.FEMALE, friend.Gender);
            Assert.AreEqual(165, friend.Height);
            Assert.AreEqual("en_GB", friend.Locale);
            Assert.AreEqual(DateTime.Parse("2013-02-01"), friend.MemberSince);
            Assert.AreEqual("", friend.Nickname);
            Assert.AreEqual(3600000, friend.OffsetFromUTCMillis);
            Assert.AreEqual("SUNDAY", friend.StartDayOfWeek);
            Assert.AreEqual(0, friend.StrideLengthRunning);
            Assert.AreEqual(0, friend.StrideLengthWalking);
            Assert.AreEqual("Europe/London", friend.Timezone);
            Assert.AreEqual(0, friend.Weight);
        }
    }
}