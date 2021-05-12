using System;
using HSFrameWork.Common;

namespace Jyx2.Crossplatform.BasePojo
{
    public static class ToolsSharedEx
    {
        #region 时间
        private const long UnixEpoch = 621355968000000000L;
        /// <summary>
        /// Unix timestamp, val:1970-01-01 00:00:00 UTC
        /// </summary>
        public static readonly DateTime UnixEpochDateTime = new DateTime(UnixEpoch);

        ////真实时间转游戏时间
        //public static string DateToGameTime(DateTime date)
        //{
        //    return HSText.LoadText("CommonSettings_JianghuTime", "江湖{0}年{1}月{2}日{3}时",
        //        ToolsShared.chineseNumber[date.Year - 2015],
        //        ToolsShared.chineseNumber[date.Month],
        //        ToolsShared.chineseNumber[date.Day],
        //        ToolsShared.chineseTime[date.Hour / 2]);
        //}

        ////时间长度
        //public static string HourToChineseTime(int hour)
        //{
        //    int day = hour / 24;
        //    int h = hour % 24;
        //    string rst = "";
        //    if (day > 0)
        //    {
        //        rst += string.Format(HSText.LoadText("TEXT.DayStringFormat", "{0}天"), day);
        //    }
        //    if (h != 0)
        //    {
        //        rst += HSText.LoadText("CommonSettings_ChineseHour", "{0}个时辰", (h / 2));
        //    }
        //    return rst;
        //}

        static private DateTime jianghuYearZero = DateTime.MinValue;

        //获取距离江湖元年的真实时间
        //public static DateTime GetDateSinceJianghuZeroYear()
        //{
        //    if (jianghuYearZero == DateTime.MinValue)
        //    {

        //        var jianghuYearZeroServer = ConfigManager.Get("JIANGHU_YEAR_ZERO", "");
        //        if (!string.IsNullOrEmpty(jianghuYearZeroServer))
        //        {
        //            jianghuYearZero = DateTime.ParseExact(jianghuYearZeroServer, "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
        //        }
        //        else
        //        {
        //            jianghuYearZero = DateTime.ParseExact(ConfigManager.Get("JIANGHU_YEAR_ZERO"), "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
        //        }
        //    }
        //    var date = jianghuYearZero + TimeSpan.FromDays((HSTimeHelper.Now - jianghuYearZero).TotalDays * 4);

        //    return date;
        //}
        #endregion

        //生成a到b直接不均匀分布的随机数,靠近a的部分概率大，靠近b的部分概率小
        public static double GetImbalancedRandom(double a, double b)
        {
            double tmp = 0;
            if (b < a)
            {
                tmp = a;
                a = b;
                b = tmp;
            }

            int rate = 4; //高概率部分爆率为低概率的1/(2 * 5 ) = 1/ 10

            double diff = b - a;
            double num = ToolsShared.GetRandom(a, a + rate * diff);
            if (num >= a && num <= a + (rate - 1) * diff)
            {
                num = a + (num - a) / (double)((rate - 1) * 2);
            }
            else
            {
                num = num - (rate - 1) * diff;
            }

            if (num < a)
                num = a;

            if (num > b)
                num = b;

            return num;
        }

        public static int GetImbalancedRandomInt(int a, int b)
        {
            int tmp = 0;
            if (b < a)
            {
                tmp = a;
                a = b;
                b = tmp;
            }

            int num = (int)GetImbalancedRandom(a, b + 1);
            if (num >= b && b >= a)
                num = b;
            return num;
        }

        //门派等级翻译
        //public static string getMenpaiLevelDesc(string menpaiKey, int level)
        //{
        //    Menpai menpai = ConfigTable.Get<Menpai>(menpaiKey);
        //    if (menpai == null)
        //        return "";

        //    if (menpai.Levels == null || ((menpai.Levels.Count - 1) < level))
        //        return "";

        //    return menpai.Levels[level].Key;
        //}
    }
}
