using UnityEngine;

public class ParticleSystemController : GameBehaviour
{
    private ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        GameManager.OnIsGamePaused += IsParticlesPaused;
    }

    private void OnDisable()
    {
        GameManager.OnIsGamePaused += IsParticlesPaused;
    }

    private void IsParticlesPaused(bool isPaused)
    {
        if (_particles == null) return;
        if (isPaused)
        {
            _particles.Pause();
        }

        else if (!isPaused)
        {
            _particles.Play();
        }
    }
}
