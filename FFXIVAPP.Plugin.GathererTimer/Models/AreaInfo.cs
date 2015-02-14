using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVAPP.Plugin.GathererTimer.Models {
    public class AreaInfo {

        // from xml

        public String Id { get; set; }
        public String NameJP { get; set; }
        public String NameEN { get; set; }
        public String NameFR { get; set; }
        public String NameDE { get; set; }

        // by calc

        public String Name {
            get {
                String ret = "";
                String culture = Constants.CultureInfo.TwoLetterISOLanguageName;
                if ("ja" == culture && !String.IsNullOrWhiteSpace(NameJP)) {
                    ret = NameJP;
                } else if ("de" == culture && !String.IsNullOrWhiteSpace(NameDE)) {
                    ret = NameDE;
                } else if ("fr" == culture && !String.IsNullOrWhiteSpace(NameFR)) {
                    ret = NameFR;
                } else {
                    ret = NameEN;
                }
                return ret;
            }
        }
    }
}
