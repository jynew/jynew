using HSFrameWork.Common;
using System.Collections.Generic;
using UnityEngine;

namespace Jyx2.Middleware
{
    public class UnityRichTextTool
    {
        Color color = Color.white;

        /// <summary>
        /// 文字显示工具类
        /// 自动计算color标签的文字长度
        /// </summary>
        /// <param name="text"></param>
        public UnityRichTextTool(string text)
        {
            _text = text;
            int index = 0;
            while (index < text.Length)
            {
                char c = text[index];
                if (c == '<' && text.Substring(index).StartsWith("<color=red>"))
                {
                    index += "<color=red>".Length;
                    if (index >= text.Length) break;
                    c = text[index];
                    color = Color.red;
                }
                else if (c == '<' && text.Substring(index).StartsWith("<color=yellow>"))
                {
                    index += "<color=yellow>".Length;
                    if (index >= text.Length) break;
                    c = text[index];
                    color = Color.yellow;
                }
                else if (c == '<' && text.Substring(index).StartsWith("<color=orange>"))
                {
                    index += "<color=orange>".Length;
                    if (index >= text.Length) break;
                    c = text[index];
                    color = HSColorHelper.fromString("FFA400FF");
                }
                else if (c == '<' && text.Substring(index).StartsWith("</color>"))
                {
                    index += "</color>".Length;
                    if (index >= text.Length) break;
                    c = text[index];
                    color = Color.white;
                }
                chars.Add(c);
                colors.Add(color);
                index++;
            }
        }
        List<char> chars = new List<char>();
        List<Color> colors = new List<Color>();

        private string _text;

        /// <summary>
        /// 获取这个位置的文字
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetChar(int index)
        {
            if (colors[index] == Color.red)
            {
                return string.Format("<color=red>{0}</color>", chars[index]);
            }
            else if (colors[index] == Color.yellow)
            {
                return string.Format("<color=yellow>{0}</color>", chars[index]);
            }
            else if (colors[index] == HSColorHelper.fromString("FFA400FF"))
            {
                return string.Format("<color=orange>{0}</color>", chars[index]);
            }
            return chars[index].ToString();
        }

        /// <summary>
        /// 获取处理后的文字长度
        /// </summary>
        /// <returns></returns>
        public int GetLength() { return chars.Count; }

    }
}
