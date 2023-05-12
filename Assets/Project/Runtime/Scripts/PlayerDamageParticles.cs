using UnityEngine;

public class PlayerDamageParticles : MonoBehaviour
{
    [SerializeField] private GameObject[] _damageParticles;
    [SerializeField] private GameObject[] _crashParticles;

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
        for (int i = 0; i < _damageParticles.Length; i++)
        {
            if (_damageParticles[i].activeSelf)
            {
                _damageParticles[i].SetActive(false);
            }
        }

        for (int i = 0; i < _crashParticles.Length; i++)
        {
            if (_crashParticles[i].activeSelf)
            {
                _crashParticles[i].SetActive(false);
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
                _damageParticles[0].SetActive(true);
                break;
            case PlayerHealthState.Critical:
                _damageParticles[1].SetActive(true);
                break;
        }
    }

    private void EnableCrashParticles()
    {
        for (int i = 0; i < _crashParticles.Length; i++)
        {
            _crashParticles[i].SetActive(true);
        }
    }
}
