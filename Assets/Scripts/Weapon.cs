using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : GameBehaviour
{
    [SerializeField] private WeaponScriptableObject _weaponInfo;

    private GameObject _firePoint;
    private Transform _firePointTransform;
    [HideInInspector] public bool readyToFire;
    protected bool _autoFire;

    private string _weaponName;
    private GameObject _objectToFire;
    private float _timeBetweenShots;
    [HideInInspector] public bool _holdToFire;
    private bool _useSpread;
    private float _spreadAngle;
    private bool _burstFire;
    private int _bulletsInBurst;
    private float _timeBetweenBurstShots;

    private void Awake()
    {
        _firePoint = transform.GetChild(0).gameObject;
        _firePointTransform = _firePoint.transform;
        AssignWeaponInfo();
    }

    private void AssignWeaponInfo()
    {
        _weaponName = _weaponInfo.weaponName;
        _objectToFire = _weaponInfo.objectToFire;
        _timeBetweenShots = _weaponInfo.timeBetweenShots;
        _holdToFire = _weaponInfo.holdToFire;
        _useSpread = _weaponInfo.useSpread;
        _spreadAngle = _weaponInfo.spreadAngle;
        _burstFire = _weaponInfo.burstFire;
        _bulletsInBurst = _weaponInfo.bulletsInBurst;
        _timeBetweenBurstShots = _weaponInfo.timeBetweenBurstShots;
    }

    protected virtual void Start()
    {
        readyToFire = true;
    }

    private void Update()
    {
        if (_autoFire)
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
        if (_burstFire)
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

        for (int i = 0; i < _bulletsInBurst; i++)
        {
            Fire();
            yield return new WaitForSeconds(_timeBetweenBurstShots);
        }
        StartCoroutine(ResetShooting());
    }

    public void Fire()
    {
        if (_useSpread)
        {
            Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, Random.Range(-_spreadAngle, _spreadAngle));
            GameObject bullet = Instantiate(_objectToFire, _firePointTransform.position, directionWithSpread);
        }

        if (!_useSpread)
        {
            GameObject bullet = Instantiate(_objectToFire, _firePointTransform.position, _firePointTransform.rotation);
        }
    }

    private IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        //Debug.Log("Gun is ready to fire");
        readyToFire = true;
    }
}
