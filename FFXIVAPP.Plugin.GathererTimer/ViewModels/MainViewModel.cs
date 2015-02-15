// FFXIVAPP.Plugin.GathererTimer
// MainViewModel.cs
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
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.ViewModelBase;
using FFXIVAPP.Plugin.GathererTimer.Views;
using FFXIVAPP.Plugin.GathererTimer.Manager;
using FFXIVAPP.Plugin.GathererTimer.Models;
using FFXIVAPP.Plugin.GathererTimer.Utilities;


namespace FFXIVAPP.Plugin.GathererTimer.ViewModels
{
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static MainViewModel _instance;
        public static ObservableCollection<String> FilterList { get; private set;}
        public static List<GathererInfo> GathererInfoList { get; private set; }

        public static MainViewModel Instance
        {
            get { return _instance ?? (_instance = new MainViewModel()); }
        }

        #endregion

        #region Declarations
        public ICommand LoadedCommand { get; private set; }
        public ICommand KeyUpFilterCommand { get; private set; }
        public ICommand SelectionChangedFilterCommand { get; private set; }
        public ICommand SelectionChangedMainListCommand { get; private set; }
        public ICommand CheckedAlarmCommand { get; private set; }
        public ICommand UncheckedAlarmCommand { get; private set; }
        #endregion

        #region Variables

        private static Boolean isInitialized = false;

        private static List<ItemInfo> listDummyDetail = new List<ItemInfo>();

        private static BitmapImage bmpDummyMap = null;
        private static BitmapImage bmpWorkMap = null;

        private static String filterText = "";

        private static DispatcherTimer filterTimer = new DispatcherTimer();
        private static DispatcherTimer etTimer = new DispatcherTimer();

        private static DateTime dtNow = DateTime.MinValue;
        private static DateTime dtET = DateTime.MinValue;

        private static Dictionary<String, DateTime> alarmRestrictMap = null;

        private static int sortIndexStartPoint = -1;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public MainViewModel() {
            LoadedCommand = new DelegateCommand(Loaded);
            KeyUpFilterCommand = new DelegateCommand(KeyUpFilter);
            SelectionChangedFilterCommand = new DelegateCommand(SelectionChangedFilter);
            SelectionChangedMainListCommand = new DelegateCommand(SelectionChangedMainList);
            CheckedAlarmCommand = new DelegateCommand<RoutedEventArgs>(CheckedAlarm);
            UncheckedAlarmCommand = new DelegateCommand<RoutedEventArgs>(UncheckedAlarm);
        }

        #region Loading Functions

        /// <summary>
        /// UserControl.Loaded
        /// </summary>
        private static void Loaded() {
            if (isInitialized) {
                return;
            }

            // data
            GathererInfoList = GathererInfoManager.Load();

            FilterList = new ObservableCollection<string>();
            foreach (String text in Properties.Settings.Default.FilterTextList) {
                FilterList.Add(text);
            }

            for (int i = 0; i < 8; i++) {
                listDummyDetail.Add(Constants.DUMMY_ITEM);
            }
            bmpDummyMap = new BitmapImage(new Uri("pack://application:,,,/FFXIVAPP.Plugin.GathererTimer;component/Data/Maps/map_error.png"));
            UpdateTimeText();
            UpdateGatheringStatus();
            UpdateGatheringListOrder();

            // binding
            MainView.View.lvList.ItemsSource = GathererInfoList;
            ((CollectionView)CollectionViewSource.GetDefaultView(MainView.View.lvList.ItemsSource)).Filter = FilteringGathererInstList;
            MainView.View.lvDetail.ItemsSource = listDummyDetail;
            MainView.View.imgMap.Source = bmpDummyMap;
            MainView.View.cbFilter.ItemsSource = FilterList;
            
            // default selection
            MainView.View.lvList.SelectedIndex = -1;
            MainView.View.lvDetail.SelectedIndex = -1;

            // event
            PluginViewModel.Instance.PropertyChanged += Instance_PropertyChanged;

            filterTimer.Interval = TimeSpan.FromMilliseconds(100);
            filterTimer.Tick += filterTimer_Tick;
            filterTimer.Stop();

            etTimer.Interval = TimeSpan.FromMilliseconds(1000);
            etTimer.Tick += etTimer_Tick;
            etTimer.Start();

            isInitialized = true;
        }


        #endregion

        #region Command Bindings
        /// <summary>
        /// PropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if ("Locale".Equals(e.PropertyName)) {
                UpdateLanguage();
            }
        }
        /// <summary>
        /// ComboBox.KeyUp (filter)
        /// </summary>
        private static void KeyUpFilter() {
            // lazy evaluation
            filterTimer.Stop();
            filterTimer.Start();
        }
        /// <summary>
        /// ComboBox.SelectionChanged (filter)
        /// </summary>
        private static void SelectionChangedFilter() {
            if (-1 == MainView.View.cbFilter.SelectedIndex) {
                // lazy evaluation
                filterTimer.Stop();
                filterTimer.Start();
            } else {
                // immediatly evaluation
                filterTimer.Stop();
                filterText = MainView.View.cbFilter.Items[MainView.View.cbFilter.SelectedIndex].ToString().ToLower();
                CollectionViewSource.GetDefaultView(MainView.View.lvList.ItemsSource).Refresh();
            }
        }

        /// <summary>
        /// ListView.SelectionChanged (main list)
        /// </summary>
        private static void SelectionChangedMainList() {
            GathererInfo gInfo = (GathererInfo)MainView.View.lvList.SelectedItem;
            if (null == gInfo) {
                return;
            }

            // set data
            MainView.View.lbAreaName.Content = gInfo.AreaName;
            MainView.View.imgMap.Source = new BitmapImage(new Uri(gInfo.MapImagePath));//bmpWorkMap;
            MainView.View.lvDetail.ItemsSource = gInfo.DeteilItemInfoList;
            RefreshDetailListView();
        }
        private static void RefreshDetailListView() {
            // refresh for update language
            MainView.View.lvDetail.Items.Refresh();

            // adjust columns width
            foreach (GridViewColumn column in (MainView.View.lvDetail.View as GridView).Columns) {
                column.Width = 0;
                column.Width = double.NaN;
            }
            MainView.View.lvDetail.SelectedIndex = -1;
        }
        /// <summary>
        /// CheckBox.Checked (ListViewItem in main list)
        /// </summary>
        /// <param name="e"></param>
        private static void CheckedAlarm(RoutedEventArgs e) {
            UpdateCheckedStatus(e);
        }

        /// <summary>
        /// CheckBox.Unchecked (ListViewItem in main list)
        /// </summary>
        /// <param name="e"></param>
        private static void UncheckedAlarm(RoutedEventArgs e) {
            UpdateCheckedStatus(e);
        }

        #endregion

        #region General Functions
        /// <summary>
        /// Timer for filter (lazy evaluation)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void filterTimer_Tick(object sender, EventArgs e) {
            filterTimer.Stop();
            filterText = MainView.View.cbFilter.Text.ToLower();

            // apply filter
            CollectionViewSource.GetDefaultView(MainView.View.lvList.ItemsSource).Refresh();

            // update filter(combobox)
            UpdateFilterTextList(MainView.View.cbFilter.Text);
        }

        /// <summary>
        /// Update filter text list (in combobox)
        /// </summary>
        /// <param name="inputText"></param>
        static void UpdateFilterTextList(String inputText) {
            // TODO ignore not hit 
            // TODO how to count items after filter?
            /*
            if (0 == this.mainViewList.Count) {
                return;
            }*/

            // delete ignore chars
            String workText = Regex.Replace(inputText, "[ |\t|\r|\n]", "");
            String testText = workText.ToLower();
            if (String.IsNullOrWhiteSpace(workText)) {
                return;
            }

            // check already exists?
            Boolean isExist = false;
            foreach (String text in FilterList) {
                if (text.ToLower().Equals(testText)) {
                    isExist = true;
                    break;
                }
            }
            if (isExist) {
                return;
            }

            // check similar text
            // 1. workText:"abc" / textInList:"abcd" => workText is ignore
            // 2. workText:"abc" / textInList:"ab"   => delete textInList, add workText (at first element)
            // 3. other: add workText (at first element), delete old list (list count > max)

            Boolean isAdd = true;
            int removeIndex = -1;
            for (int i = 0; i < FilterList.Count; i++) {
                String targetText = FilterList[i].ToLower();
                // pattern 1: ignore
                if (targetText.StartsWith(testText)) {
                    isAdd = false;
                    break;
                }
                // pattern 2: delete and add
                if (testText.StartsWith(targetText)) {
                    // delete textInList
                    removeIndex = i;
                    break;
                }
            }
            if (-1 != removeIndex) {
                FilterList.RemoveAt(removeIndex);
            }
            if (isAdd) {
                FilterList.Insert(0, workText);
            }
            if (FilterList.Count > Constants.MAX_COUNT_FILTER_LIST) {
                FilterList.RemoveAt(FilterList.Count - 1);
            }

            ///update setting
            List<String> newList = new List<string>();
            newList.AddRange(FilterList);
            Properties.Settings.Default.FilterTextList = newList;
        }

        /// <summary>
        /// filter for main list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool FilteringGathererInstList(object item) {
            Boolean ret = false;
            if (String.IsNullOrEmpty(filterText)) {
                ret = true;
            } else {
                GathererInfo gInfo = (GathererInfo)item;
                ret = (gInfo.SearchText.Contains(filterText));
            }
            return ret;
        }

        /// <summary>
        /// Alarm checked/unchecked
        /// </summary>
        /// <param name="e"></param>
        private static void UpdateCheckedStatus(RoutedEventArgs e) {
            // validate
            if (null == e.Source || !(e.Source is CheckBox)) {
                return;
            }
            CheckBox cb = (CheckBox)e.Source;
            if (null == cb.DataContext || !(cb.DataContext is GathererInfo)) {
                return;
            }

            GathererInfo gInfo = (GathererInfo)cb.DataContext;

            // reset restrict info
            if (alarmRestrictMap.ContainsKey(gInfo.Id)) {
                alarmRestrictMap.Remove(gInfo.Id);
            }

            if ((bool)cb.IsChecked) {
                // checked
                // restrict alarm if now in time
                int alarmEarlyTime = int.Parse(Properties.Settings.Default.AlarmTime);
                DateTime alarmEarlyBaseTime = dtNow.AddMinutes(alarmEarlyTime);
                DateTime dtETTo = GetTargetETTo(gInfo);
                DateTime dtETFrom = GetTargetETFrom(gInfo, dtETTo);
                DateTime dtLTTo = Utility.EorzeaTimeToLocalTime(dtETTo);
                DateTime dtLTFrom = Utility.EorzeaTimeToLocalTime(dtETFrom);

                if (alarmEarlyBaseTime.Ticks >= dtLTFrom.Ticks && alarmEarlyBaseTime.Ticks < dtLTTo.Ticks) {
                    alarmRestrictMap.Add(gInfo.Id, dtLTTo.AddSeconds(10));
                    //Console.WriteLine("add restrict: " + gInfo.Title + "(" + gInfo.TimeFrom + "):" + dtLTTo.AddSeconds(10));
                }
                gInfo.IsAlarm = true;
            } else {
                // unchecked
                gInfo.IsAlarm = false;
            }

            // update setting
            List<String> alarmTargetIDList = Properties.Settings.Default.AlarmTargetIDList;
            if (gInfo.IsAlarm && !alarmTargetIDList.Contains(gInfo.Id)) {
                alarmTargetIDList.Add(gInfo.Id);
            } else if (!gInfo.IsAlarm && alarmTargetIDList.Contains(gInfo.Id)) {
                alarmTargetIDList.Remove(gInfo.Id);
            }
            Properties.Settings.Default.AlarmTargetIDList = alarmTargetIDList;

            e.Handled = true;
        }

        /// <summary>
        /// update language
        /// </summary>
        private static void UpdateLanguage() {
            UpdateTimeText();// update time info
            UpdateGatheringStatus(); // update status text
            MainView.View.lvList.Items.Refresh(); // update item text

            SelectionChangedMainList();// update detail list

            // adjust column width
            foreach (GridViewColumn column in (MainView.View.lvList.View as GridView).Columns) {
                column.Width = 0;
                column.Width = double.NaN;
            }
        }
        #endregion

        #region Time Management
        /// <summary>
        /// update time info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void etTimer_Tick(object sender, EventArgs e) {
            Boolean isUpdate = UpdateTimeText();

            // if time info is not updated, nothing to do.
            if (!isUpdate) {
                return;
            }
            UpdateGatheringStatus();
            UpdateGatheringListOrder();
        }

        /// <summary>
        /// update time info.
        /// </summary>
        /// <returns></returns>
        private static Boolean UpdateTimeText() {
            TimeZoneInfo tzi = TimeZoneInfo.Local;
            dtNow = DateTime.Now;
            dtET = Utility.LocalTimeToEorzeaTime(dtNow);
            String timeText =
                "ET " + dtET.ToString("HH:mm:ss") + " / " +
                "LT " + dtNow.ToString("HH:mm:ss");

            Boolean ret = false;
            if (!MainView.View.lbTime.Content.Equals(timeText)) {
                ret = true;
                MainView.View.lbTime.Content = timeText;
            }
            return ret;
        }

        /// <summary>
        /// update gathering status, and check alarm
        /// </summary>
        private static void UpdateGatheringStatus() {
            // initialize
            int alarmEarlyTime = int.Parse(Properties.Settings.Default.AlarmTime);
            DateTime alarmEarlyBaseTime = dtNow.AddMinutes(alarmEarlyTime);

            Boolean isFirst = false;
            List<GathererInfo> activeAlarmList = new List<GathererInfo>();
            if (null == alarmRestrictMap) {
                isFirst = true;
                alarmRestrictMap = new Dictionary<string, DateTime>();
            }

            // check time
            foreach (GathererInfo gInfo in GathererInfoList) {
                // ex. ET 05:00 / LT 10:00
                // now: ET 5:05 / LT 10:01 => ITEMSTATUS_ACTIVE "remaining MM:SS"
                // now: ET 4:00 / LT 09:50 => ITEMSTATUS_NEXT   "active in MM:SS" (in 10 minutes)
                // now: ET 3:00 / LT 09:40 => ITEMSTATUS_NONE   "(HH:MM)" (with grayout)
                DateTime dtETTo = GetTargetETTo(gInfo);
                DateTime dtETFrom = GetTargetETFrom(gInfo, dtETTo);
                DateTime dtLTTo = Utility.EorzeaTimeToLocalTime(dtETTo);
                DateTime dtLTFrom = Utility.EorzeaTimeToLocalTime(dtETFrom);
                String text = "";
                int itemStatus = Constants.ITEMSTATUS_NONE;

                if (dtET.Ticks >= dtETFrom.Ticks && dtET.Ticks < dtETTo.Ticks) {
                    // active
                    TimeSpan ts = dtLTTo.Subtract(dtNow);
                    text = String.Format(PluginViewModel.Instance.Locale["text_active"], ts.ToString(@"mm\:ss"));
                    itemStatus = Constants.ITEMSTATUS_ACTIVE;

                } else if (dtET.Ticks + Constants.NEXT_ACTIVE_INTERVAL_ET * 10000 * 1000 >= dtETFrom.Ticks) {
                    // next
                    TimeSpan ts = dtLTFrom.Subtract(dtNow);
                    text = String.Format(PluginViewModel.Instance.Locale["text_next"], ts.ToString(@"mm\:ss"));
                    itemStatus = Constants.ITEMSTATUS_NEXT;

                } else {
                    // inactive
                    text = String.Format(PluginViewModel.Instance.Locale["text_inactive"], dtLTFrom.ToString("HH:mm"));
                }

                gInfo.TimeDetailText = text;
                gInfo.ItemStatus = itemStatus;

                // check alarm
                if (alarmRestrictMap.ContainsKey(gInfo.Id)) {
                    if (alarmRestrictMap[gInfo.Id].Ticks < dtNow.Ticks) {
                        //Console.WriteLine("deactive:" + gInfo.Title + "(" + gInfo.TimeFrom + ")");
                        alarmRestrictMap.Remove(gInfo.Id);
                    }
                }
                if (alarmEarlyBaseTime.Ticks >= dtLTFrom.Ticks && alarmEarlyBaseTime.Ticks < dtLTTo.Ticks) {
                    // active alarm
                    if (alarmRestrictMap.ContainsKey(gInfo.Id)) {
                        //disabled
                    } else {
                        //enabled
                        //Console.WriteLine("active: " + gInfo.Title + "(" + gInfo.TimeFrom + "):" + dtLTTo.AddSeconds(10));
                        alarmRestrictMap.Add(gInfo.Id, dtLTTo.AddSeconds(10));
                        if (gInfo.IsAlarm) {
                            activeAlarmList.Add(gInfo);
                        }
                    }
                }
            }

            //this.lvList.Items.Refresh();

            if (!isFirst && 0 != activeAlarmList.Count) {
                DoAlarm(activeAlarmList);
            }
        }

        /// <summary>
        /// alarm
        /// </summary>
        /// <param name="activeAlarmList"></param>
        private static void DoAlarm(List<GathererInfo> activeAlarmList) {
            if (!Properties.Settings.Default.IsEnabledAlarm) {
                return;
            }
            if (Properties.Settings.Default.AlarmSoundFile.Trim() == "") {
                return;
            }
            var volume = Properties.Settings.Default.AlarmVolume * 100;
            SoundPlayerHelper.PlayCached(SettingsView.View.TSound.Text, (int)volume);
        }

        /// <summary>
        /// String ET to DateTime (to)
        /// </summary>
        /// <param name="gInfo"></param>
        /// <returns></returns>
        private static DateTime GetTargetETTo(GathererInfo gInfo) {
            String[] hmText = gInfo.TimeTo.Split(new String[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (2 != hmText.Length) {
                return DateTime.MinValue;
            }
            int hh = int.Parse(hmText[0]);
            int mm = int.Parse(hmText[1]);
            if (hh >= 24) {
                hh -= 24;
            }
            Boolean isTomorrow = false;
            if (dtET.Hour * 60 + dtET.Minute > hh * 60 + mm) {
                isTomorrow = true;
            }
            DateTime dtETWork = new DateTime(
                dtET.Year,
                dtET.Month,
                dtET.Day,
                hh,
                mm,
                0,
                dtET.Kind);
            if (isTomorrow) {
                dtETWork = dtETWork.AddDays(1);
            }
            return dtETWork;
        }

        /// <summary>
        /// String ET(HHMM) to DateTime (from)
        /// </summary>
        /// <param name="gInfo"></param>
        /// <param name="dtETTo"></param>
        /// <returns></returns>
        private static DateTime GetTargetETFrom(GathererInfo gInfo, DateTime dtETTo) {
            String[] hmText = gInfo.TimeFrom.Split(new String[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (2 != hmText.Length) {
                return DateTime.MinValue;
            }
            int hh = int.Parse(hmText[0]);
            int mm = int.Parse(hmText[1]);
            if (hh >= 24) {
                hh -= 24;
            }
            DateTime dtETWork = new DateTime(
                dtETTo.Year,
                dtETTo.Month,
                dtETTo.Day,
                hh,
                mm,
                0,
                dtETTo.Kind);
            if (dtETTo.Hour <= dtETWork.Hour) {
                dtETWork = dtETWork.AddDays(-1);
            }

            return dtETWork;
        }

        /// <summary>
        /// Update order
        /// </summary>
        private static void UpdateGatheringListOrder() {
            int index = -1;
            if (!Properties.Settings.Default.IsShowNearestTimeItem) {
                index = 0;
            } else {
                DateTime dtBaseET = dtET.AddHours(Constants.SHOW_TOP_EARLY_TIME);
                foreach (GathererInfo gInfo in GathererInfoList) {
                    String[] hmText = gInfo.TimeFrom.Split(new String[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (2 != hmText.Length) {
                        continue;
                    }
                    int hh = int.Parse(hmText[0]);
                    if (hh >= 24) {
                        hh -= 24;
                    }
                    if (hh >= dtBaseET.Hour) {
                        index = gInfo.DefaultIndex;
                        break;
                    }
                }
            }
            if (-1 == index || index == sortIndexStartPoint) {
                return;
            }
            sortIndexStartPoint = index;

            int maxIndex = GathererInfoList.Count;
            foreach (GathererInfo gInfo in GathererInfoList) {
                gInfo.SortIndex = (gInfo.DefaultIndex - index < 0 ? gInfo.DefaultIndex - index + maxIndex : gInfo.DefaultIndex - index);
            }

            MainView.View.lvList.Items.SortDescriptions.Clear();
            SortDescription sd = new SortDescription("SortIndex", ListSortDirection.Ascending);
            MainView.View.lvList.Items.SortDescriptions.Add(sd);
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
