using UnityEngine;

namespace CyberCruiser
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/Enemy")]
    public class EnemyScriptableObject : ScriptableObject
    {
        public GameObject EnemyPrefab;
        public Sprite EnemyImage;
        public string EnemyAbilities;
        public EnemyKillData KillData;

        [Header("Enemy Stats")]
        public EnemyStats GeneralStats;

        [Header("Movement Stats")]
        public EnemyMovementStats MovementStats;

        private void OnEnable()
        {
            SaveManager.OnClearSaveData += ClearData;
        }

        private void OnDisable()
        {
            SaveManager.OnClearSaveData -= ClearData;
        }

        public void ClearData()
        {
            KillData.TimesKilled = 0;
            KillData.PlayerKills = 0;
        }

        public void OnPlayerKilled()
        {
            KillData.PlayerKills++;
        }

        public void OnEnemyDied()
        {
            KillData.TimesKilled++;
        }
    }
}