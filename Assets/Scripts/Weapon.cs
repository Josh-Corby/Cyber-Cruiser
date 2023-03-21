using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] private GameObject firePoint;
    private Transform firePointTransform;
    public bool readyToFire;

    [Header("Gun Base Stats")]
    [SerializeField] private GameObject bulletToFire;
    [SerializeField] private float timeBetweenShots;
    public bool holdToFire;
    [SerializeField] private bool autoFire;

    [Header("Spread Stats")]
    [SerializeField] private bool useSpread;
    [SerializeField] private float spreadAngle;

    [Header("Burst Fire Stats")]
    [SerializeField] private bool burstFire;
    [SerializeField] private int bulletsInBurst;
    [SerializeField] private float timeBetweenBurstShots;

    private void Awake()
    {
        firePointTransform = firePoint.transform;
    }

    private void Start()
    {
        readyToFire = true;
    }

    private void Update()
    {
        if (autoFire)
        {
            if (readyToFire)
            {
                StartFireSequence();
            }
        }
    }

    public void StartFireSequence()
    {
        //check for burst fire
        readyToFire = false;
        CheckBurstFire();
        return;
    }
    private void CheckBurstFire()
    {
        //if gun is burst fire start burst fire
        if (burstFire)
        {
            StartCoroutine(BurstFire());
            return;
        }
        //otherwise normal fire

        Fire();
        StartCoroutine(ResetShooting());
    }

    private IEnumerator BurstFire()
    {

        for (int i = 0; i < bulletsInBurst; i++)
        {
            Fire();
            yield return new WaitForSeconds(timeBetweenBurstShots);
        }
        StartCoroutine(ResetShooting());
    }

    public void Fire()
    {
        if (useSpread)
        {
            Quaternion directionWithSpread = firePointTransform.rotation * Quaternion.Euler(0, 0, Random.Range(-spreadAngle, spreadAngle));
            GameObject bullet = Instantiate(bulletToFire, firePointTransform.position, directionWithSpread);
        }

        if (!useSpread)
        {
            GameObject bullet = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation);
        }
    }

    private IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        Debug.Log("Gun is ready to fire");
        readyToFire = true;
    }
}
