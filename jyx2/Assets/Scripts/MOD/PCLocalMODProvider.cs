using System.Collections.Generic;

namespace Jyx2.MOD
{
    public class PCLocalMODProvider: MODProviderBase
    {
        public override List<string> GetInstalledMods()
        {
            var mods = new List<string>();

            return mods;
        }
        
        public override void LoadMod(string modId)
        {
        }
    }
}