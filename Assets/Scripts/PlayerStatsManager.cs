using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatsManager : GameBehaviour<PlayerStatsManager>
{
    #region Fields
    [SerializeField] private int _playerIon;
    [SerializeField] private int _playerPlasma;
    [SerializeField] private int _plasmaCost;
    [SerializeField] private float _playerCurrentMaxHealth = 5;
    [SerializeField] private float _heatPerShot = 1.75f;
    [SerializeField] private float _weaponUpgradeDuration = 10;

    private const string PLAYER_PLASMA = "PlayerPlasma";
    private const string PLAYER_ION = "PlayerIon";
    private float _iFramesDuration = 0.3f;
    [SerializeField] private bool _isBatteryPack, _isHydrocoolant, _isPlasmaCache, _isPulseDetonator;
    #endregion

    #region Properties
    public int PlayerIonAmount
    {
        get
        {
            return _playerIon;
        }
        set
        {
            _playerIon = value;
            OnIonChange(_playerIon);
        }
    }

    public int PlayerPlasma
    {
        get
        {
            return _playerPlasma;
        }
        set
        {
            _playerPlasma = value;
            //OnPlasmaChange(_playerPlasma);
        }
    }

    public int PlasmaCost
    {
        get
        {
            return _plasmaCost;
        }
        private set
        {
            _plasmaCost = value;
        }
    }

    public float PlayerCurrentMaxHealth
    {
        get
        {
            return _playerCurrentMaxHealth;
        }

        private set
        {
            _playerCurrentMaxHealth = value;
        }
    }

    public float HeatPerShot
    {
        get
        {
            return _heatPerShot;
        }

        set
        {
            _heatPerShot = value;
        }
    }

    public float WeaponUpgradeDuration
    {
        get
        {
            return _weaponUpgradeDuration;
        }
        set
        {
            _weaponUpgradeDuration = value;
        }
    }

    public float IFramesDuration
    {
        get
        {
            return _iFramesDuration;
        }
        private set
        {
            _iFramesDuration = value;
        }
    }

    public bool IsBatteryPack
    {
        get
        {
            return _isBatteryPack;
        }
        set
        {
            _isBatteryPack = value;
            ToggleBatteryPack(_isBatteryPack);
        }
    }

    public bool IsHydrocoolant
    {
        get
        {
            return _isHydrocoolant;
        }
        set
        {
            _isHydrocoolant = value;
            ToggleHydrocoolant(_isHydrocoolant);
        }
    }

    public bool IsPlasmaCache
    {
        get
        {
            return _isPlasmaCache;
        }
        set
        {
            _isPlasmaCache = value;
            TogglePlasmaCache(_isPlasmaCache);
        }
    }

    public bool IsPulseDetonator
    {
        get
        {
            return _isPulseDetonator;
        }
        set
        {
            _isPulseDetonator = value;
            TogglePulseDetonator(_isPulseDetonator);
        }
    }
    #endregion

    #region Actions
    public static event Action<int> OnIonChange = null;
    public static event Action<int> OnPlasmaChange = null;
    #endregion

    private void OnEnable()
    {
        AddOn.OnAddOnToggled += ToggleAddOnBool;
        PlayerManager.OnIonPickup += ChangeIon;
        PlayerManager.OnPlasmaChange += SetPlasma;
        GameManager.OnMissionEnd += DisableAllAddOns;
    }

    private void OnDisable()
    {
        AddOn.OnAddOnToggled -= ToggleAddOnBool;
        PlayerManager.OnIonPickup -= ChangeIon;
        PlayerManager.OnPlasmaChange -= SetPlasma;
        GameManager.OnMissionEnd -= DisableAllAddOns;
    }

    private void Start()
    {
        RestoreValues();
    }

    private void RestoreValues()
    {
        PlayerIonAmount = PlayerPrefs.GetInt(nameof(PLAYER_ION));
        PlayerPlasma = PlayerPrefs.GetInt(nameof(PLAYER_PLASMA));
        PlasmaCost = 5;
    }

    private void ChangeIon(int value)
    {
        PlayerIonAmount += value;
    }

    private void SetPlasma(int value)
    {
        PlayerPlasma = value;
    }

    private void DisableAllAddOns()
    {
        if (IsBatteryPack)
        {
            IsBatteryPack = false;
        }
        if (IsHydrocoolant)
        {
            IsHydrocoolant = false;
        }
        if (IsPlasmaCache)
        {
            IsPlasmaCache = false;
        }
        if (IsPulseDetonator)
        {
            IsPulseDetonator = false;
        }
    }

    private void ToggleAddOnBool(AddOnTypes addOnType, int cost, bool value)
    {
        //If AddOn is being bought check if player can afford it
        if (value)
        {
            CanPlayerAffordAddon(cost);
        }

        //Spend or refund ions depending on bool state
        PlayerIonAmount += value ? -cost : cost;
        //Process ion change
        OnIonChange(PlayerIonAmount);

        //Find function related to addon type
        switch (addOnType)
        {
            case AddOnTypes.BatteryPack:
                IsBatteryPack = value;
                break;
            case AddOnTypes.Hydrocoolant:
                IsHydrocoolant = value;
                break;
            case AddOnTypes.PlasmaCache:
                IsPlasmaCache = value;
                break;
            case AddOnTypes.PulseDetonator:
                IsPulseDetonator = value;
                break;
        }
    }

    private bool CanPlayerAffordAddon(int cost)
    {
        return PlayerIonAmount > cost;
    }

    #region AddOnEffects
    private void ToggleBatteryPack(bool value)
    {
        WeaponUpgradeDuration += value ? 5 : -5;
    }

    private void ToggleHydrocoolant(bool value)
    {
        HeatPerShot += value ? -0.25f : 0.25f;
    }

    private void TogglePlasmaCache(bool value)
    {
        PlasmaCost += value ? -1 : 1;
    }

    //pulse detonator not implemented yet
    private void TogglePulseDetonator(bool value)
    {
    }
    #endregion


    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(nameof(PLAYER_PLASMA), PlayerPlasma);
        PlayerPrefs.SetInt(nameof(PLAYER_ION), PlayerIonAmount);
    }
}
