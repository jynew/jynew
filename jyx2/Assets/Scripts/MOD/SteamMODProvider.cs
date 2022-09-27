using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Jyx2.MOD
{
    public class SteamMODProvider: MODProviderBase
    {
        public SteamMODProvider()
        {
            try
            {
                SteamClient.Init(2098790);
            } 
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 获取安装的MOD列表
        /// </summary>
        /// <returns></returns>
        public override List<string> GetInstalledMods()
        {
            var mods = new List<string>();
            var query = Steamworks.Ugc.Query.All.WhereUserSubscribed();
            

            return mods;
        }
        
        public override void LoadMod(string modId)
        {
        }
    }
}