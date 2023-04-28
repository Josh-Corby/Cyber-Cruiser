using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "ScriptableObject/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Unit Info")]
    public GameObject unitPrefab;
    public string unitName;
    public float maxHealth;
    public bool explodeOnDeath;
    public float explosionRadius;
    public float explosionDamage;
    public GameObject explosionEffect;

    public bool clusterOnDeath;
    public bool spawnEnemy;
    public EnemyScriptableObject enemyToSpawn;
    public GameObject objectToSpawn;
    public int amountOfObjects;

    [Header("Movement Info")]
    public MovementTypes moveTypes;
    public float speed;
    public float crashSpeed;

    public bool upDownMovement;
    public float upDownSpeed;
    public float upDownDistance;

    public bool seekPlayer;
    public bool seekPlayerY;
    public bool seekPlayerX;
    public float seekSpeed;

    public bool sinUpDownMovement;
    public float sinFrequency;
    public float sinMagnitude;

    public bool homeOnPlayer;
    public float homeTurnSpeed;
    public float homeTime;
    public bool homeDelay;
    public float homeDelayTime;
}

public enum MovementTypes
{
    Default, UpDown, SeekPlayer, SinUpDown, HomeOnPlayer
}