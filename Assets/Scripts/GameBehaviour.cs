using UnityEngine;

public class GameBehaviour : Utilities
{
    protected static EnemyManager EM { get { return EnemyManager.INSTANCE; } }
    protected static EnemySpawnerManager ESM { get { return EnemySpawnerManager.INSTANCE; } }
    protected static GameManager GM { get { return GameManager.INSTANCE; } }
    protected static GameplayUIManager GUIM { get { return GameplayUIManager.INSTANCE; } }
    protected static InputManager IM { get { return InputManager.INSTANCE; } }
    protected static MissionManager MM { get { return MissionManager.INSTANCE; } }
    protected static PlayerManager PM { get { return PlayerManager.INSTANCE; } }
    protected static PlayerStatsManager PSM { get { return PlayerStatsManager.INSTANCE; } }
    protected static PickupManager PUM { get { return PickupManager.INSTANCE; } }
    protected static RankManager RM { get { return RankManager.INSTANCE; } }
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
