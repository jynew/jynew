using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jyx2.Setup;
using HSFrameWork.ConfigTable;
using Jyx2;
using UnityEngine;

namespace Jyx2
{
    public static class BeforeSceneLoad
    {
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        public static void ColdBind()
        {
            CrossplatformSetupHelper.ColdBind();
            ConfigTable.InitSync();
            DebugInfoManager.Init();
            LuaManager.Init();
            loadFinishTask = Jyx2ResourceHelper.Init();
        }

        public static Task loadFinishTask = null;
    }
}
