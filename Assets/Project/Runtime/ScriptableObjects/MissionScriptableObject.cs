using UnityEngine;

public enum MissionPersistence
{
    OneRun, Total
}

public enum MissionConditions
{
    EndMission, CollectPlasma, UseShield, UseWeaponPack, FlyDistance, KillEnemy, DontShootForDistance
}

public enum EnemyTypes
{
    AllBasicEnemies, Gunship, Mine, Missile, Blimp, Slicer, Dragon, AllBosses, Robodactyl, Behemoth, Battlecruiser, CyberKraken
}


[CreateAssetMenu(fileName = "Mission", menuName = "ScriptableObject/New Mission")]
public class MissionScriptableObject : ScriptableObject
{
    public int missionID;
    public string missionDescription;
    public MissionPersistence missionPersistence;
    public MissionConditions missionCondition;
    public EnemyTypes enemy;
    public int missionObjectiveAmount;
    public int missionStarReward;
}
