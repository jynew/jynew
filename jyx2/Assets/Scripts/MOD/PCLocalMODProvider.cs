using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities;
using UnityEngine;

namespace Jyx2.MOD
{
    public class PCLocalMODProvider: MODProviderBase
    {
        public override List<string> GetInstalledMods()
        {
            if (Application.isEditor || !Application.isMobilePlatform)
            {
                if (File.Exists("modlist.txt"))
                {
                    List<string> rst = new List<string>();
                    var lines = File.ReadAllLines("modlist.txt");
                    foreach (var line in lines)
                    {
                        if (line.IsNullOrWhitespace()) continue;
                        rst.Add(line);
                    }
                    return rst;
                }
            }
        
            //暂不支持自由扩展MOD
            return new List<string> {"JYX2", "SAMPLE"};
        }
        
        public override void LoadMod(string modId)
        {
        }
    }
}