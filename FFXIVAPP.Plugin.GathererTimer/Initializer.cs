// FFXIVAPP.Plugin.GathererTimer
// Initializer.cs
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
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using FFXIVAPP.Common.Controls;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Plugin.GathererTimer.Properties;
using FFXIVAPP.Plugin.GathererTimer.Views;

namespace FFXIVAPP.Plugin.GathererTimer
{
    internal static class Initializer
    {
        #region Declarations

        #endregion

        /// <summary>
        /// </summary>
        public static void LoadSettings()
        {
            if (Constants.XSettings != null)
            {
                foreach (var xElement in Constants.XSettings.Descendants()
                                                  .Elements("Setting"))
                {
                    var xKey = (string) xElement.Attribute("Key");
                    var xValue = (string) xElement.Element("Value");
                    if (String.IsNullOrWhiteSpace(xKey) || String.IsNullOrWhiteSpace(xValue))
                    {
                        return;
                    }
                    Settings.Default.SetValue(xKey, xValue, CultureInfo.InvariantCulture);
                    if (!Constants.Settings.Contains(xKey))
                    {
                        Constants.Settings.Add(xKey);
                    }
                }
            }
        }

        public static void LoadSoundsAndCache() {
            PluginViewModel.Instance.SoundFiles.Clear();
            //do your gui stuff here
            var legacyFiles = new List<FileInfo>();
            var filters = new List<string>
            {
                "*.wav",
                "*.mp3"
            };
            foreach (var filter in filters) {
                var files = Directory.GetFiles(Constants.BaseDirectory, filter, SearchOption.AllDirectories)
                                     .Select(file => new FileInfo(file));
                legacyFiles.AddRange(files);
            }
            foreach (var legacyFile in legacyFiles) {
                if (legacyFile.DirectoryName == null) {
                    continue;
                }
                var baseKey = legacyFile.DirectoryName.Replace(Constants.BaseDirectory, "");
                var key = String.IsNullOrWhiteSpace(baseKey) ? legacyFile.Name : String.Format("{0}\\{1}", baseKey.Substring(1), legacyFile.Name);
                if (File.Exists(Path.Combine(Common.Constants.SoundsPath, key))) {
                    continue;
                }
                try {
                    var directoryKey = String.IsNullOrWhiteSpace(baseKey) ? "" : baseKey.Substring(1);
                    var directory = Path.Combine(Common.Constants.SoundsPath, directoryKey);
                    if (!Directory.Exists(directory)) {
                        Directory.CreateDirectory(directory);
                    }
                    File.Copy(legacyFile.FullName, Path.Combine(Common.Constants.SoundsPath, key));
                    SoundPlayerHelper.TryGetSetSoundFile(key);
                } catch (Exception ex) {
                }
            }
            foreach (var cachedSoundFile in SoundPlayerHelper.SoundFileKeys()) {
                PluginViewModel.Instance.SoundFiles.Add(cachedSoundFile);
            }
            PluginViewModel.Instance.SoundFiles.Insert(0, " ");
        }

    }
}
