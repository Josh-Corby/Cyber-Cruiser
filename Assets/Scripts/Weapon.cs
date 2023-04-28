using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : GameBehaviour
{
    #region References
    [SerializeField] private WeaponScriptableObject _weaponInfo;
    private GameObject _firePoint;
    private Transform _firePointTransform;
    [HideInInspector] public GameObject _objectToFire;
    #endregion

    #region Fields
    private bool _readyToFire;
    private bool _holdToFire;
    private bool _isHoming;
    private bool _autoFire;
    private int _bursts;
    private int _multiFireShots;
    private float _timeBetweenShots;
    private float _spreadAngle;
    private float _timeBetweenBurstShots;
    private bool _useSpread;
    private bool _burstFire;
    private bool _multiFire;
    private bool _isMultiFireSpreadRandom;
    #endregion

    #region Properties

    public bool ReadyToFire { get => _readyToFire; private set => _readyToFire = value; }

    public bool HoldToFire { get => _holdToFire; private set => _holdToFire = value; }

    public bool IsHoming { get => _isHoming; set => _isHoming = value; }

    protected bool AutoFire { get => _autoFire; set => _autoFire = value; }
    #endregion

    private void Awake()
    {
        _firePoint = transform.GetChild(0).gameObject;
        _firePointTransform = _firePoint.transform;
        AssignWeaponInfo();
    }

    protected virtual void OnEnable()
    {
        ReadyToFire = true;
    }

    public void AssignWeaponInfo()
    {
        _objectToFire = _weaponInfo.objectToFire;
        _timeBetweenShots = _weaponInfo.timeBetweenShots;
        HoldToFire = _weaponInfo.holdToFire;
        _useSpread = _weaponInfo.useSpread;
        _spreadAngle = _weaponInfo.spreadAngle;
        _burstFire = _weaponInfo.burstFire;
        _bursts = _weaponInfo.bursts;
        _timeBetweenBurstShots = _weaponInfo.timeBetweenBurstShots;
        _multiFire = _weaponInfo.multiFire;
        _multiFireShots = _weaponInfo.multiFireShots;
        _isMultiFireSpreadRandom = _weaponInfo.isMultiFireSpreadRandom;
    }

    private void Update()
    {
        if (AutoFire)
        {
            if (ReadyToFire)
            {
                CheckFireTypes();
            }
        }
    }

    public void CheckFireTypes()
    {
        ReadyToFire = false;
        //check for burst fire
        if (_burstFire)
        {
            StartCoroutine(BurstFire());
            return;
        }
        //check for multifire
        else if (_multiFire)
        {
            MultiFire();
        }
        else
        {
            SpreadCheck();
        }
        StartCoroutine(ResetShooting());
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < _bursts; i++)
        {
            if (_multiFire)
            {
                MultiFire();
            }
            else
            {
                SpreadCheck();
            }
            yield return new WaitForSeconds(_timeBetweenBurstShots);
        }
        StartCoroutine(ResetShooting());
    }

    private void MultiFire()
    {
        if (_isMultiFireSpreadRandom)
        {
            //Debug.Log("Random spread fire");
            for (int i = 0; i < _multiFireShots; i++)
            {
                FireWithSpread(GetRandomSpreadAngle());
            }
        }
        else if (!_isMultiFireSpreadRandom)
        {
            for (int i = 0; i < _multiFireShots; i++)
            {
                Quaternion projectileSpread = GetFixedSpreadAngle(i);
                FireWithSpread(projectileSpread);
            }
            //Debug.Log("set spread fire");    
        }
    }

    private void SpreadCheck()
    {
        if (!_useSpread)
        {
            FireWithoutSpread();
        }
        else
        {
            FireWithSpread(GetRandomSpreadAngle());
        }
    }

    private Quaternion GetRandomSpreadAngle()
    {
        Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, Random.Range(-_spreadAngle, _spreadAngle));
        return directionWithSpread;
    }

    private Quaternion GetFixedSpreadAngle(int index)
    {
        //get total weapon spread
        float totalSpread = _spreadAngle * 2;
        //find what angle the current bullet should be given
        float spreadValue = totalSpread / (_multiFireShots - 1) * index;
        //subtract _spread angle so negative values are assigned
        float angle = spreadValue - _spreadAngle;
        //convert to quaternion
        Quaternion directionWithSpread = _firePointTransform.rotation * Quaternion.Euler(0, 0, angle);
        return directionWithSpread;

        /*
          formula explanation
          
          i value to angle = total spread / number of bullets -1 * i
          final angle = increment - angle
          
          example equations
          fire 3
          spread angle of 10
          total spread if 20
          i values are   0,1 ,2
          increments are 0,10,20
          bullets should fire at angles -10,0,10
          
          i = 0
          angle = 20 / 2 * 0 = 0 - 10 = -10
          i = 1
          angle = 20 / 2 * 1 = 10 - 10 = 0
          i = 2
          angle = 20 / 2 * 2 = 20 - 10 = 10
          
          fire 5
          spread angle of 10
          total spread of 20
          
          i values       0,1,2 ,3 ,4
          increments are 0,5,10,15,20
          bullets should fire at -10,-5,0,5,10
          
          i = 0
          angle = 20 / 4 * 0 = 0 - 10 = -10
          i = 1
          angle = 20 / 4 * 1 = 5 - 10 = -5
          i = 2
          angle = 20 / 4 * 2 = 10 - 10 = 0
          i = 3
          angle = 20 / 4 * 3 = 15 - 10 = 5
          i = 4
         angle = 20 / 4 * 4 = 20 - 10 = 10
         */
    }

    public void FireWithSpread(Quaternion directionWithSpread)
    {
        GameObject bullet = Instantiate(_objectToFire, _firePointTransform.position, directionWithSpread);
        if (IsHoming)
        {
            ApplyHoming(bullet.GetComponent<Bullet>());
        }
    }

    public void FireWithoutSpread()
    {
        GameObject bullet = Instantiate(_objectToFire, _firePointTransform.position, _firePointTransform.rotation);
        if (IsHoming)
        {
            ApplyHoming(bullet.GetComponent<Bullet>());
        }
    }

    private void ApplyHoming(Bullet bullet)
    {
        bullet.IsHoming = true;
    }

    private IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        ReadyToFire = true;
    }

    public void ScatterUpgrade(WeaponUpgradeType scatterType)
    {
        switch (scatterType)
        {
            case WeaponUpgradeType.Scatter_Fixed:
                _isMultiFireSpreadRandom = false;
                break;
            case WeaponUpgradeType.Scatter_Random:
                _isMultiFireSpreadRandom = true;

                break;
        }
        _multiFire = true;
        _multiFireShots = 3;
        _useSpread = true;
        _spreadAngle = 30;
    }
}
