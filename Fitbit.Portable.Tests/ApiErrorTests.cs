﻿namespace Fitbit.Portable.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Fitbit.Api.Portable;
    using Fitbit.Models;

    using NUnit.Framework;

    [TestFixture]
    public class ApiErrorTests
    {
        [Test] [Category("Portable")]
        public void Can_Deserialize_ApiError()
        {
            string content = SampleDataHelper.GetContent("ApiError.json");

            List<ApiError> result = new JsonDotNetSerializer().ParseErrors(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            ApiError error = result.First();
            Assert.AreEqual("request", error.ErrorType);
            Assert.AreEqual("n/a", error.FieldName);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_ApiError_BadRequest()
        {
            string content = SampleDataHelper.GetContent("ApiError-Request-BadRequest.json");

            List<ApiError> result = new JsonDotNetSerializer().ParseErrors(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            ApiError error = result.First();
            Assert.AreEqual("invalid_request", error.ErrorType);
            Assert.AreEqual("n/a", error.FieldName);
            Assert.AreEqual("There was an error reading the request body.", error.Message);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_ApiError_Forbidden()
        {
            string content = SampleDataHelper.GetContent("ApiError-Request-Forbidden.json");

            List<ApiError> result = new JsonDotNetSerializer().ParseErrors(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            ApiError error = result.First();
            Assert.AreEqual("insufficient_permissions", error.ErrorType);
            Assert.AreEqual(null, error.FieldName);
            Assert.AreEqual("Read-only API client is not authorized to update resources.", error.Message);
        }

        [Test]
        [Category("Portable")]
        public void Can_Deserialize_ApiError_Unauthorized()
        {
            string content = SampleDataHelper.GetContent("ApiError-Request-Unauthorized.json");

            List<ApiError> result = new JsonDotNetSerializer().ParseErrors(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            ApiError error = result.First();
            Assert.AreEqual("invalid_request", error.ErrorType);
            Assert.AreEqual(null, error.FieldName);
            Assert.AreEqual("Authorization header required.", error.Message);
        }
    }
}