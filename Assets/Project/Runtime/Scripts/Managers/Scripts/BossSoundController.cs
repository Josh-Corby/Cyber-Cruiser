using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class BossSoundController : SoundControllerBase
        {
            [SerializeField] private ClipInfo _spawnClip;
            [SerializeField] private ClipInfo _deathClip;
            protected override void OnEnable()
            {
                base.OnEnable();
                EnemySpawner.OnBossSpawned += (_) => PlaySpawnSound();
                Boss.OnBossDiedPosition += (_, _) => PlayDeathSound();
            }

            protected override void OnDisable()
            {
                base.OnDisable();
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