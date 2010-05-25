/*=====================================================================================================
 * Class:   Vortech.MadMimi.Promotion
 * Author:  Joshua Jackson <jjackson@vortech.net> http://www.vortech.net
 * Date:    April 30, 2010
 * Purpose: Encasulated a MadMimi promotion
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
    [XmlRoot("promotion")]
    public class Promotion {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("id")]
        public string ID { get; set; }
        [XmlAttribute("updated_at")]
        public DateTime Updated { get; set; }
        
        [XmlArray("mailings")]
        [XmlArrayItem("mailing")]
        public List<Mailing> Mailings { get; set; }

    }
}
