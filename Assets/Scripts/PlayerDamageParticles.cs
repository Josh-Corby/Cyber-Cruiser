using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageParticles : MonoBehaviour
{
    [SerializeField] private GameObject[] _damageParticles;

    private void Awake()
    {
        {
            DisableParticles();
        }
    }

    private void OnEnable()
    {
        PlayerManager.OnPlayerHealthStateChange += ToggleDamageParticles;
    }

    private void OnDisable()
    {
        PlayerManager.OnPlayerHealthStateChange -= ToggleDamageParticles;
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
}
