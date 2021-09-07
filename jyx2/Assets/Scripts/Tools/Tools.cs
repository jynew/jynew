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
using HSFrameWork.Common;

namespace Jyx2.Middleware
{
    public class Tools : ToolsShared
    {
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
    }
}

