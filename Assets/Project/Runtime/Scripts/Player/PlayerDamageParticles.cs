using UnityEngine;

namespace CyberCruiser
{
    public class PlayerDamageParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _damagedParticles;
        [SerializeField] private ParticleSystem[] _crashingParticles;

        private void OnEnable()
        {
            PlayerManager.OnPlayerHealthStateChange += ToggleDamageParticles;
            PlayerManager.OnPlayerDeath += EnableCrashParticles;
            DisableParticles();
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerHealthStateChange -= ToggleDamageParticles;
            PlayerManager.OnPlayerDeath -= EnableCrashParticles;
        }

        private void DisableParticles()
        {
            for (int i = 0; i < _damagedParticles.Length; i++)
            {
         
                if(_damagedParticles[i].gameObject.activeSelf)
                {
                    _damagedParticles[i].Stop();
                }
            }

            for (int i = 0; i < _crashingParticles.Length; i++)
            {
                if (_crashingParticles[i].gameObject.activeSelf)
                {
                    _crashingParticles[i].Stop();
                }
            }
        }

        private void ToggleDamageParticles(PlayerHealthState playerHealthState)
        {
            switch (playerHealthState)
            {
                case PlayerHealthState.Healthy:
                    DisableParticles();
                    break;
                case PlayerHealthState.Low:
                    _damagedParticles[0].Play();
                    _damagedParticles[1].Stop();
                    break;
                case PlayerHealthState.Critical:
                    _damagedParticles[1].Play();
                    break;
            }
        }

        private void EnableCrashParticles()
        {
            for (int i = 0; i < _crashingParticles.Length; i++)
            {
                _crashingParticles[i].Play();
            }
        }
    }
}