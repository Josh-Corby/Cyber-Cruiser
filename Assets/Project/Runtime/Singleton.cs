using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class Singleton : Utilities { }

    public class Singleton<T> : Singleton where T : Singleton
    {
        public bool dontDestroy;

        private static T instance_;
        public static T INSTANCE
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = FindObjectOfType<T>();
                    if (instance_ == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        singleton.AddComponent<T>();
                    }
                }
                return instance_;
            }
        }

        protected virtual void Awake()
        {
            if (instance_ == null)
            {
                instance_ = this as T;
                if (dontDestroy) DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
