namespace Fitbit.Api.Portable
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Fitbit.Models;

    public class FitbitException : Exception
    {
        public List<ApiError> ApiErrors { get; set; }

        public FitbitException(string message, IEnumerable<ApiError> errors, Exception innerEx = null) : base(message, innerEx)
        {
            ApiErrors = errors != null ? new List<ApiError>(errors) : new List<ApiError>();
        }
    }
}
