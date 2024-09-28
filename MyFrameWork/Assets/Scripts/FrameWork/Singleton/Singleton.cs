
using UnityEngine;

namespace Mr3
{
    /// <summary>
    /// 创建普通单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }
        public static T GetInstance()
        {
            return Instance;
        }
    }
}
