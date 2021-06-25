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
