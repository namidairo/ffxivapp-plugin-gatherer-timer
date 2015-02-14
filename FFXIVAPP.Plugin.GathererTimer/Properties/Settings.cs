// FFXIVAPP.Plugin.GathererTimer
// Settings.cs
// 
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using NLog;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;

namespace FFXIVAPP.Plugin.GathererTimer.Properties
{
    internal class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private static Settings _default;

        public static Settings Default
        {
            get { return _default ?? (_default = ((Settings) (Synchronized(new Settings())))); }
        }

        public override void Save()
        {
            // this call to default settings only ensures we keep the settings we want and delete the ones we don't (old)
            DefaultSettings();
            SaveSettingsNode();
            // I would make a function for each node itself; other examples such as log/event would showcase this
            Constants.XSettings.Save(Path.Combine(FFXIVAPP.Common.Constants.PluginsSettingsPath, "FFXIVAPP.Plugin.GathererTimer.xml"));
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();
            Constants.Settings.Add("AlarmTargetIDTSV");
            Constants.Settings.Add("FilterTextTSV");

            Constants.Settings.Add("IsExpandMap");
            Constants.Settings.Add("IsExpandDetail");

            Constants.Settings.Add("IsShowNearestTimeItem");
            Constants.Settings.Add("IsEnabledAlarm");
            Constants.Settings.Add("AlarmVolume");
            Constants.Settings.Add("AlarmTime");
            Constants.Settings.Add("AlarmSoundFile");
        }

        public new void Reset()
        {
            DefaultSettings();
            foreach (var key in Constants.Settings)
            {
                var settingsProperty = Default.Properties[key];
                if (settingsProperty == null)
                {
                    continue;
                }
                var value = settingsProperty.DefaultValue.ToString();
                SetValue(key, value, CultureInfo.InvariantCulture);
                RaisePropertyChanged(key);
            }
        }

        public void SetValue(string key, string value, CultureInfo cultureInfo)
        {
            try
            {
                var type = Default[key].GetType()
                                       .Name;
                switch (type)
                {
                    case "Boolean":
                        Default[key] = Boolean.Parse(value);
                        break;
                    case "Color":
                        var cc = new ColorConverter();
                        var color = cc.ConvertFrom(value);
                        Default[key] = color ?? Colors.Black;
                        break;
                    case "Double":
                        Default[key] = Double.Parse(value, cultureInfo);
                        break;
                    case "Font":
                        var fc = new FontConverter();
                        var font = fc.ConvertFromString(value);
                        Default[key] = font ?? new Font(new FontFamily("Microsoft Sans Serif"), 12);
                        break;
                    case "Int32":
                        Default[key] = Int32.Parse(value, cultureInfo);
                        break;
                    default:
                        Default[key] = value;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        #region Property Bindings (Settings)
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public String AlarmTargetIDTSV {
            get { return ((String)(this["AlarmTargetIDTSV"])); }
            set {
                this["AlarmTargetIDTSV"] = value;
                RaisePropertyChanged();
            }
        }
        public List<String> AlarmTargetIDList {
            get {
                String[] tsv = (AlarmTargetIDTSV ?? "").Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                List<String> ret = new List<string>();
                ret.AddRange(tsv);
                return ret;
            }
            set {
                String result = "";
                foreach (String v in value) {
                    String workV = v.Replace("\t", "");
                    if (!String.IsNullOrWhiteSpace(workV)) {
                        if (!"".Equals(result)) {
                            result += "\t";
                        }
                        result += workV;
                    }
                }
                AlarmTargetIDTSV = result;
            }
        }


        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("MIN\tBTN")]
        public String FilterTextTSV {
            get { return ((String)(this["FilterTextTSV"])); }
            set {
                this["FilterTextTSV"] = value;
                RaisePropertyChanged();
            }
        }
        public List<String> FilterTextList {
            get {
                String[] tsv = (FilterTextTSV ?? "").Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                List<String> ret = new List<string>();
                ret.AddRange(tsv);
                return ret;
            }
            set {
                String result = "";
                foreach (String v in value) {
                    String workV = v.Replace("\t", "");
                    if (!String.IsNullOrWhiteSpace(workV)) {
                        if (!"".Equals(result)) {
                            result += "\t";
                        }
                        result += workV;
                    }
                }
                FilterTextTSV = result;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool IsExpandMap {
            get { return (bool)this["IsExpandMap"]; }
            set {
                this["IsExpandMap"] = value;
                RaisePropertyChanged();
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool IsExpandDetail {
            get { return (bool)this["IsExpandDetail"]; }
            set {
                this["IsExpandDetail"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool IsShowNearestTimeItem {
            get { return (bool)this["IsShowNearestTimeItem"]; }
            set {
                this["IsShowNearestTimeItem"] = value;
                RaisePropertyChanged();
            }
        }
        
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public String AlarmSoundFile {
            get { return ((String)(this["AlarmSoundFile"])); }
            set {
                this["AlarmSoundFile"] = value;
                RaisePropertyChanged();
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0.5")]
        public Double AlarmVolume {
            get { return ((Double)(this["AlarmVolume"])); }
            set {
                this["AlarmVolume"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool IsEnabledAlarm {
            get { return (bool)this["IsEnabledAlarm"];}
            set {
                this["IsEnabledAlarm"] = value;
                RaisePropertyChanged();
            }
        }


        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public String AlarmTime {
            get {
                return ((String)(this["AlarmTime"]));
            }
            set {
                this["AlarmTime"] = value;
            }
        }

        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>0</string>
  <string>1</string>
  <string>2</string>
  <string>3</string>
  <string>4</string>
  <string>5</string>
  <string>6</string>
  <string>7</string>
  <string>8</string>
  <string>9</string>
  <string>10</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection AlarmTimeList {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["AlarmTimeList"]));
            }
        }
        #endregion

        #region Implementation of INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Iterative Settings Saving

        private void SaveSettingsNode()
        {
            if (Constants.XSettings == null)
            {
                return;
            }
            var xElements = Constants.XSettings.Descendants()
                                     .Elements("Setting");
            var enumerable = xElements as XElement[] ?? xElements.ToArray();
            foreach (var setting in Constants.Settings)
            {
                var element = enumerable.FirstOrDefault(e => e.Attribute("Key")
                                                              .Value == setting);
                if (element == null)
                {
                    var xKey = setting;
                    var xValue = Default[xKey].ToString();
                    var keyPairList = new List<XValuePair>
                    {
                        new XValuePair
                        {
                            Key = "Value",
                            Value = xValue
                        }
                    };
                    XmlHelper.SaveXmlNode(Constants.XSettings, "Settings", "Setting", xKey, keyPairList);
                }
                else
                {
                    var xElement = element.Element("Value");
                    if (xElement != null)
                    {
                        xElement.Value = Default[setting].ToString();
                    }
                }
            }
        }

        #endregion
    }
}
