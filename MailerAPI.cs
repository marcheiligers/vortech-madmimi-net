/*=====================================================================================================
 * Class:   Vortech.MadMimi.MailerAPI
 * Author:  Joshua Jackson <jjackson@vortech.net> http://www.vortech.net
 * Date:    April 17, 2010
 * Purpose: Implementation of MadMimi's mailer API
 * 
 * URL: http://developer.madmimi.com/developer/api
 =====================================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Web;
using System.IO;
using System.Data;

namespace Vortech.MadMimi {

    public enum MadMimiMailStatus {
        ignorant,
        sending,
        failed,
        sent,
        received,
        clicked_through,
        bounced,
        retried,
        retry_failed,
        abused
    }
    
    public class MailerAPI {
        public string Username { get; set; }
        public string APIKey { get; set; }
		public bool ThrowExceptions { get; set; }
		
		
		private string MAD_MIMI_MAILER_API_URL = "https://api.madmimi.com/mailer";
		private string MAD_MIMI_LIST_API_URL = "http://api.madmimi.com";
		private bool debug = false;
		private ApiResult lastResult;

        /// <summary>
        /// Initializes a MadMimi Mailer API object
        /// </summary>
        /// <param name="username">MadMimi Username</param>
        /// <param name="apiKey">MadMimi API Key</param>
        public MailerAPI(string username, string apiKey) {
            Username = username;
            APIKey = apiKey;
        }
		
        public MailerAPI(string username, string apiKey, bool throwExceptions) {
            Username = username;
            APIKey = apiKey;
            ThrowExceptions = throwExceptions;
        }
		
		public void SetDebugMode() {
			debug = true;
		}

		public void SetDebugMode(bool useLocalhost) {
			if(useLocalhost) {
				MAD_MIMI_MAILER_API_URL = "http://localhost:3000/mailer";
				MAD_MIMI_LIST_API_URL = "http://localhost:3000";
			}
			debug = true;
		}
		
        private string GetMadMimiMailerURL(string purl) {
            return String.Format("{0}{1}", MAD_MIMI_MAILER_API_URL, purl);
        }

        private string GetMadMimiURL(string purl) {
            return String.Format("{0}{1}", MAD_MIMI_LIST_API_URL, purl);
        }
        
        private ApiResult PostToMadMimi(string url, string data, bool useMailer) {
            try {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(
                    (useMailer) ? GetMadMimiMailerURL(url) : GetMadMimiURL(url)
                );
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                string reqStr = String.Format("username={0}&api_key={1}&{2}", Username, APIKey, data);
                req.ContentLength = reqStr.Length;
				
				if(debug) {
					Console.WriteLine(reqStr);
				}

                using(StreamWriter reqStream = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII)) {
                    reqStream.Write(reqStr);
                    reqStream.Close();
                }
                
                lastResult = new ApiResult((HttpWebResponse) req.GetResponse());
            } catch (Exception ex) {
                lastResult = new ApiResult(ex);
                if (ThrowExceptions) {
                    throw new ApiException(ex);
                }
            }
            if(lastResult.IsError && ThrowExceptions) {
            		throw new ApiException(lastResult);
			}
			return lastResult;
        }

        private ApiResult PostToMadMimi(string url, string data) {
            return PostToMadMimi(url, data, false);
        }

        private ApiResult GetFromMadMimi(string url, bool useMailer) {
            try {
                string reqStr = String.Format("{0}&username={1}&api_key={2}", url, Username, APIKey);
                                
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                    (useMailer) ? GetMadMimiMailerURL(reqStr) : GetMadMimiURL(reqStr)
                );
                req.Method = "GET";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = 0;
                StreamWriter reqStream = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                reqStream.Close();
                
                lastResult = new ApiResult((HttpWebResponse) req.GetResponse());
            } catch(Exception ex) {
                lastResult = new ApiResult(ex);
                if (ThrowExceptions) {
                    throw new ApiException(ex);
                }
            }
            if(lastResult.IsError && ThrowExceptions) {
            		throw new ApiException(lastResult);
			}
			return lastResult;
        }

        /// <summary>
        /// Converts a .Net DateTime to a Unix timestamp
        /// </summary>
        /// <param name="aDate">DateTime to convert</param>
        /// <returns>Number of seconds since 1/1/1970</returns>
        private int DateTimeToUnixTime(DateTime aDate) {
            DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = aDate - EPOCH;
            return (int)Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// Adds a new audience list
        /// </summary>
        /// <param name="listName">Name of list to create</param>
        /// <returns>True if an OK status is returned</returns>
        public ApiResult AudienceListAdd(string listName) {
            string reqStr = String.Format("name={0}", HttpUtility.UrlEncode(listName));
            return PostToMadMimi("/audience_lists", reqStr);
        }

        /// <summary>
        /// Deletes an existing audience list
        /// </summary>
        /// <param name="listName">Name of the list to delete</param>
        /// <returns>True if an OK status is returned</returns>
        public ApiResult AudienceListDelete(string listName) {
            return PostToMadMimi(String.Format("/audience_lists/{0}", HttpUtility.UrlEncode(listName)), "_method=delete");
        }

        /// <summary>
        /// Search for audience list members that match the given criterea
        /// </summary>
        /// <param name="queryStr">Search parameteres</param>
        /// <returns>Up to 100 matching audience members</returns>
        //TODO: return AudienceMemberCollection
        public XmlDocument AudienceListSearch(string queryStr) {
            string url = GetMadMimiURL("/audience_members/search.xml");
            XmlDocument ret = new XmlDocument();
            ret.Load(String.Format("{0}?username={1}&api_key={2}&query={3}", url, HttpUtility.UrlEncode(Username), APIKey, HttpUtility.UrlEncode(queryStr)));
            return ret;
        }

        /// <summary>
        /// Gets a list of current audience lists
        /// </summary>
        /// <returns>XML document containing existing audience list information</returns>
        //TODO: return a AudienceListCollection
        public XmlDocument AudienceLists() {
            string url  = GetMadMimiURL("/audience_lists/lists.xml");
            XmlDocument ret = new XmlDocument();
            ret.Load(String.Format("{0}?username={1}&api_key={2}", url, HttpUtility.UrlEncode(Username), APIKey));
            return ret;
        }

        /// <summary>
        /// Imports a list of accounts into an audience list
        /// </summary>
        /// <param name="emailList">List of email addresses to import</param>
        /// <param name="listName">Name of audience list</param>
        /// <returns>True if a status code of OK is returned</returns>
        public ApiResult AudienceImport(List<String> emailList, string listName) {

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("email", typeof(string));
            foreach (string s in emailList) {
                DataRow dr = dt.NewRow();
                dr[0] = s;
                dt.Rows.Add(dr);
            }
            ds.Tables.Add(dt);

            return AudienceImport(ds, listName);
        }

        /// <summary>
        /// Imports a list of accounts into an audience list
        /// </summary>
        /// <param name="userInfo">Dataset of accounts to import</param>
        /// <param name="listName">Name of audience list</param>
        /// <returns>True if a status code of OK is returned</returns>
        public ApiResult AudienceImport(DataSet userInfo, string listName) {
            if (userInfo.Tables.Count < 1) {
                return new ApiResult(new ArgumentException("No tables in userInfo"));
            }

            if (userInfo.Tables[0].Rows.Count < 1) { //Marc says: made this 1. TODO: validate that email is a column
                return new ApiResult(new ArgumentException("No columns in zeroth table in userInfo"));
            }
			
			return AudienceImport(userInfo.Tables[0], listName);
        }
		
		/// <summary>
		/// Imports a data table of audience members into Mimi. By Marc Heiligers (marc@madmimi.com)
		/// </summary>
		/// <param name="audienceMembers">
		/// The datatable containing the audience member information (note: there must be an 'email' column) <see cref="DataTable"/>
		/// </param>
		/// <returns>
		/// True is successful, false otherwise <see cref="System.Boolean"/>
		/// </returns>
		public ApiResult AudienceImport(DataTable audienceMembers) {
			return AudienceImport(audienceMembers, null);
		}
		
		/// <summary>
		/// Imports a data table of audience members into Mimi. By Marc Heiligers (marc@madmimi.com)
		/// </summary>
		/// <param name="audienceMembers">
		/// The datatable containing the audience member information (note: there must be an 'email' column) <see cref="DataTable"/>
		/// </param>
		/// <param name="listName">
		/// An optional name of a list that the audience members should be imported into <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// True is successful, false otherwise <see cref="System.Boolean"/>
		/// </returns>
		public ApiResult AudienceImport(DataTable audienceMembers, string listName) {
		    try {
                StringBuilder reqData = new StringBuilder();

                string[] cols = new string[audienceMembers.Columns.Count + (listName == null ? 0 : 1)];
                int idx = 0;
                foreach (DataColumn dc in audienceMembers.Columns) {
                    cols[idx] = FormatCsvValue(dc.ColumnName);
                    idx++;
                }
	    			if(listName != null) {
	    				cols[idx] = "add_list"; //Special Mimi Column to add imported members to a list
	    			}
                reqData.AppendLine(String.Join(",", cols));

                string[] rowData = new string[audienceMembers.Columns.Count + (listName == null ? 0 : 1)];
                foreach (DataRow dr in audienceMembers.Rows) {
                    for (idx = 0; idx < audienceMembers.Columns.Count; idx++) {
                        rowData[idx] = FormatCsvValue(dr[idx].ToString());
                    }
    				if(listName != null) {
    					rowData[idx] = FormatCsvValue(listName); //The list name to add to if supplied
    				}
                    reqData.AppendLine(String.Join(",", rowData));
                }

                string data = String.Format("csv_file={0}", HttpUtility.UrlEncode(reqData.ToString()));
                PostToMadMimi("/audience_members", data);
            } catch(Exception ex) {
                lastResult = new ApiResult(ex);
                if (ThrowExceptions) {
                    throw new ApiException(ex);
                }
            }
            if(lastResult.IsError && ThrowExceptions) {
            		throw new ApiException(lastResult);
			}
			return lastResult;
		}
		
		/// <summary>
		/// This function comes from the closed source library ETechnik.Utility2. 
		/// As a Director of E-Technik, and the original author of the library I am 
		/// giving permission to use this function in this library (marc@e-technik.com).  
		/// </summary>
		/// <param name="value">
		/// the value to be formatted for use in a csv file <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// the value formatted for use in a csv file <see cref="System.String"/>
		/// </returns>
		private static string FormatCsvValue(string value) {
			if(value == null) {
				return "";
			} else if (value.IndexOf(',') != -1) {
				return "\"" + value.Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "\\r") + "\"";
			} else {
				return value.Replace("\n", "\\n").Replace("\r", "\\r");
			}
		}

        /// <summary>
        /// Gets the list membership status information for the specified email
        /// </summary>
        /// <param name="userEmail">Email of the user to get list status for</param>
        /// <returns>XML response containing information about user's list membership</returns>
        public XmlDocument AudienceListMembershipStatus(string userEmail) {
            XmlDocument ret = new XmlDocument();
            string url = GetMadMimiURL(String.Format("/audience_members/{0}/lists.xml", userEmail));
            ret.Load(String.Format("{0}?username={1}&api_key={2}", url, Username, APIKey));
            return ret;
        }

        /// <summary>
        /// Adds an existing email address to an audience list
        /// </summary>
        /// <param name="userEmail">Email of existing audience member</param>
        /// <param name="listName">Name of audience list to add email to</param>
        /// <returns>True if a status code of OK is returned</returns>
        public ApiResult AddAudienceListMembership(string userEmail, string listName) {
            string data = String.Format("email={0}", HttpUtility.UrlEncode(userEmail));
            string url = String.Format("/audience_lists/{0}/add", HttpUtility.UrlEncode(listName));
            return PostToMadMimi(url, data);
        }

        /// <summary>
        /// Removes an email from an audience list
        /// </summary>
        /// <param name="userEmail">Email address to remove</param>
        /// <param name="listName">Name of audience list</param>
        /// <returns>True if a status code of OK is returned</returns>
        public ApiResult RemoveAudienceListMembership(string userEmail, string listName) {
            string data = String.Format("email={0}", userEmail);
            return PostToMadMimi(String.Format("/audience_lists/{0}/remove", HttpUtility.UrlEncode(listName)), data);
        }

        /// <summary>
        /// Returns a list of strings containg email addresses suppressed since the specified date
        /// </summary>
        /// <param name="date">Start date of suppression list</param>
        /// <returns>List of strings containing suppressed email addresses</returns>
        public List<String> SuppressedSince(DateTime date) {
            int ts = DateTimeToUnixTime(date);

            lastResult = GetFromMadMimi(String.Format("/audience_members/suppressed_since/{0}.txt", ts.ToString()), false);
            if(lastResult.IsSuccess) {
				return new List<string>(lastResult.ResultBody.Split('\n'));
            } else {
				return null;
			}
        }

        /// <summary>
        /// Gets a list of existing promotions
        /// </summary>
        /// <returns>XML document containing information about existing promotions and mailings</returns>
        public XmlDocument PromotionList() {
            XmlDocument ret = new XmlDocument();
            string url = GetMadMimiURL("/promotions.xml");
            ret.Load(String.Format("{0}?username={1}&api_key={2}", url, Username, APIKey));
            return ret;
        }

        /// <summary>
        /// Gets statistics for a specific mailing
        /// </summary>
        /// <param name="PromoID">ID of promotion</param>
        /// <param name="MailingID">ID of mailing</param>
        /// <returns>XML document containing stats for the specified mailing</returns>
        public XmlDocument MailingStats(string PromoID, string MailingID) {
            string url = GetMadMimiURL(String.Format("/promotions/{0}/mailings/{1}.xml", PromoID, MailingID));
            XmlDocument ret = new XmlDocument();
            ret.Load(String.Format("{0}?username={1}&api_key={2}", url, Username, APIKey));
            return ret;
        }


        /// <summary>
        /// Sends a transactional email. NOTE: Requires a subscription to the mailer API.
        /// </summary>
        /// <param name="item">Mailer Item to send</param>
        /// <returns>Transaction ID</returns>
        public ApiResult SendEmail(MailItem item, string promoName) {
            return PostToMadMimi("", item.BuildRequest(promoName), true);
        }

        /// <summary>
        /// Sends a promotional email to an audience list. NOTE: Requires a subscription to the
        /// mailer API.
        /// </summary>
        /// <param name="item">Mailer Item to send</param>
        /// <param name="listName">Audience List to send to</param>
        /// <param name="promotionName">Name of the promotion</param>
        /// <returns>Transaction ID</returns>
        public ApiResult SendPromotion(MailItem item, string listName, string promotionName) {
            return PostToMadMimi("/to_list", item.BuildRequest(promotionName, listName), true);
        }


        /// <summary>
        /// Gets the status of an email sent via the MailerAPI
        /// </summary>
        /// <param name="TransactionID">Transaction ID of the email to check</param>
        /// <returns>Status of the email</returns>
        public MadMimiMailStatus GetEmailStatus(string TransactionID) {
            // Note: This call requires the path /mailers/ instead of /mailer/  - not sure why this was done
			// Marc says: Wow! That's the first time I've become aware of that. TODO: Fix (but allow both so as not to break production code)
            GetFromMadMimi(String.Format("s/status/{0}", TransactionID), true);

            if(lastResult.IsSuccess) {
                return (MadMimiMailStatus)(Enum.Parse(typeof(MadMimiMailStatus), lastResult.ResultBody));
            }
			return MadMimiMailStatus.ignorant;
        }
    }
}
