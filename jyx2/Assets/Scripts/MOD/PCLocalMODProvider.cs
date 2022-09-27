using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Jyx2.ResourceManagement;
using Sirenix.Utilities;
using UnityEngine;

namespace Jyx2.MOD
{
    public class PCLocalMODProvider: MODProviderBase
    {
        private static string ModDir => Application.streamingAssetsPath;
        
        public override async UniTask<List<string>> GetInstalledMods()
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

            return new List<string>();
        }
        
        public override async UniTask LoadMod(string modId)
        {
            await ResLoader.LoadMod(modId, ModDir);
        }
    }
}