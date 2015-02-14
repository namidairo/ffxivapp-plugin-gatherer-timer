// FFXIVAPP.Plugin.GathererTimer
// German.cs
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

using System.Windows;

namespace FFXIVAPP.Plugin.GathererTimer.Localization
{
    public abstract class German
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();

            Dictionary.Add("job_0", "MIN");
            Dictionary.Add("job_1", "GÄR");
            Dictionary.Add("job_2", "FIS");

            Dictionary.Add("list_col_alarm", "");
            Dictionary.Add("list_col_time_et", "Zeit(ET)");
            Dictionary.Add("list_col_time_lt", "Zustand");
            Dictionary.Add("list_col_job", "Job");
            Dictionary.Add("list_col_title", "Gegenstände");
            Dictionary.Add("list_col_item", "Gegenstände");

            Dictionary.Add("title_gathering_item", "Liste");
            Dictionary.Add("title_map", "CARTE");
            Dictionary.Add("title_detail", "Detail");

            Dictionary.Add("text_etc", ", etc");
            Dictionary.Add("text_not_selected", "[nicht ausgewählt]");
            Dictionary.Add("text_gain", "Sammelgeschick {0}");
            Dictionary.Add("text_quality", "Wahrnehmung {0}");

            Dictionary.Add("text_active", "{0} Rest");
            Dictionary.Add("text_next", "in {0} aktiv");
            Dictionary.Add("text_inactive", "({0})");

            Dictionary.Add("text_show_nearest_time_item", "Display-Elemente, die in der Nähe von der Zeit an die Spitze genommen werden kann");
            Dictionary.Add("text_alarm_enabled", "Benachrichtigung per Sound");
            Dictionary.Add("text_alarm_sound_configure", "Klang");
            Dictionary.Add("text_alarm_sound", "Klang:");
            Dictionary.Add("text_alarm_volume", "Volume:");
            Dictionary.Add("text_alarm_testplay", "TEST");
            Dictionary.Add("text_alarm_time_earlier", "Alarmzeit (N Minuten früher)");

            Dictionary.Add("text_github", "Open Project Source (GitHub)");


            return Dictionary;
        }
    }
}
