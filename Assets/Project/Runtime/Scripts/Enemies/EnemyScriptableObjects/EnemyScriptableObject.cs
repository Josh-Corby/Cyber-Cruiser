using UnityEngine;

namespace CyberCruiser
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/Enemy")]
    public class EnemyScriptableObject : ScriptableObject
    {
        public GameObject EnemyPrefab;
        public string EnemyName;
        public Sprite EnemyImage;
        public string EnemyAbilities;

        [Header("Enemy Stats")]
        public EnemyStats GeneralStats;

        [Header("Movement Stats")]
        public EnemyMovementStats MovementStats;
    }
}