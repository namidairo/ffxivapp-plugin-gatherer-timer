using System;
using System.Windows;
using System.Windows.Resources;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Xml;

using FFXIVAPP.Plugin.GathererTimer.Models;

namespace FFXIVAPP.Plugin.GathererTimer.Manager {
    public class GathererInfoManager {

        public static List<GathererInfo> Load() {
            List<GathererInfo> gathererInfoList = new List<GathererInfo>();
            Dictionary<String, ItemInfo>itemInfoMap = new Dictionary<string, ItemInfo>();
            Dictionary<String, AreaInfo> areaInfoMap = new Dictionary<string, AreaInfo>();

                    
            //String gtXMLPath = Path.Combine(Constants.PluginDir, "GathererInfo.xml");
            String resourcePath = "pack://application:,,,/FFXIVAPP.Plugin.GathererTimer;component/Data/GathererInfo.xml";
            try {
                //NOTE error xml is ignore.
                StreamResourceInfo sri = Application.GetResourceStream(new Uri(resourcePath));
                XmlDocument doc = new XmlDocument();
                doc.Load(sri.Stream);

                XmlNodeList itemsNodes = doc.SelectNodes("/gathererInfo/items/data");
                foreach (XmlNode node in itemsNodes) {
                    ItemInfo iInfo = new ItemInfo();
                    iInfo.Id = node.SelectSingleNode("id").InnerText;
                    iInfo.NameJP = node.SelectSingleNode("jp").InnerText;
                    iInfo.NameEN = node.SelectSingleNode("en").InnerText;
                    iInfo.NameFR = node.SelectSingleNode("fr").InnerText;
                    iInfo.NameDE = node.SelectSingleNode("de").InnerText;
                    iInfo.IconFileName = node.SelectSingleNode("icon").InnerText;
                    iInfo.NeedGain = int.Parse(node.SelectSingleNode("needGain").InnerText);
                    iInfo.NeedQuality = int.Parse(node.SelectSingleNode("needQuality").InnerText);
                    itemInfoMap.Add(iInfo.Id, iInfo);
                }

                XmlNodeList areaNodes = doc.SelectNodes("/gathererInfo/areas/data");
                foreach (XmlNode node in areaNodes) {
                    AreaInfo aInfo = new AreaInfo();
                    aInfo.Id = node.SelectSingleNode("id").InnerText;
                    aInfo.NameJP = node.SelectSingleNode("jp").InnerText;
                    aInfo.NameEN = node.SelectSingleNode("en").InnerText;
                    aInfo.NameFR = node.SelectSingleNode("fr").InnerText;
                    aInfo.NameDE = node.SelectSingleNode("de").InnerText;
                    areaInfoMap.Add(aInfo.Id, aInfo);
                }

                List<String> alarmTargetIDList = Properties.Settings.Default.AlarmTargetIDList;

                XmlNodeList gatheringNodes = doc.SelectNodes("/gathererInfo/gathering/data");
                int sortIndex = 0;
                foreach (XmlNode node in gatheringNodes) {
                    GathererInfo gInfo = new GathererInfo();
                    gInfo.Id = node.SelectSingleNode("id").InnerText;
                    gInfo.TimeFrom = node.SelectSingleNode("timeFrom").InnerText;
                    gInfo.TimeTo = node.SelectSingleNode("timeTo").InnerText;
                    gInfo.JobId = node.SelectSingleNode("type").InnerText;
                    gInfo.LocationX = node.SelectSingleNode("locationX").InnerText;
                    gInfo.LocationY = node.SelectSingleNode("locationY").InnerText;
                    gInfo.RepresentiveIndex = int.Parse(node.SelectSingleNode("representative").InnerText) - 1;//1-8 => 0-7
                    String mapId = node.SelectSingleNode("mapId").InnerText;
                    gInfo.Area = areaInfoMap.ContainsKey(mapId) ? areaInfoMap[mapId] : null;

                    XmlNodeList detailNodes = node.SelectNodes("detail/data");
                    gInfo.DeteilItemInfoList = new List<ItemInfo>();
                    foreach (XmlNode detailNode in detailNodes) {
                        ItemInfo info = null;
                        String itemId = detailNode.InnerText;
                        if (!String.IsNullOrWhiteSpace(itemId) && itemInfoMap.ContainsKey(itemId)) {
                            info = itemInfoMap[itemId];
                        }
                        if (null == info) {
                            info = Constants.DUMMY_ITEM;
                        }
                        gInfo.DeteilItemInfoList.Add(info);
                    }

                    //for Hidden items
                    XmlNodeList hiddenItemsNode = node.SelectNodes("hiddens/data");
                    foreach (XmlNode hiddenItemNode in hiddenItemsNode) {
                        ItemInfo info = null;
                        String itemId = hiddenItemNode.InnerText;
                        if (!String.IsNullOrWhiteSpace(itemId) && itemInfoMap.ContainsKey(itemId)) {
                            info = itemInfoMap[itemId];
                            info.IsHidden = true;
                            if (null != info) {
                                gInfo.DeteilItemInfoList.Add(info);
                            }
                        }
                    }

                    gInfo.IsAlarm = alarmTargetIDList.Contains(gInfo.Id);
                    gInfo.DefaultIndex = sortIndex;
                    gInfo.SortIndex = sortIndex;
                    sortIndex++;
                    gathererInfoList.Add(gInfo);
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                gathererInfoList.Clear();
            }
            return gathererInfoList;
        }
    }
}
