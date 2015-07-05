using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVAPP.Plugin.GathererTimer.Models {
    public class ItemInfo {

        // from xml

        public String Id { get; set; }
        public String NameJP { get; set; }
        public String NameEN { get; set; }
        public String NameFR { get; set; }
        public String NameDE { get; set; }
        public int NeedGain { get; set; }
        public int NeedQuality { get; set; }
        public String IconFileName { get; set; }
        public Boolean IsHidden { get; set; }

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
        public String NameWithParameter {
            get {
                String ret = Name;
                if (!"".Equals(ret) && !"-".Equals(ret)) {
                    String addText = "";
                    if (NeedGain > 0) {
                        addText = String.Format(PluginViewModel.Instance.Locale["text_gain"], NeedGain.ToString());
                    }
                    if (NeedQuality > 0) {
                        if (!"".Equals(addText)) {
                            addText += " / ";
                        }
                        addText += String.Format(PluginViewModel.Instance.Locale["text_quality"], NeedQuality.ToString());
                    }
                    if (!"".Equals(addText)) {
                        ret += " [" + addText + "]";
                    }
                }
                if (IsHidden) {
                    ret = PluginViewModel.Instance.Locale["text_hidden"] + ": " + ret;
                }
                return ret;

            }
        }
        public String Icon {
            get {
                return "pack://application:,,,/FFXIVAPP.Plugin.GathererTimer;component/Data/Icons/" + IconFileName;
            }
        }

    }
}
