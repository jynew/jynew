using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Hanjiasongshu
{
    public static class Tools
    {

        #region 数学方法

        private static System.Random rnd = new System.Random();

        /// <summary>
        /// 生成a到b之间的随机数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double GetRandom(double a, double b)
        {
            double k = rnd.NextDouble();
            double tmp = 0;
            if (b > a)
            {
                tmp = a;
                a = b;
                b = tmp;
            }
            return b + (a - b)*k;
        }

        public static int GetRandomInt(int a, int b)
        {
            int k = (int) Tools.GetRandom(a, b + 1);
            if (k >= b && b >= a)
                k = b;
            return k;
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
            int num = (int) Tools.GetImbalancedRandom(a, b + 1);
            if (num >= b && b >= a)
                num = b;
            return num;
        }

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

            int rate = 4; //LuaManager.GetConfigInt("IMBALANCED_RARE_RATE");//高概率部分爆率为低概率的1/(2 * 5 ) = 1/ 10

            double num = Tools.GetRandom(a, rate*b);
            if (num >= a && num <= (rate - 1)*b)
            {
                num = a + (num - a)/(double) ((rate - 1)*2);
            }
            else
            {
                num = a + (num - (rate - 1)*b)/(rate*b - (rate - 1)*b)*(b - a);
            }

            if (num < a)
                num = a;

            if (num > b)
                num = b;

            return num;
        }

        public static T GetRandomElement<T>(IEnumerable<T> list)
        {
            return GetRandomElementInList<T>(list.ToList());
        }

        public static T GetRandomElementInList<T>(List<T> list)
        {
            if (list.Count == 0) return default(T);
            return list[GetRandomInt(0, list.Count - 1)];
        }

        public static string GetRandomElement(string[] list)
        {
            if (list.Length == 0) return "";
            return list[GetRandomInt(0, list.Length - 1)];
        }

        public static int[] GetRandomArray(int count, int minNum, int maxNum)
        {
            int j;
            int[] b = new int[count];
            for (j = 0; j < count; j++)
            {
                int i = rnd.Next(minNum, maxNum + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }
        /// <summary>
        /// 测试概率
        /// </summary>
        /// <param name="p">小于1的</param>
        /// <returns></returns>
        public static bool ProbabilityTest(double p)
        {
            if (p < 0) return false;
            if (p >= 1) return true;
            return rnd.NextDouble() < p;
        }

        #endregion

        #region 时间相关

        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static int GetSecondsSince1970()
        {
            return (int) DateTime.Now.ToUniversalTime()
                .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .TotalSeconds;
        }

        //每天刷新的时间（凌晨5点）
        public const int DayBegin = 5;

        public static DateTime GetTodayTime()
        {
            var today = DateTime.Today.AddHours(DayBegin); //凌晨5点刷新
            if (DateTime.Now.Hour < DayBegin)
            {
                today = today.AddHours(-24);
            }
            return today;
        }

        //获取在5点刷新规则下，指定的日期是“哪天”
        public static DateTime GetTodayTime(DateTime date)
        {
            var today = date.Date.AddHours(DayBegin); //凌晨5点刷新
            if (date.Hour < DayBegin)
            {
                today = today.AddHours(-24);
            }
            return today;
        }

        public static DateTime GetWeekBegin()
        {
            var today = GetTodayTime();
            var days = today.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)today.DayOfWeek - 1);
            //var days = today.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)today.DayOfWeek;
            var weekBegin = today.AddDays(-days);
            return weekBegin;
        }

        public static DateTime GetThisMonthTime()
        {
            var diff = DateTime.Now.Day;
            var firstDay = DateTime.Today.AddDays(1 - diff);
            return firstDay;
        }

        #endregion

        #region 字符串操作

        /// <summary>
        /// 给字符串分成多行
        /// </summary>
        /// <param name="content"></param>
        /// <param name="lineLength"></param>
        /// <param name="enterFlag"></param>
        public static string StringToMultiLine(string content, int lineLength, string enterFlag = "\n")
        {
            string rst = "";
            string tmp = content;
            while (tmp.Length > 0)
            {
                if (tmp.Length > lineLength)
                {
                    string line = tmp.Substring(0, lineLength);
                    tmp = tmp.Substring(lineLength, tmp.Length - lineLength);
                    rst += line + "\n";
                }
                else
                {
                    rst += tmp;
                    tmp = "";
                }
            }
            return rst;
        }


        /// <summary>
        /// 将字符串HASH成int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int StringHashtoInt(string str)
        {
            int rst = 0;
            foreach (var c in str)
            {
                rst += Convert.ToInt32(c);
            }
            return rst;
        }

        /// <summary>
        /// 时间转中文
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateToString(DateTime date)
        {
            string str = chineseNumber[date.Year] + "年" +
                         chineseNumber[date.Month] + "月" +
                         chineseNumber[date.Day] + "日";
            return str;
        }

        public static string ToVertical(string value)
        {
            string result = "";
            foreach (var item in value)
            {
                result += item + "\n";
            }
            return result;
        }

        public static string[] chineseNumber = new String[]
        {
            "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九",
            "二十", "二十一", "二十二", "二十三", "二十四", "二十五", "二十六", "二十七", "二十八", "二十九", "三十", "三十一"
        };

        public static char[] chineseTime = new char[] {'子', '丑', '寅', '卯', '辰', '巳', '午', '未', '申', '酉', '戌', '亥'};

        public static bool IsChineseTime(System.DateTime t, char time)
        {
            var index = (int) (t.Hour/2);
            if (index < 0 || index >= chineseTime.Length) return false;
            return chineseTime[index] == time;
        }


        public static string StringWithColorTag(string s)
        {
            return s.Replace("[[red:", "<color='red'>")
                .Replace("[[yellow:", "<color=#DF6A00FF>")
                .Replace("]]", "</color>");
        }

        private static string Md5Encrypt(byte[] buffer)
        {
            System.Security.Cryptography.MD5 alg = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = alg.ComputeHash(buffer);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string Md5Encrypt(string buffer)
        {
            return Md5Encrypt(System.Text.Encoding.UTF8.GetBytes(buffer));
        }

        #endregion

        #region 字典操作

        public static void DesOrderDictionByValue<T, V>(Dictionary<T, V> dic)
        {
            List<KeyValuePair<T, V>> lst = new List<KeyValuePair<T, V>>(dic);
            lst.Sort((s1, s2) => Comparer<V>.Default.Compare(s2.Value, s1.Value));
            dic.Clear();
            foreach (KeyValuePair<T, V> pair in lst)
            {
                dic.Add(pair.Key, pair.Value);
            }
        }

        public static void OrderDictionByValue<T, V>(Dictionary<T, V> dic)
        {
            List<KeyValuePair<T, V>> lst = new List<KeyValuePair<T, V>>(dic);
            lst.Sort((s1, s2) => Comparer<V>.Default.Compare(s1.Value, s2.Value));
            dic.Clear();
            foreach (KeyValuePair<T, V> pair in lst)
            {
                dic.Add(pair.Key, pair.Value);
            }
        }

        #endregion

        #region XML操作

        //public static XElement GetXmlElement(XElement xml, string key)
        //{
        //    return xml.Element(key);
        //}

        //public static IEnumerable<XElement> GetXmlElements(XElement xml, string key)
        //{
        //    return xml.Elements(key);
        //}

        //public static string GetXmlAttribute(XElement xml, string attribute)
        //{
        //    return xml.Attribute(attribute).Value;
        //}

        //public static float GetXmlAttributeFloat(XElement xml, string attribute)
        //{
        //    return float.Parse(xml.Attribute(attribute).Value);
        //}

        //public static int GetXmlAttributeInt(XElement xml, string attribute)
        //{
        //    return int.Parse(xml.Attribute(attribute).Value);
        //}

        //public static bool GetXmlAttributeBool(XElement xml, string attribute)
        //{
        //    return bool.Parse(xml.Attribute(attribute).Value);
        //}

        //public static DateTime GetXmlAttributeDate(XElement xml, string attribute)
        //{
        //    return DateTime.ParseExact(xml.Attribute(attribute).Value, "yyyy-MM-dd HH:mm:ss");
        //}

        public static T LoadObjectFromXML<T>(string xml)
        {
            return DeserializeXML<T>(xml);
        }

        /// <summary>  
        /// 反序列化XML为类实例  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="xmlObj"></param>  
        /// <returns></returns>  
        public static T DeserializeXML<T>(string xmlObj)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (T));
                using (StringReader reader = new StringReader(xmlObj))
                {
                    return (T) serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                return default(T);
            }
        }

        /// <summary>  
        /// 序列化类实例为XML  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static string SerializeXML<T>(T obj)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = Encoding.UTF8;
            settings.NewLineChars = "\n";
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                new XmlSerializer(obj.GetType()).Serialize(writer, obj, ns);
                return sb.ToString();
            }
        }

        #endregion

        #region CRC

        static public string CRC16_C(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi; //CRC寄存器 
            byte CL;
            byte CH; //多项式码&HA001 
            byte SaveHi;
            byte SaveLo;
            byte[] tmpData;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (int i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte) (CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte) (CRC16Hi >> 1); //高位右移一位 
                    CRC16Lo = (byte) (CRC16Lo >> 1); //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC16Lo = (byte) (CRC16Lo | 0x80); //则低位字节右移后前面补1 
                    } //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC16Hi = (byte) (CRC16Hi ^ CH);
                        CRC16Lo = (byte) (CRC16Lo ^ CL);
                    }
                }
            }
            return string.Format("{0}{1}", CRC16Hi, CRC16Lo);
        }

        #endregion

    }
}
