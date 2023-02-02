//本方法提供主光源随时间变换角度，在游戏中产生晨昏变化。另附游戏时间计数器。--by citydream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoChangeLighting : MonoBehaviour
{
    public GameObject MainLight;
    public float _RealGameTimeCount = 0;
    public bool isCount = false;
    public float TimeChangerate = 5f;//默认游戏时间5分钟等于游戏内1天
    public float time2angle = 1.2f;//日照5分钟循环一次 --360/(TimeChangerate * 60)
    public int _GameDayCount = int.Parse(Jyx2.Jyx2LuaBridge.jyx2_GetFlag("GameDayCount"));//读取日期数据
    void Update()
    {
        if (isCount)
        {
        _RealGameTimeCount += Time.deltaTime;
            if (_RealGameTimeCount >= TimeChangerate*60)//5分钟1天
            {
                _GameDayCount += 1; //日计数器累加
                Jyx2.Jyx2LuaBridge.jyx2_SetFlag("GameDayCount", _GameDayCount.ToString());//存储日期数据
                _RealGameTimeCount = 0;
            }
        }
        MainLight.transform.Rotate(Vector3.right * Time.deltaTime * time2angle);
    }
}
