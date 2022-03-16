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
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;


namespace Jyx2.Middleware
{
    public class Tools 
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
        
        #region 数学方法

        /// <summary>
        /// 求向量夹角（欧拉角度）
        /// </summary>
        /// <param name="p_vector2">P vector2.</param>
        public static float Angle(Vector2 p_vector2)
        {
            if (p_vector2.x < 0)
            {
                return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
            }
            else
            {
                return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
            }
        }

        /// <summary>
        /// 判断两点是否在指定距离
        /// 没太看懂这个算法
        /// </summary>
        /// <param name="pos">第一点</param>
        /// <param name="basePoint">第二点</param>
        /// <param name="rotation"></param>
        /// <param name="longArm">距离</param>
        /// <returns></returns>
        public static bool InEclipse(Vector3 pos, Vector3 basePoint, Vector3 rotation, float longArm)
        {
            float a = longArm;
            float b = longArm * Mathf.Cos(Mathf.PI / 180 * rotation.x);
            float x = Mathf.Abs(pos.x - basePoint.x);
            float y = Mathf.Abs(pos.y - basePoint.y);
            float x2 = x * x;
            float y2 = y * y;
            float a2 = a * a;
            float b2 = b * b;
            if (x2 / a2 + y2 / b2 <= 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断一点是否在指定点的扇面范围
        /// </summary>
        /// <param name="vec">两点的向量</param>
        /// <param name="startAngle">扇面起始角度</param>
        /// <param name="rotation_z">扇面旋转角度</param>
        /// <param name="widspread">扇面范围角度</param>
        /// <returns></returns>
        public static bool InRange(Vector2 vec, float startAngle, float rotation_z, float widspread)
        {
            float angle = Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
            float lowerAngle = (startAngle + rotation_z - widspread / 2) % 360;
            float upAngle = (startAngle + rotation_z + widspread / 2) % 360;
            if (upAngle < lowerAngle)
                upAngle = upAngle + 360;

            if ((lowerAngle <= angle && angle <= upAngle) || (lowerAngle <= (angle + 360) && (angle + 360) <= upAngle))
                return true;

            return false;
        }

        /// <summary>
        /// 战场角色间的实际距离
        /// 使用角色的战场坐标计算
        /// </summary>
        /// <param name="sourceX">源x坐标</param>
        /// <param name="sourceY">源y坐标</param>
        /// <param name="targetX">目标x坐标</param>
        /// <param name="targetY">目标y坐标</param>
        /// <returns></returns>
        public static int GetDistance(int sourceX, int sourceY, int targetX, int targetY)
        {
            var X = sourceX;
            var Y = sourceY;
            var spX = targetX;
            var spY = targetY;

            int s2 = spX % 2 == 1 ? 1 : 0;
            int s1 = X % 2 == 1 ? 1 : 0;
            int T = Math.Abs((spY - Y) * 2 + (s2 - s1));
            int TXright = X + T;
            int TXleft = X - T;

            int distance = 0;

            if (spX >= TXright)
                distance = Math.Abs(TXright - X) + Math.Abs((int)(spX - TXright) / 2);
            else if (spX < TXleft)
                distance = Math.Abs(X - TXleft) + Math.Abs((int)(spX - TXleft) / 2);
            else
                distance = T;
            return distance;
        }

        #endregion

        public static IEnumerator HttpGet(string path, Hashtable paramTable, Action<string> callback, Action<string> errCallback = null)
        {
            int i = 0;
            StringBuilder buffer = new StringBuilder();

            if (paramTable != null)
            {
                foreach (string key in paramTable.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, WWW.EscapeURL(paramTable[key] as string));
                    }
                    else
                    {
                        buffer.AppendFormat("?{0}={1}", key, WWW.EscapeURL(paramTable[key] as string));
                    }
                    i++;
                }
            }

            if (buffer.ToString() == string.Empty)
            {
                buffer.AppendFormat("?p=" + GetRandomInt(0, 999999));
            }

            WWW www = new WWW(path + buffer.ToString());
            yield return www;

            int timeout = 100;
            while (!www.isDone && timeout-- > 0)
            {
                yield return www;
            }
            if (string.IsNullOrEmpty(www.error))
            {
                string response = www.text;
                callback(response);
                yield return response;
            }
            else if (errCallback != null)
            {
                errCallback(www.error);
            }

            //www.Dispose();
            yield return null;
        }

        /// <summary>
        /// open URL
        /// </summary>
        /// <param name="url"></param>
        public static void openURL(string url)
        {
            Application.OpenURL(url);
        }
        

        /// <summary>  
        /// 转换十六进制数据为一个rgba或rgb颜色
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <example>
        ///     <code>
        ///          gameobject.GetComponent&lt;Image&gt;().color = Tools.GetColor("3076E6");
        ///          gameobject.GetComponent&lt;Image&gt;().color = Tools.GetColor("3076E600");
        ///     </code>
        /// </example>
        public static Color GetColor(string str)
        {
            var value = Convert.ToInt32(str, 16);

            float a = 255;
            if (str.Length > 6)
            {
                a = value & 0xff;
                value >>= 8;
            }

            float b = value & 0xff;
            float g = (value >> 8) & 0xff;
            float r = (value >> 16) & 0xff;

            return new Color(r / 255, g / 255, b / 255, a / 255);
        }

        /// <summary>
        /// 截图函数
        /// 路径处理
        /// 注：path是一个在Application.persistentDataPath全路径
        /// Application.persistentDataPath这个方法的参数：
        /// 在pc传全路径
        /// 在android和ios上可能传Application.persistentDataPath下的相关路径
        /// </summary>
        /// <param name="path">截图存放路径</param>
        /// <param name="callback">截图成功后的回调</param>
        /// <returns></returns>
        public static IEnumerator CaptureScreenshot(string path, System.Action<bool, string> callback)
        {
            string newPath = path;
#if !UNITY_EDITOR
			newPath = path.Replace(Application.persistentDataPath + "/", "");
#endif
            ScreenCapture.CaptureScreenshot(newPath);
            float time = Time.time;
            bool b = false;
            yield return new WaitUntil(() =>
            {
                b = System.IO.File.Exists(path);
                return b || ((Time.time - time) > 1f);
            });
            string str = path;
            if (b == false)
            {
                str = "截屏出错！";
            }
            if (callback != null)
            {
                callback(b, str);
            }
        }
        
        public static int Limit(int value, int min, int max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }
    }
}

