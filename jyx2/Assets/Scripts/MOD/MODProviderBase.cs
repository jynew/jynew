using System.Collections.Generic;

namespace Jyx2.MOD
{
    public abstract class MODProviderBase
    {
        //名字
        public string Name;
        
        //获取所有安装的MOD
        public virtual List<string> GetInstalledMods() { return new List<string>(); }
        
        //加载指定的MOD
        public virtual void LoadMod(string modId) {}
    }
}