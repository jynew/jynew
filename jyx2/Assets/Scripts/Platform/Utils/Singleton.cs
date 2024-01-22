using System;

namespace Jyx2.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static bool _HasInstanced = false;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_HasInstanced)
                {
                    _HasInstanced = true;
                    _instance = Activator.CreateInstance<T>();
                }
                return _instance;
            }
        }

        protected Singleton() { }

        public virtual void Init() { }

        public virtual void DeInit() { }

        public virtual void Clear()
        {
            _HasInstanced = false;
            _instance = null;
        }
    }
}