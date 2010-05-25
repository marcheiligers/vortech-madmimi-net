/*=====================================================================================================
 * Class:   Vortech.MadMimi.Mailing
 * Author:  Joshua Jackson <jjackson@vortech.net> http://www.vortech.net
 * Date:    April 30, 2010
 * Purpose: Encasulated a MadMimi promotion mailing
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
    [XmlRoot("mailing")]
    public class MailingStats {
        [XmlElement("sent")]
        public int Sent { get; private set; }
        [XmlElement("views")]
        public int Views { get; private set; }
        [XmlElement("untraced")]
        public int Untraced { get; private set; }
        [XmlElement("clicked")]
        public int Clicked { get; internal set; }
        [XmlElement("links")]
        public int Links { get; set; }
        [XmlElement("forwarded")]
        public int Forwarded { get; set; }
        [XmlElement("bounced")]
        public int Bounced { get; set; }
        [XmlElement("unsubscribed")]
        public int Unsubscribed { get; set; }

        public MailingStats() {
        }
    }
    
    [Serializable]
    public class Mailing {
        [XmlAttribute("id")]
        public string ID { get; set; }
        [XmlElement("started_send")]
        public DateTime StartDate { get; set; }
        [XmlElement("finished_send")]
        public DateTime EndDate { get; set; }
        
        public string PromoID { get; set; }

        private MailingStats _stats;
        [XmlIgnore]
        public MailingStats Stats { 
            get {
                if ((_stats == null) &&
                    (!String.IsNullOrEmpty(ID) && 
                     !String.IsNullOrEmpty(PromoID))) {
                    
                    LoadStats();
                }
                return _stats;
            }
            set {
                _stats = value;
            }
        }

        private void _init() {
            ID = PromoID = String.Empty;
            _stats = null;
        }

        public Mailing() {
            _init();
        }

        public bool LoadStats() {
            return false;
        }

    }
}
