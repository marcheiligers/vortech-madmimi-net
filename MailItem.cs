/*=====================================================================================================
 * Class:   Vortech.MadMimi.MailItem
 * Author:  Joshua Jackson <jjackson@vortech.net> http://www.vortech.net
 * Date:    April 17, 2010
 * Purpose: Generic mail item class for madmimi API implementation
 * 
 * URL: http://developer.madmimi.com/developer/api
 =====================================================================================================*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Web;

namespace Vortech.MadMimi {

    public enum MadMimiMailType {
        PlainText,
        Html,
        Yaml
    }
    
    public class MailItem {

        public MailAddress Recipient { get; set; }
        public MailAddress Sender { get; set; }
        public string Subject { get; set; }
        public bool Unconfirmed { get; set; }
        public bool Hidden { get; set; }
        public bool UseERB { get; set; }
        private string _body;
        public string Body {
            get {
                return ConstructBody();
            }
            set {
                _body = value;
            }
        }
        public Dictionary<string, string> yamlData { get; set; }
        public MadMimiMailType BodyType { get; set; }

        public MailItem() {
            yamlData = new Dictionary<string, string>();
            BodyType = MadMimiMailType.Html;
        }

        /// <summary>
        /// Sets the email address of the recipient
        /// </summary>
        /// <param name="Email">Recipient Email Address</param>
        public void SetRecipient(string Email) {
            Recipient = new MailAddress(Email);
        }

        /// <summary>
        /// Sets the recipient email info
        /// </summary>
        /// <param name="Email">Recipient Email Address</param>
        /// <param name="DisplayName">Recipient name</param>
        public void SetRecipient(string Email, string DisplayName) {
            Recipient = new MailAddress(Email, DisplayName);
        }

        /// <summary>
        /// Sents the sender email
        /// </summary>
        /// <param name="Email">Email address of the sender</param>
        public void SetSender(string Email) {
            Sender = new MailAddress(Email);
        }

        /// <summary>
        /// Sets the sender information for the mailing
        /// </summary>
        /// <param name="Email">Email address</param>
        /// <param name="DisplayName">Mail display name</param>
        public void SetSender(string Email, string DisplayName) {
            Sender = new MailAddress(Email, DisplayName);
        }

        /// <summary>
        /// Constructs the body of the email as it should be sent to MadMimi
        /// </summary>
        /// <returns>URL Encoded / translated version of the mail body</returns>
        public string ConstructBody() {
            StringBuilder sb = new StringBuilder();
            if (BodyType != MadMimiMailType.Yaml) {
                return HttpUtility.UrlEncode(_body);
            } else {
                foreach (string k in yamlData.Keys) {
                    sb.AppendLine(String.Format("{0}: {1}", k, HttpUtility.UrlEncode(yamlData[k])));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Build the complete web request to be sent to MadMimi
        /// </summary>
        /// <param name="PromoName">Name of the promotion to use</param>
        /// <returns>Encoded / Translated version of the web request</returns>
        public string BuildRequest(string PromoName) {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("promotion_name={0}&", HttpUtility.UrlEncode(PromoName));
            sb.AppendFormat("recipients={0}&", HttpUtility.UrlEncode(String.Format("{0} <{1}>", Recipient.DisplayName, Recipient.Address)));
            sb.AppendFormat("subject={0}&", HttpUtility.UrlEncode(Subject));
            sb.AppendFormat("from={0}&", HttpUtility.UrlEncode(String.Format("{0} <{1}>", Sender.DisplayName, Recipient.Address)));
            switch (BodyType) {
                case MadMimiMailType.Html:
                    sb.AppendFormat("raw_html={0}", Body);
                    break;
                case MadMimiMailType.PlainText:
                    sb.AppendFormat("raw_plain_text={0}", Body);
                    break;
                case MadMimiMailType.Yaml:
                    sb.AppendFormat("&body=--- {0}", Body);
                    break;

            }

            return sb.ToString();
        }

        /// <summary>
        /// Build the complete web request with list specification to be sent to MadMimi
        /// </summary>
        /// <param name="PromoName">Promotion name to use</param>
        /// <param name="ListName">Audience list to send to</param>
        /// <returns>Encoded / Translated version of the web request</returns>
        public string BuildRequest(string PromoName, string ListName) {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("list_name={0}&", HttpUtility.UrlEncode(ListName));
            sb.Append(BuildRequest(PromoName));
            return sb.ToString();
        }

    }
}
