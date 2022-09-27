using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Jyx2.MOD
{
    public abstract class MODProviderBase
    {
        //名字
        public string Name;
        
        //获取所有安装的MOD
        public virtual UniTask<List<string>> GetInstalledMods() { return new UniTask<List<string>>(); }
        
        //加载指定的MOD
        public virtual UniTask LoadMod(string modId) { return new UniTask(); }
    }
}