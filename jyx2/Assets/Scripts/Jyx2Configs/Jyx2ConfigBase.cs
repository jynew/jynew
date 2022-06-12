using Cysharp.Threading.Tasks;

namespace Jyx2Configs
{
    abstract public class Jyx2ConfigBase
    {
        //ID
        public int Id;
        
        //名称
        public string Name;

        /// <summary>
        /// 资源预热
        /// </summary>
        /// <returns></returns>
        public abstract UniTask WarmUp();
    }
}