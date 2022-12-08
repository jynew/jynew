using UnityEngine;
using XLua;

namespace Jyx2
{
    public partial class Jyx2LuaBridge
    {
        public static bool FastBindEventToObj(string gameObjectPathInLevel, string functionName)
        {
            var levelObj = GameObject.Find("Level");
            var findObj = levelObj.transform.Find(gameObjectPathInLevel);

            if (findObj == null)
            {
                Debug.LogError($"出错了，找不到场景内物体{gameObjectPathInLevel}");
                return false;
            }

            var obj = findObj.gameObject;
            var sc = obj.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 1;

            var evt = obj.AddComponent<GameEvent>();
            evt.m_InteractiveEventId = functionName;
            evt.m_EventTargets = new GameObject[] {obj};

            obj.layer = LayerMask.NameToLayer("GameEvent");
            
            return true;
        }
    }
}