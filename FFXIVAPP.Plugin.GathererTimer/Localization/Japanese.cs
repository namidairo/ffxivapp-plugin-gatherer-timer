// FFXIVAPP.Plugin.GathererTimer
// Japanese.cs
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
    public abstract class Japanese
    {
        private static readonly ResourceDictionary Dictionary = new ResourceDictionary();

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public static ResourceDictionary Context()
        {
            Dictionary.Clear();
            
            Dictionary.Add("job_0", "採掘");
            Dictionary.Add("job_1", "園芸");
            Dictionary.Add("job_2", "漁師");

            Dictionary.Add("list_col_alarm", "");
            Dictionary.Add("list_col_time_et", "時間(ET)");
            Dictionary.Add("list_col_time_lt", "状況");
            Dictionary.Add("list_col_job", "ジョブ");
            Dictionary.Add("list_col_title", "アイテム");
            Dictionary.Add("list_col_item", "アイテム");

            Dictionary.Add("title_gathering_item", "未知ポイント一覧");
            Dictionary.Add("title_map", "マップ");
            Dictionary.Add("title_detail", "詳細");

            Dictionary.Add("text_etc", "など");
            Dictionary.Add("text_not_selected", "[未選択]");
            Dictionary.Add("text_gain", "獲得力 {0}");
            Dictionary.Add("text_quality", "識質力 {0}");

            Dictionary.Add("text_active", "★あと {0}");
            Dictionary.Add("text_next", "あと {0}");
            Dictionary.Add("text_inactive", "({0})");

            Dictionary.Add("text_show_nearest_time_item", "時間が近いアイテムを上に表示する");
            Dictionary.Add("text_alarm_enabled", "サウンドによるアラーム通知");
            Dictionary.Add("text_alarm_sound_configure", "サウンド設定");
            Dictionary.Add("text_alarm_sound", "サウンド:");
            Dictionary.Add("text_alarm_volume", "音量:");
            Dictionary.Add("text_alarm_testplay", "テスト");
            Dictionary.Add("text_alarm_time_earlier", "アラーム通知時間(N分前)");

            Dictionary.Add("text_github", "プロジェクトソースを開く(GitHub)");

            return Dictionary;
        }
    }
}
