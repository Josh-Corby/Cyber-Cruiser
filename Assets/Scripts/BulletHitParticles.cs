using UnityEngine;

public class BulletHitParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        particles.Play();


    }
}
