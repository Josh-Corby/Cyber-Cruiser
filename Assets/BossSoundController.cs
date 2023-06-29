using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class BossSoundController : SoundControllerBase
        {
            [SerializeField] private ClipInfo _spawnClip;
            [SerializeField] private ClipInfo _deathClip;
            private void OnEnable()
            {
                EnemySpawner.OnBossSpawned += (_) => PlaySpawnSound();
                Boss.OnBossDiedPosition += (_, _) => PlayDeathSound();
            }

            private void OnDisable()
            {
                EnemySpawner.OnBossSpawned -= (_) => PlaySpawnSound();
                Boss.OnBossDiedPosition -= (_, _) => PlayDeathSound();
            }

            private void PlaySpawnSound()
            {
                PlayNewClip(_spawnClip);
            }

            private void PlayDeathSound()
            {
                PlayNewClip(_deathClip);
            }
        }
    }
}