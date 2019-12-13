using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DataReef.TM.Api.Classes
{
	public static class HttpResponseMessageExtensions
	{
		private static class Defaults
		{
			public const string CustomHeaderValue = "Unspecified header value";

			public const string WarningAgent = "Warning";

			public const string WarningText = "Unspecified warning";

			public static class HeaderNames
			{
				public const string XDebugInfoHeader = "X-DebugInfo";

				public const string XDtvDebugInfoHeader = "X-DTV-DebugInfo";
			}
		}

		public static void AddWarningHeader(this HttpResponseMessage response, WarningCode warningCode, string warningAgent, string warningText)
		{
			if ((response != null) && (response.Headers != null) && (response.Headers.Warning != null))
			{
				if (!Enum.IsDefined(typeof(WarningCode), warningCode))
				{
					warningCode = WarningCode.MiscellaneousPersistentWarning;
				}

				if (string.IsNullOrWhiteSpace(warningAgent))
				{
					warningAgent = Defaults.WarningAgent;
				}

				if (string.IsNullOrWhiteSpace(warningText))
				{
					warningText = Defaults.WarningAgent;
				}

				warningText = HttpResponseMessageExtensions.SafeHeaderValue(warningText);

				response.Headers.Warning.Add(new System.Net.Http.Headers.WarningHeaderValue(
					code: (int)warningCode,
					agent: warningAgent,
					text: "\"" + warningText + "\""
				));
			}
		}

		public static void AddDebugInfoHeader(this HttpResponseMessage response, Exception ex)
		{
			HttpResponseMessageExtensions.AddCustomHeader(response: response, headerName: Defaults.HeaderNames.XDebugInfoHeader, ex: ex);
		}

		public static void AddDtvDebugInfoHeader(this HttpResponseMessage response, string errorDetails)
		{
			HttpResponseMessageExtensions.AddCustomHeader(response: response, headerName: Defaults.HeaderNames.XDtvDebugInfoHeader, headerValue: errorDetails);
		}

		public static void AddDtvDebugInfoHeader(this HttpResponseMessage response, Exception ex)
		{
			HttpResponseMessageExtensions.AddCustomHeader(response: response, headerName: Defaults.HeaderNames.XDtvDebugInfoHeader, ex: ex);
		}

		public static void AddCustomHeader(this HttpResponseMessage response, string headerName, Exception ex)
		{
			if (ex != null)
			{
				string safeErrorMessage = HttpResponseMessageExtensions.SafeHeaderValue(ex.Message);

				string safeStackTrace = HttpResponseMessageExtensions.SafeHeaderValue(ex.StackTrace);

				string safeErrorDetails = string.Format(
					"Exception-type: {0}; Error-message: {1}; Stack-trace: {2}",
					ex.GetType(),
					safeErrorMessage,
					safeStackTrace);

				HttpResponseMessageExtensions.AddCustomHeader(response: response, headerName: headerName, headerValue: safeErrorDetails);
			}
		}

		public static void AddCustomHeader(this HttpResponseMessage response, string headerName, string headerValue)
		{
			if ((response != null) && (response.Headers != null) && (!string.IsNullOrWhiteSpace(headerName)) && (!string.IsNullOrWhiteSpace(headerValue)))
			{
				// strip off invalid characters
				string safeHeaderValue = HttpResponseMessageExtensions.SafeHeaderValue(headerValue);

				response.Headers.Add(headerName, safeHeaderValue);
			}
		}

		private static string SafeHeaderValue(string originalValue)
		{
			if (!string.IsNullOrWhiteSpace(originalValue))
			{
				return originalValue.Replace("\"", "`")
									.Replace("'", "`")
									.Replace(Environment.NewLine, " | ");
			}

			return originalValue;
		}
	}

	/// <summary>
	/// Warning codes, see http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
	/// </summary>
	public enum WarningCode
	{
		/// <summary>
		/// Warning code 110: response is stale. MUST be included whenever the returned response is stale.
		/// </summary>
		ResponseIsStale = 110,

		/// <summary>
		/// Warning code 111: revalidation failed. MUST be included if a cache returns a stale response because an attempt to revalidate the response failed, due to an inability to reach the server.
		/// </summary>
		RevalidationFailed = 111,

		/// <summary>
		/// Warning code 112: disconnected operation. SHOULD be included if the cache is intentionally disconnected from the rest of the network for a period of time.
		/// </summary>
		DisconnectedOperation = 112,

		/// <summary>
		/// Warning code 113: heuristic expiration. MUST be included if the cache heuristically chose a freshness lifetime greater than 24 hours and the response's age is greater than 24 hours.
		/// </summary>
		HeuristicExpiration = 113,

		/// <summary>
		/// Warning code 199: miscellaneous warning. The warning text MAY include arbitrary information to be presented to a human user, or logged. A system receiving this warning MUST NOT take any automated action, besides presenting the warning to the user.
		/// </summary>
		MiscellaneousWarning = 199,

		/// <summary>
		/// Warning code 214: transformation applied. MUST be added by an intermediate cache or proxy if it applies any transformation changing the content-coding (as specified in the Content-Encoding header) or media-type (as specified in the Content-Type header) of the response, or the entity-body of the response, unless this Warning code already appears in the response.
		/// </summary>
		TransformationApplied = 214,

		/// <summary>
		/// Warning code 299: miscellaneous persistent warning. The warning text MAY include arbitrary information to be presented to a human user, or logged. A system receiving this warning MUST NOT take any automated action.
		/// </summary>
		MiscellaneousPersistentWarning = 299
	}
}