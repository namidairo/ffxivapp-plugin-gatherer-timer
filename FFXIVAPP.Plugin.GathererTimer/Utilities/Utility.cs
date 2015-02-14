using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using FFXIVAPP.Plugin.GathererTimer.Models;

namespace FFXIVAPP.Plugin.GathererTimer.Utilities {
    public class Utility {
        private static readonly DateTime BASE_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public static String GetJobText(String jobId) {
            String ret = "-";
            String key = "job_" + jobId;
            if (PluginViewModel.Instance.Locale.ContainsKey(key)) {
                ret = PluginViewModel.Instance.Locale[key];
            } else if (PluginViewModel.Instance.Locale.ContainsKey("job_0")) {
                ret = PluginViewModel.Instance.Locale["job_0"];
            }

            return ret;
        }

        public static BitmapImage CreateBitmapImageFromFile(String path) {
            BitmapImage bmpImage = new BitmapImage();
            FileStream stream = File.OpenRead(path);
            bmpImage.BeginInit();
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.StreamSource = stream;
            bmpImage.EndInit();
            stream.Close();

            return bmpImage;
        }

        public static DateTime LocalTimeToEorzeaTime(DateTime dtBase) {
            long epochTicks = dtBase.ToUniversalTime().Ticks - BASE_EPOCH.Ticks;
            long eorzeaTicks = (long)Math.Round(epochTicks * Constants.ETLT_RATE);
            DateTime ret = new DateTime(eorzeaTicks);

            return ret;
        }
        public static DateTime EorzeaTimeToLocalTime(DateTime dtET) {
            double dtTicksWork = dtET.Ticks / Constants.ETLT_RATE;//epocTicks
            double dtTicks = (new DateTime(1970, 1, 1).Ticks) + dtTicksWork;//date.ToUniversalTime().Ticks
            DateTime dtReverseUTC = new DateTime((long)dtTicks, DateTimeKind.Utc);//date(UTC)
            DateTime ret = dtReverseUTC.ToLocalTime();//date(local)

            return ret;
        }


    }
}
