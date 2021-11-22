/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 */
using System;
using Jyx2.Middleware;


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

        static private DateTime jianghuYearZero = DateTime.MinValue;

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
            double num = Tools.GetRandom(a, a + rate * diff);
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

    }
}
