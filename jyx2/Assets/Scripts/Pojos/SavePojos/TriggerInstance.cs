
using HSFrameWork.SPojo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Jyx2
{
    public class TriggerInstance : SaveablePojo
    {
        public TriggerInstance() { }

        public TriggerInstance(string triggerData)
        {
            Key = triggerData.Split(':')[0];
            ArgvsString = triggerData.Split(':')[1];
        }

        public TriggerInstance(string triggerKey, string argvsString)
        {
            Key = triggerKey;
            ArgvsString = argvsString;
        }

        public TriggerInstance(TriggerInstance trigger)
        {
            _trigger = trigger.TriggerData;
            Key = trigger.Key;
            ArgvsString = trigger.ArgvsString;
            KeyArgv = trigger.KeyArgv;
            CommonArgvs = trigger.CommonArgvs;
        }

        public override string PK
        {
            get
            {
                if (TriggerData == null) return "Error:$$" + Key ;
                string rst = Key;
                if (CommonArgvs != null && CommonArgvs.Length > 0 && !string.IsNullOrEmpty(CommonArgvs[0])) {
                    rst += ":";
                    foreach (var argv in CommonArgvs)
                    {
                        rst += argv + ",";
                    }
                }
                rst = rst.TrimEnd(':');
                rst = rst.TrimEnd(',');
                return rst;
            }
        }

        public string Key
        {
            get { return Get("Key"); }
            set { Save("Key", value); }
        }

        public string ArgvsString
        {
            get{ return Get("ArgvsString"); }
            set{ Save("ArgvsString", value); }
        }

        //关键参数（可以合并的参数）
        //TODO：支持别名
        public double KeyArgv
        {
            get
            {
                if(_keyArgv != Double.MinValue) return _keyArgv;
                if (!ArgvsString.Contains("#")) return Double.MinValue;
                //ArgvsString中#后面为关键参数
                if (_keyArgv == Double.MinValue) _keyArgv = Convert.ToDouble(ArgvsString.Split('#')[1]);
                return _keyArgv;
            }
            set
            {
                _keyArgv = value;
            }
        }
        private double _keyArgv = Double.MinValue;

        //参数（不可合并的参数，作为PK的一部分）
        //TODO：支持别名
        public string[] CommonArgvs
        {
            get
            {
                //ArgvsString中#前面为参数
                if (_commonArgvs == null)
                {
                    _commonArgvs = ArgvsString.Split('#')[0].Split(',');
                }
                for (int i = 0; i < _commonArgvs.Count(); i++)
                {
                    //处理浮点数
                    if (_commonArgvs[i].StartsWith(".")) _commonArgvs[i] = "0" + _commonArgvs[i];
                }
                return _commonArgvs;
            }
            set
            {
                _commonArgvs = value;
            }
        }
        private string[] _commonArgvs;
        
        public int Level
        {
            get { return Get<int>("Level", -1); }
            set { Save("Level", value); }
        }

        public string Source
        {
            get { return Get("Source"); }
            set { Save("Source", value); }
        }

        //合并词条加一个数量标识
        public int Count
        {
            get { return Get("Count", 1); }
            set { Save("Count", value); }
        }

        public string Tag
        {
            get { return Get("Tag"); }
            set { Save("Tag", value); }
        }

        public string Desc
        {
            get
            {
                if (TriggerData == null || string.IsNullOrEmpty(TriggerData.Desc)) return "";

                string desc = TriggerData.Desc;
                if (!string.IsNullOrEmpty(TriggerData.KeyParam))
                {
                    desc = desc.Replace($"{{{TriggerData.KeyParam.Split(':')[0]}}}", (KeyArgv >= 0? "+": "-") + KeyArgv.ToString());
                }
                if (!string.IsNullOrEmpty(TriggerData.CommonParam) && TriggerData.CommonParams.Length > 0)
                {
                    for (int i = 0; i < TriggerData.CommonParams.Length; i++)
                    {
                        var pKey = TriggerData.CommonParams[i].Split(':')[0];
                        var pType = TriggerData.CommonParams[i].Split(':')[1];
                        if(pType == "Attribute")
                            desc = desc.Replace($"{{{pKey}}}", Attribute.Get(CommonArgvs[i].ToString()).Name);
                        else
                            desc = desc.Replace($"{{{pKey}}}", CommonArgvs[i].ToString());
                    }
                }
                return desc;
            }
        }

        /// <summary>
        /// 比较一个词条与目标词条的区别
        /// </summary>
        public static string GetTriggerComparedString(TriggerInstance sourceTrigger, TriggerInstance targetTrigger, bool isReverse = false)
        {
            string up = "<color=#a9ffe0>↑↑";
            string down = "<color=#ffa9a9>↓↓";
            if (sourceTrigger.KeyArgv > targetTrigger.KeyArgv) return isReverse ? down : up + sourceTrigger.Desc + "</color>";
            if (sourceTrigger.KeyArgv < targetTrigger.KeyArgv) return isReverse ? up : down + sourceTrigger.Desc + "</color>";
            return sourceTrigger.Desc;
        }

        /// <summary>
        /// 比较一个词条与目标词条列表中对应词条的区别
        /// </summary>
        public static string GetTriggerComparedString(TriggerInstance sourceTrigger, List<TriggerInstance> targetTriggers, bool isReverse = false)
        {
            if (targetTriggers == null || targetTriggers.Count == 0) return sourceTrigger.Desc;

            var targetTrigger = targetTriggers.Find(trigger => { return trigger.Key == sourceTrigger.Key; });
            if (targetTrigger == null) return sourceTrigger.Desc;

            return GetTriggerComparedString(sourceTrigger, targetTrigger, isReverse);
        }

        public Trigger TriggerData
        {
            get
            {
                if (_trigger == null) _trigger = Trigger.Get(Key);
                return _trigger;
            }
        }
        private Trigger _trigger;
    }
}
