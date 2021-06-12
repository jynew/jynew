using Jyx2.Crossplatform.BasePojo;
using HSFrameWork.Common;
using HSFrameWork.ConfigTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Jyx2
{
    [XmlType("itemdrop")]
    public class ItemDropTemplate : BaseBean
    {
        public override string PK
        {
            get
            {
                return Key;
            }
        }

        [XmlAttribute]
        public string Key;

        [XmlAttribute("Must")]
        public string MustValue;

        [XmlAttribute("Random")]
        public string RandomValue;

        [XmlAttribute]
        public int RandomMin;

        [XmlAttribute]
        public int RandomMax;

        [XmlAttribute]
        public string RandomRateValue;

        [XmlAttribute]
        public string RareValue;

        [XmlAttribute]
        public int RareMin;

        [XmlAttribute]
        public int RareMax;

        [XmlAttribute]
        public string RareRateValue;

        /// <summary>
        /// 获得最大随机词条数
        /// </summary>
        /// <returns></returns>
        public int GetMaxTriggerCount()
        {
            int count = 0;
            if (!string.IsNullOrEmpty(MustValue))
            {
                count += MustValue.Split('\n').Length;
            }
            count += RandomMax;
            return count;
        }

        public List<double> GetRandomRates()
        {
            List<double> rst = new List<double>();
            if (string.IsNullOrEmpty(RandomRateValue)) return rst;
            foreach (var r in RandomRateValue.Split('#'))
            {
                rst.Add(Convert.ToDouble(r));
            }
            return rst;
        }

        public List<double> GetRareRates()
        {
            List<double> rst = new List<double>();
            if (string.IsNullOrEmpty(RareRateValue))
                return rst;
            foreach (var r in RareRateValue.Split('#'))
            {
                rst.Add(Convert.ToDouble(r));
            }
            return rst;
        }

        public IEnumerable<ItemDropTip> GetMustTips()
        {
            if (string.IsNullOrEmpty(MustValue)) yield break;
            foreach (var line in MustValue.Split('\n'))
            {
                yield return ItemDropTip.Parse(line);
            }
        }

        public IEnumerable<ItemDropTip> GetRandomTips()
        {
            if (string.IsNullOrEmpty(RandomValue)) yield break;
            foreach (var line in RandomValue.Split('\n'))
            {
                yield return ItemDropTip.Parse(line);
            }
        }

        public ItemDropTip[] GetRandomTipsArray()
        {
            return GetRandomTips().ToArray();
        }

        public IEnumerable<ItemDropTip> GetRareTips()
        {
            if (string.IsNullOrEmpty(RareValue))
                yield break;
            foreach (var line in RareValue.Split('\n'))
            {
                yield return ItemDropTip.Parse(line);
            }
        }

        //所有词条，用于checker
        public IEnumerable<ItemDropTip> GetAllTips()
        {
            foreach (var t in GetMustTips()) yield return t;
            foreach (var t in GetRandomTips()) yield return t;
            foreach (var t in GetRareTips()) yield return t;
        }

        //所有可洗练出的词条，用于洗练；不包含稀有词条
        public IEnumerable<ItemDropTip> GetAllXilianTips()
        {
            foreach (var t in GetMustTips()) yield return t;
            foreach (var t in GetRandomTips()) yield return t;
        }

        public List<TriggerInstance> GenerateItemTriggers(double teamMf, float percentile)
        {
            List<TriggerInstance> list = new List<TriggerInstance>();

            //固定属性
            foreach(var t in GetMustTips())
            {
                var trigger = t.GenerateItemTrigger(percentile);
                if (trigger != null) list.Add(trigger);
            }

            //随机属性
            List<double> rates = GetRandomRates();
            bool randomGenerated = false;
            for (int i = rates.Count - 1; i >= 0; --i)
            {
                var rate = rates[i] * (1 + teamMf);
                int triggercount = i + 1;
                if (triggercount < RandomMin || triggercount > RandomMax) continue;

                //匹配到了
                if (ToolsShared.ProbabilityTest(rate))
                {
                    list.AddRange(GenerateDifferentTriggers(GetRandomTips().ToList(), i + 1, percentile));
                    randomGenerated = true;
                    break;
                }
            }

            //没有匹配到
            if ((!randomGenerated) && RandomMin > 0)
            {
                list.AddRange(GenerateDifferentTriggers(GetRandomTips().ToList(), RandomMin, percentile));
            }
            return list;
        }

        public List<TriggerInstance> GenerateDifferentTriggers(List<ItemDropTip> tipPools, int count, float percentile, string tag = "")
        {
            List<ItemDropTip> visited = new List<ItemDropTip>();
            List<TriggerInstance> rst = new List<TriggerInstance>();

            if (tipPools.Count < count)
            {
                HSUtils.LogError("物品随机属性个数小于设定个数！模板=" + Key);
                count = tipPools.Count;
            }
            var tList = ToolsShared.GenerateRandomListNotRepeat(tipPools, count);
            for (var i = 0; i < tList.Count; i++)
            {
                var t = tList[i];
                TriggerInstance trigger = t.GenerateItemTrigger(percentile);
                if (trigger != null)
                {
                    trigger.Tag = tag;
                    rst.Add(trigger);
                }
            }
 
            return rst;
        }
    }

    /// <summary> 跨平台 </summary>

    public class ItemDropTip
    {
        public static ItemDropTip Parse(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            ItemDropTip rst = new ItemDropTip();
            rst.key = line.Split(':')[0];
            if (line.Contains("#"))
            {
                rst.paras = line.Split(':')[1].Split('#')[0].Split(',').ToList();
                rst.paras.Add(line.Split(':')[1].Split('#')[1]);
            }else
            {
                rst.paras = line.Split(':')[1].Split(',').ToList();
            }
            return rst;
        }

        public string key;
        public List<string> paras;

        public string GetParaRangeString(int index, float percentile)
        {
            if (paras == null || index >= paras.Count)
                return string.Empty;
            var v = paras[index].Trim('\n');
            if (v.StartsWith("{") && v.EndsWith("}"))
            {//枚举
                v = v.Trim(new char[] { '{', '}' });
                return string.Format("<color=grey>[{0}]</color>", v.Replace("-", ","));
            }
            else if (v.StartsWith("[") && v.EndsWith("]")) //整数范围
            {
                v = v.Trim(new char[] { '[', ']' });

                int min = int.Parse(v.Split('-')[0]);
                int max = int.Parse(v.Split('-')[1]);
                int newmin = (int)(min + (max - min) * percentile);
                if (newmin < min)
                    newmin = min;

                string str = string.Format("<color=grey>[{0}-{1}]</color>", newmin, max);
                if (percentile > 0)
                    str = string.Format("<color=grey>[</color><color=green>{0}</color><color=grey>-{1}]</color>", newmin, max);
                //return v.Replace("#", "-");
                return str;
            }
            else if (v.StartsWith("(") && v.EndsWith(")")) //浮点范围
            {
                v = v.Trim(new char[] { '(', ')' });

                float min = float.Parse(v.Split('-')[0]);
                float max = float.Parse(v.Split('-')[1]);
                float newmin = min + (max - min) * percentile;

                string str = string.Format("<color=grey>[{0:F1}-{1:F1}]</color>", newmin, max);
                if (percentile > 0)
                    str = string.Format("<color=grey>[</color><color=green>{0:F1}</color><color=grey>-{1:F1}]</color>", newmin, max);

                return str;

                //return v.Replace("#", "-");
            }
            else
            {
                return string.Format("<color=grey>[{0}]</color>", v);
            }
        }

        //p:用于解析原随机范围
        //percentile:将下限提升至 ：【下限+ （上限-下限）*percentile】 的值。 by PY 2017.7.19
        static public string GeneratePara(string p, float percentile = 0)
        {
            string v = p.Trim('\n');

            if (v.StartsWith("{") && v.EndsWith("}"))//枚举
            {
                v = v.Trim(new char[] { '{', '}' });
                return (ToolsShared.GetRandomElement(v.Split('-')));

            }
            else if (v.StartsWith("[") && v.EndsWith("]")) //整数范围
            {
                v = v.Trim(new char[] { '[', ']' });
                int min = int.Parse(v.Split('-')[0]);
                int max = int.Parse(v.Split('-')[1]);
                int newmin = (int)(min + (max - min) * percentile);
                if (newmin < min)
                    newmin = min;
                return (ToolsSharedEx.GetImbalancedRandomInt(newmin, max).ToString());
            }
            else if (v.StartsWith("(") && v.EndsWith(")")) //浮点范围
            {
                v = v.Trim(new char[] { '(', ')' });
                float min = float.Parse(v.Split('-')[0]);
                float max = float.Parse(v.Split('-')[1]);
                float newmin = min + (max - min) * percentile;
                return (ToolsSharedEx.GetImbalancedRandom(newmin, max).ToString("#.#"));
            }
            else
            {
                return v;
            }
        }

        static List<string> rst = new List<string>();
        public static char[] m_CharBig = new char[] { '{', '}' };
        public static char[] m_CharMid = new char[] { '[', ']' };
        public static char[] m_CharSml = new char[] { '(', ')' };
        static public List<string> TransferParaFormat(string p)
        {
            string v = p.Trim('\n');
            rst.Clear();
            if (v.StartsWith("{") && v.EndsWith("}"))
            {
                v = v.Trim(m_CharBig);
                rst.Add("{}");
                rst.AddRange(v.Split('-'));
            }
            else if (v.StartsWith("[") && v.EndsWith("]"))
            {
                v = v.Trim(m_CharMid);
                string[] vSplit = v.Split('-');
                int min = int.Parse(vSplit[0]);
                int max = int.Parse(vSplit[1]);
                rst.Add("[]");
                rst.Add(min.ToString());
                rst.Add(max.ToString());
            }
            else if (v.StartsWith("(") && v.EndsWith(")"))
            {
                v = v.Trim(m_CharSml);
                string[] vSplit = v.Split('-');
                float min = float.Parse(vSplit[0]);
                float max = float.Parse(vSplit[1]);
                rst.Add("()");
                rst.Add(min.ToString());
                rst.Add(max.ToString());
            }
            else
            {
                int numint; float numfloat;
                if (int.TryParse(v, out numint))
                {
                    rst.Add("[]");
                    rst.Add(numint.ToString());
                    rst.Add(numint.ToString());
                }
                else if (float.TryParse(v, out numfloat))
                {

                    rst.Add("()");
                    rst.Add(numfloat.ToString());
                    rst.Add(numfloat.ToString());
                }
                else
                {
                    rst.Add("{}");
                    rst.Add(v);
                }
            }
            return rst;
        }

        static public List<string> GetAllPossibleName(List<string> paras)
        {
            List<string> rst = new List<string>();
            rst.Clear();

            if (paras.Count < 0)
                return rst;

            if (paras[0] == "{}")
            {
                for (int i = 1; i < paras.Count; i++)
                {
                    rst.Add(paras[i]);
                }
            }

            return rst;
        }

        public TriggerInstance GenerateItemTrigger(float percentile)
        {
            TriggerInstance rst = new TriggerInstance
            {
                Key = GeneratePara(key, percentile)
            };
            if (rst.TriggerData == null)
            {
                rst.Key = "Error";
                rst.ArgvsString = "$$" + key;
                return rst;
            }

            List<string> tmp = new List<string>();
            foreach (var p in paras)
            {
                tmp.Add(GeneratePara(p, percentile));
            }
            string argvs = string.Join(",", tmp.ToArray());
            if (!string.IsNullOrEmpty(rst.TriggerData.KeyParam)) {
                int pos = argvs.LastIndexOf(",");
                argvs = argvs.Substring(0, pos) + "#" + argvs.Substring(pos + 1);
            }
            rst.ArgvsString = argvs;
            return rst;
        }
    }
}

