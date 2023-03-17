using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        particles.Play();


    }
}
