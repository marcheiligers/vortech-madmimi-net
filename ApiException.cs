/// <summary>
/// Represents an API exception
/// by Marc Heiligers (marc@madmimi.com)
/// </summary>
using System;
using System.Net;

namespace Vortech.MadMimi
{

	public class ApiException : Exception
	{
		HttpStatusCode statusCode = HttpStatusCode.Unused;
		
		public ApiException(string message, HttpStatusCode statusCode)
			: base(message)
		{
			this.statusCode = statusCode;
		}
		
		public ApiException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ApiException(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}
		
		public ApiException(ApiResult result) 
			: base(result.ResultBody, result.Exception)
		{
			statusCode = result.StatusCode;
		}
		
		public HttpStatusCode StatusCode {
			get {
				return statusCode;
			}
		}
	}
}
