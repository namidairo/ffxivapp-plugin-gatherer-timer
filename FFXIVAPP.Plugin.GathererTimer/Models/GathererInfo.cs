using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using FFXIVAPP.Plugin.GathererTimer.Utilities;

namespace FFXIVAPP.Plugin.GathererTimer.Models {
    public class GathererInfo : INotifyPropertyChanged {
        // from xml

        public String Id { get; set; }
        public String TimeFrom { get; set; }
        public String TimeTo { get; set; }
        public String JobId { get; set; }
        public AreaInfo Area { get; set; }
        public String LocationX { get; set; }
        public String LocationY { get; set; }
        public String Note { get; set; }
        public List<ItemInfo> DeteilItemInfoList { get; set; }
        public int RepresentiveIndex { get; set; }

        // by calc
        public String JobText {
            get {
                return Utility.GetJobText(JobId);
            }
        }
        public String AreaName {
            get {
                String ret = Area.Name;
                if (!"".Equals(ret) && !"".Equals(LocationX) && !"".Equals(LocationY)) {
                    ret += " (" + LocationX + ", " + LocationY + ")";
                }
                return ret;
            }
        }
        public String MapImagePath {
            get {
                String mapImageId = (Id.Length < 2 ? "0" : "") + Id;
                String mapImageFileName = "map_n" + mapImageId + ".png";
                return "pack://application:,,,/FFXIVAPP.Plugin.GathererTimer;component/Data/Maps/" + mapImageFileName;
            }
        }

        public String Title {
            get {
                String ret = "";
                if (RepresentiveIndex < DeteilItemInfoList.Count) {
                    ret = DeteilItemInfoList[RepresentiveIndex].Name;
                    int count = 0;
                    foreach (ItemInfo iInfo in DeteilItemInfoList) {
                        if (iInfo != Constants.DUMMY_ITEM) {
                            count++;
                        }
                    }
                    if (count > 1) {
                        ret += PluginViewModel.Instance.Locale["text_etc"];
                    }
                }
                return ret;
            }
        }
        public String SearchText {
            get {
                String ret = "";

                ret += JobText + " ";
                ret += Area.Name + " ";
                foreach (ItemInfo iInfo in DeteilItemInfoList) {
                    if (iInfo != Constants.DUMMY_ITEM) {
                        ret += iInfo.Name + " ";
                    }
                }

                return ret.ToLower();
            }
        }

        public Boolean IsAlarm { get; set; }

        public int DefaultIndex { get; set; }
        public int SortIndex { get; set; }

        private int _itemStatus = Constants.ITEMSTATUS_NONE;
        public int ItemStatus {
            get {
                return this._itemStatus;
            }
            set {
                if (this._itemStatus == value) {
                    return;
                }
                this._itemStatus = value;
                NotifyPropertyChanged("ItemStatus");
            }
        }

        private String _timeDetailText = "";
        public String TimeDetailText {
            get {
                return this._timeDetailText;
            }
            set {
                if (this._timeDetailText.Equals(value)) {
                    return;
                }
                this._timeDetailText = value;
                NotifyPropertyChanged("TimeDetailText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
