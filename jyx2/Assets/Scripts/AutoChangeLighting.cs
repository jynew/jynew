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
        private float timetoangle;
        public string GamedaycountString="Gamedaycount";//自定义保存日期数据的关键字
        public int _GameDayCount;//读取日期数据
        //public float Rotatex;//旋转角度的值x

        private void Start()
        {
            if (RuntimeEnvSetup.CurrentModConfig.AutoChangeLightingOn == true)
            {
                isCount = true;
            }
            if (isCount) 
            {
                timetoangle = 360 / TimeChangerate;//时间和角度换算关系
                if (GamedaycountString != null)
                {
                    _GameDayCount = int.Parse(Jyx2.Jyx2LuaBridge.jyx2_GetFlag(GamedaycountString));//读取日期数据
                }
            }
        }
        private void Update()
        {
            if (isCount)
            {
                _RealGameTimeCount += Time.deltaTime;
                if (_RealGameTimeCount >= TimeChangerate)
                {
                    _RealGameTimeCount = 0;
                    if (GamedaycountString != null)
                    {
                        _GameDayCount += 1; //日计数器累加
                    Jyx2.Jyx2LuaBridge.jyx2_SetFlag(GamedaycountString, _GameDayCount.ToString());//存储日期数据
                    }
                }
            }
            float sdeg = ( Time.deltaTime * timetoangle) % 360;//不大于360°
            MainLight.transform.Rotate(Vector3.right * sdeg);
           //Rotatex = MainLight.transform.eulerAngles.x;
        }
    }
}