using UnityEngine;

namespace CyberCruiser
{
    public class GameBehaviour : Utilities
    {
        protected static EnemyManager EnemyManagerInstance { get { return EnemyManager.INSTANCE; } }
        protected static EnemySpawnerManager EnemySpawnerManagerInstance { get { return EnemySpawnerManager.INSTANCE; } }
        protected static InputManager InputManagerInstance { get { return InputManager.INSTANCE; } }
        protected static PlayerManager PlayerManagerInstance { get { return PlayerManager.INSTANCE; } }
        protected static RankManager RankManagerInstance { get { return RankManager.INSTANCE; } }
    }

    public class GameBehaviour<T> : GameBehaviour where T : GameBehaviour
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