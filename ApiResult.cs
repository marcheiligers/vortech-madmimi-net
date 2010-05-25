/// <summary>
/// Represents the result of an API call
/// by Marc Heiligers (marc@madmimi.com)
/// </summary>
using System;
using System.Net;
using System.IO;

namespace Vortech.MadMimi
{
	public class ApiResult
	{
		HttpStatusCode statusCode = HttpStatusCode.Unused;
		string resultBody = null;
		Exception exception = null;
		
		public ApiResult(HttpWebResponse response) {
			statusCode = response.StatusCode;
			using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
				resultBody = reader.ReadToEnd();
			}
		}
		
		public ApiResult(Exception exception) {
			this.exception = exception;
		}
		
		public bool IsSuccess {
	        get {
	            return !IsError;
	        }
		}
		
		public bool IsError {
			get {
				return (statusCode != HttpStatusCode.OK || exception != null);
			}
		}
		
		public bool IsException {
			get {
				return exception != null;
			}
		}
		
		public HttpStatusCode StatusCode {
			get {
				return statusCode;
			}
		}
		
		public string ResultBody {
			get {
				if(resultBody == null && exception != null) {
					return exception.Message;
				} else {
					return resultBody;
				}
			}
		}
		
		public Exception Exception {
			get {
				return exception;
			}
		}
	}
}
