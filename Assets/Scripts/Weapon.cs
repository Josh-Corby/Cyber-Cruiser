using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] private GameObject firePoint;
    private Transform firePointTransform;
    private bool readyToFire;

    [Header("Gun Base Stats")]
    [SerializeField] private GameObject bulletToFire;
    [SerializeField] private float timeBetweenShots;
    public bool holdToFire;
    [SerializeField] private bool autoFire;

    [Header("Spread Stats")]
    [SerializeField] private bool useSpread;
    [SerializeField] private float spreadAngle;

    [Header("Shotgun Stats")]
    private bool shotgunFire;
    private int shotsInShotgunFire;

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
                Fire();
            }
        }
    }

    public void Fire()
    {
        if (readyToFire)
        {
            //Debug.Log("Bullet fired");
            GameObject bullet = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation);
            readyToFire = false;
            StartCoroutine(ResetShooting());
        }
    }

    private IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToFire = true;
    }
}
