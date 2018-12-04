namespace Fitbit.Portable.Tests
{
    using System;

    using Fitbit.Api.Portable;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class JsonDotNetSerializerTests
    {
        public class TestClass
        {
            [JsonProperty("testproperty")]
            public string TestProperty { get; set; }

            [JsonProperty("mydate")]
            public DateTime MyDate { get; set; }


            // todo: array etc.
        }

        [Test] [Category("Portable")]
        public void DefaultValueCreated_String()
        {
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            TestClass defaultValue = serializer.Deserialize<TestClass>(string.Empty);
            Assert.AreEqual(default(TestClass), defaultValue);
        }

        [Test] [Category("Portable")]
        public void DefaultValueCreated_JToken()
        {
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            TestClass defaultValue = serializer.Deserialize<TestClass>((JToken)null);
            Assert.AreEqual(default(TestClass), defaultValue);
        }

        [Test] [Category("Portable")]
        public void NoRootValueCreated()
        {
            string data = "{\"testproperty\" : \"bob\" }";
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            TestClass value = serializer.Deserialize<TestClass>(data);
            Assert.IsNotNull(value);
            Assert.AreEqual("bob", value.TestProperty);
        }

        [Test] [Category("Portable")]
        public void RootPropertyValueCreated()
        {
            string data = "{\"testclass\" : {\"testproperty\" : \"bob\" } }";
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            serializer.RootProperty = "testclass";
            TestClass value = serializer.Deserialize<TestClass>(data);
            Assert.IsNotNull(value);
            Assert.AreEqual("bob", value.TestProperty);
        }

        [Test]  [Category("Portable")]
        public void DateParsingSuccess()
        {
            string data = "{\"mydate\" : \"1970-01-01\" }";
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            TestClass value = serializer.Deserialize<TestClass>(data);
            Assert.IsNotNull(value);
            Assert.AreEqual(new DateTime(1970, 1, 1), value.MyDate);
        }

        [Test]  [Category("Portable")]
        public void DateParsingEmptySuccess()
        {
            string data = "{\"mydate\" : \"\" }";
            JsonDotNetSerializer serializer = new JsonDotNetSerializer();
            TestClass value = serializer.Deserialize<TestClass>(data);
            Assert.IsNotNull(value);
            Assert.AreEqual(DateTime.MinValue, value.MyDate);
        }
    }
}
