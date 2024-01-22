using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using XLua;

namespace Jyx2
{
    public partial class Jyx2LuaBridge
    {
        /// <summary>
        /// 获取场景中的物体
        /// </summary>
        /// <param name="gameObjectPathInLevel"></param>
        /// <param name="findObj"></param>
        /// <returns></returns>
        private static bool GetObjectInLevel(string gameObjectPathInLevel, out GameObject findObj)
        {
            findObj = null;
            var levelObj = GameObject.Find("Level");
            var find = levelObj.transform.Find(gameObjectPathInLevel);
            
            if (find == null)
            {
                Debug.LogError($"出错了，找不到场景内物体{gameObjectPathInLevel}");
                return false;
            }

            findObj = find.gameObject;

            return true;
        }

        /// <summary>
        /// 快速绑定交互触发到场景中的物体
        /// </summary>
        /// <param name="gameObjectPathInLevel">在Level节点下的相对位置</param>
        /// <param name="functionName">绑定触发函数名，支持 {文件名}.{函数}的写法</param>
        /// <param name="radius">触发半径</param>
        /// <returns>是否绑定成功</returns>
        public static bool FastBindEventToObj(string gameObjectPathInLevel, string functionName, float radius = 1f)
        {
            if (!GetObjectInLevel(gameObjectPathInLevel, out var obj)) return false;
            
            var sc = obj.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = radius;

            var obstacle = obj.GetOrAddComponent<NavMeshObstacle>();
            
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.radius = 0.5f;
            obstacle.center = Vector3.up;
            obstacle.height = 2f;

            var evt = obj.AddComponent<GameEvent>();
            evt.m_InteractiveEventId = functionName;
            evt.m_EventTargets = new GameObject[] {obj};

            obj.layer = LayerMask.NameToLayer("GameEvent");

            RegisterDynamicSceneObj(gameObjectPathInLevel); //默认把物体也动态注册上
            
            return true;
        }

        /// <summary>
        /// 注册动态物体
        ///
        /// -1隐藏  0未初始化 1显示
        /// </summary>
        /// <param name="gameObjectPathInLevel"></param>
        /// <returns></returns>
        public static bool RegisterDynamicSceneObj(string gameObjectPathInLevel)
        {
            if (!GetObjectInLevel(gameObjectPathInLevel, out var obj)) return false;

            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用FastBindActive");
                return false;
            }

            string flag = $"SceneObj:{curMap.Id}@{gameObjectPathInLevel}";
            int v = jyx2_GetFlagInt(flag);

            if (v == 0)
            {
                //初始化
                jyx2_SetFlagInt(flag, obj.gameObject.activeSelf ? 1 : -1);
            }
            else
            {
                obj.gameObject.SetActive(v == 1);    
            }
            return true;
        }

        /// <summary>
        /// 快速控制场景内的物体隐藏和显示
        /// </summary>
        /// <param name="gameObjectPathInLevel"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public static bool DynamicSceneObjSetActive(string gameObjectPathInLevel, bool isActive)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用DynamicSceneObjSetActive");
                return false;
            }
            string flag = $"SceneObj:{curMap.Id}@{gameObjectPathInLevel}";
            jyx2_SetFlagInt(flag, isActive ? 1 : -1);
         
            return RegisterDynamicSceneObj(gameObjectPathInLevel); //重新绑定下
        }

        //设置场景变量Int
        public static void SetSceneFlagInt(string flag, int v)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用SetSceneFlag");
                return ;
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            jyx2_SetFlagInt(f, v);
        }
        
        //获取场景变量Int
        public static int GetSceneFlagInt(string flag)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用GetSceneFlag");
                return 0;
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            return jyx2_GetFlagInt(f);
        }
        
        //设置场景变量String
        public static void SetSceneFlagString(string flag, string v)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用SetSceneFlag");
                return ;
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            jyx2_SetFlag(f, v);
        }
        
        //获取场景变量String
        public static string GetSceneFlagString(string flag)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用GetSceneFlag");
                return "";
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            return jyx2_GetFlag(f);
        }
        
        //设置场景变量Bool
        public static void SetSceneFlagBool(string flag, bool v)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用SetSceneFlag");
                return ;
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            jyx2_SetFlagInt(f, v?1:0);
        }
        
        //获取场景变量Bool
        public static bool GetSceneFlagBool(string flag)
        {
            var curMap = LevelMaster.GetCurrentGameMap();
            if (curMap == null)
            {
                Debug.LogError("错误，没有在有效定义的场景内。无法调用GetSceneFlag");
                return false;
            }
            string f = $"SceneFlag:{curMap.Id}@{flag}";
            return jyx2_GetFlagInt(f) == 1;
        }
    }
}