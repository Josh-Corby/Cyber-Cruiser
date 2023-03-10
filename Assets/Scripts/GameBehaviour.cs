using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : Utilities
{
    protected static EnemySpawnerManager ESM { get { return EnemySpawnerManager.INSTANCE; } }
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
                instance_ = GameObject.FindObjectOfType<T>();
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
