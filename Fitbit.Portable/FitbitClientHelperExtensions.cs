﻿using System;
using System.Collections.Generic;
using Fitbit.Models;

namespace Fitbit.Api.Portable
{
    internal static class FitbitClientHelperExtensions
    {
        /// <summary>
        /// Converts the REST api resource into the fully qualified url
        /// </summary>
        /// <param name="apiCall"></param>
        /// <param name="encodedUserId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static string ToFullUrl(string apiCall, string encodedUserId = default(string), params object[] args)
        {
            string userSignifier = "-"; //used for current user
            if (!string.IsNullOrWhiteSpace(encodedUserId))
            {
                userSignifier = encodedUserId;    
            }
            
            var parameters = new List<object> {userSignifier};
            if (args != null)
            {
                parameters.AddRange(args);
            }

            apiCall = string.Format(apiCall, parameters.ToArray());

            if (apiCall.StartsWith("/"))
            {
                apiCall = apiCall.TrimStart(new[] { '/' });
            }
            return Constants.BaseApiUrl + apiCall;
        }

        /// <summary>
        /// Converts the timeseries resource into the property to acces the data
        /// </summary>
        /// <param name="timeseriesResource"></param>
        /// <returns></returns>
        internal static string ToTimeSeriesProperty(this IntradayResourceType timeseriesResource)
        {
            var timeSeriesResourceDisplay = timeseriesResource.GetStringValue();
            if (timeSeriesResourceDisplay.StartsWith("/"))
            {
                timeSeriesResourceDisplay = timeSeriesResourceDisplay.TrimStart(new[] { '/' });
            }

            return timeSeriesResourceDisplay.Replace("/", "-");
        }

        /// <summary>
        /// Converts the timeseries resource into the property to acces the data
        /// </summary>
        /// <param name="timeseriesResource"></param>
        /// <returns></returns>
        internal static string ToTimeSeriesProperty(this TimeSeriesResourceType timeseriesResource)
        {
            var timeSeriesResourceDisplay = timeseriesResource.GetStringValue();
            if (timeSeriesResourceDisplay.StartsWith("/"))
            {
                timeSeriesResourceDisplay = timeSeriesResourceDisplay.TrimStart(new[] { '/' });
            }

            return timeSeriesResourceDisplay.Replace("/", "-");
        }

        /// <summary>
        /// Converts the specified datetime value into the required format for calling the Fitbit API (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static string ToFitbitFormat(this DateTime dateTime)
        {
            const string DateFormat = "yyyy-MM-dd";
            return dateTime.ToString(DateFormat);
        }

        /// <summary>
        /// Returns the elapsed seconds throughout a day; 00:00:00 to 23:59:59
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static int ToElapsedSeconds(this DateTime dateTime)
        {
            int seconds = dateTime.Second;
            seconds += (dateTime.Minute*60); // seconds in minutes
            seconds += (dateTime.Hour*60*60); // seconds in hours
            return seconds;
        }
    }
}