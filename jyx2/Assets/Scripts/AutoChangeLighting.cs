//本方法提供主光源随时间变换角度，在游戏中产生晨昏变化。另附游戏时间计数器。--by citydream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Jyx2
{
    public class AutoChangeLighting : MonoBehaviour
    {
        public GameObject MainLight;
        public float _RealGameTimeCount = 0;
        public bool isCount = false;
        public float TimeChangerate = 300f;//默认游戏时间5分钟等于游戏内1天
        public float SaveTimefrequency = 60f;//默认存储间隔每分钟一次
        private float timetoangle;
        public string RealTimeCountString = "RealtimeCount";//自定义保存实际游戏时间的关键字
        public float _RealTimeCount;//读取累计时间数据

        private void Start()
        {
            if (RuntimeEnvSetup.CurrentModConfig.AutoChangeLightingOn == true)
            {
                isCount = true;
                if (RuntimeEnvSetup.CurrentModConfig.TimeChangerate > 0)
                {
                    TimeChangerate = RuntimeEnvSetup.CurrentModConfig.TimeChangerate;
                    SaveTimefrequency = RuntimeEnvSetup.CurrentModConfig.SaveTimefrequency;
                    RealTimeCountString = RuntimeEnvSetup.CurrentModConfig.RealTimeCountString;
                }
                timetoangle = 360 / TimeChangerate;//时间和角度换算关系
                _RealTimeCount = float.Parse(Jyx2LuaBridge.jyx2_GetFlag(RealTimeCountString));//读取时间数据
            }
        }
        private void Update()
        {
            if (isCount)
            {
                _RealGameTimeCount += Time.deltaTime;
                _RealTimeCount += Time.deltaTime;
                if (_RealGameTimeCount >= SaveTimefrequency && RealTimeCountString != null)
                {
                    Jyx2LuaBridge.jyx2_SetFlag(RealTimeCountString, _RealTimeCount.ToString());//存储时间数据
                }
                if (_RealGameTimeCount >= TimeChangerate)
                {
                    _RealGameTimeCount = 0;
                }
            }
#if UNITY_EDITOR
            if (MainLight != null) //编辑器状态只要绑定了Light就可以
            {
                float sdeg = ( Time.deltaTime * timetoangle) % 360;//不大于360°
                MainLight.transform.Rotate(Vector3.right * sdeg);
            }
#else
            if (LevelMaster.IsInWorldMap && MainLight != null) //实机仅限大地图
            {
                float sdeg = (Time.deltaTime * timetoangle) % 360;//不大于360°
                MainLight.transform.Rotate(Vector3.right * sdeg);
            }
#endif
        }
    }
}