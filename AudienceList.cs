/*=====================================================================================================
 * Class:   Vortech.MadMimi.AudienceList
 * Author:  Joshua Jackson <jjackson@vortech.net> http://www.vortech.net
 * Date:    April 30, 2010
 * Purpose: Encasulated a MadMimi Audience List
 * 
 * URL: http://developer.madmimi.com/developer/api
 * 
 * !! INCOMPLETE !!
 * 
 =====================================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Vortech.MadMimi {
    
    [Serializable]
    public class AudienceList {

        private MailerAPI Mailer;

        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("id")]
        public string ID { get; set; }

        private void _init() {
            Name = String.Empty;
            ID = String.Empty;
        }

        public AudienceList() {
            _init();
        }

        public AudienceList(MailerAPI mailer) {
            Mailer = mailer;
            _init();
        }

        public static bool Create(MailerAPI mailer, string ListName) {
            try {
                return mailer.AudienceListAdd(ListName);
            } catch {
                return false;
            }
        }

        public static List<AudienceList> GetLists(MailerAPI mailer) {

            List<AudienceList> ret = new List<AudienceList>();

            try {
                XmlDocument lists = mailer.AudienceLists();
                if ((lists.ChildNodes.Count > 0) && (lists.Name == "lists")) {
                    foreach(XmlNode l in lists.ChildNodes) {
                        AudienceList a = new AudienceList();
                        // Not using deserialization here as it would be major code overkill for 2 attributes.
                        // If MadMimi expands the returned values, this may be changed
                        a.ID = l.Attributes["id"].Value;
                        a.Name = l.Attributes["name"].Value;
                        ret.Add(a);
                    }
                }
            } catch (Exception e) {
                throw new Exception("Failed to retrieve audience lists. (" + e.Message + ")");
            }

            return ret;

        }

    }
}
