using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerShieldController : ShieldControllerBase
{
    private PulseDetonator _pulseDetonator;

    #region Fields
    [SerializeField] private int _shieldActiveDuration;
    [SerializeField] private float _shieldActiveTimer;
    [SerializeField] private bool _isPulseDetonator;
    #endregion

    #region Properties
    public int ShieldActiveDuration { get => _shieldActiveDuration; }

    public float ShieldActiveTimer
    {
        get => _shieldActiveTimer;
        set
        {
            _shieldActiveTimer = value;
            OnPlayerShieldsValueChange(GUIM.playerShieldBar, _shieldActiveTimer);
        }
    }

    protected override bool IsShieldsActive
    {
        get => _shieldsActive;
        set
        {
            base.IsShieldsActive = value;

            if (!_shieldsActive)
            {
                OnPlayerShieldsDeactivated(GUIM.playerShieldBar);
            }
            if (_shieldsActive)
            {
                OnPlayerShieldsActivated(GUIM.playerShieldBar, ShieldActiveDuration);
            }
        }
    }
    #endregion

    #region Actions
    public static event Action<UISlider> OnPlayerShieldsDeactivated = null;
    public static event Action<UISlider, float> OnPlayerShieldsActivated = null;
    public static event Action<UISlider, float> OnPlayerShieldsValueChange = null;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _isPulseDetonator = PSM.IsPulseDetonator;
        if (_isPulseDetonator)
        {
            _pulseDetonator = GetComponentInChildren<PulseDetonator>();
        }
    }

    private void OnEnable()
    {
        InputManager.OnShield += CheckShieldsState;
        GameManager.OnMissionEnd += DeactivateShields;
    }

    private void OnDisable()
    {
        InputManager.OnShield -= CheckShieldsState;
        GameManager.OnMissionEnd -= DeactivateShields;
    }

    private void Update()
    {
        if (IsShieldsActive)
        {
            if (ShieldActiveTimer >= 0)
            {
                ShieldActiveTimer -= Time.deltaTime;
            }
            else
            {
                DeactivateShields();
            }
        }
    }

    private void CheckShieldsState()
    {
        if (GM.IsPaused) return;

        if (IsShieldsActive)
        {
            return;
        }

        if (PM.CheckPlasma())
        {
            ActivateShields();
        }
    }

    protected override void ActivateShields()
    {
        if (_isPulseDetonator)
        {
            _pulseDetonator.Detonate();
        }

        if (!_isPulseDetonator)
        {
            IsShieldsActive = true;
            PM.IsPlayerColliderEnabled = false;
        }
    }

    protected override void DeactivateShields()
    {
        ShieldActiveTimer = ShieldActiveDuration;

        IsShieldsActive = false;
        PM.IsPlayerColliderEnabled = true;
    }

    public override void ProcessCollision(GameObject collider, Vector2 collisionPoint)
    {
        if (collider.GetComponent<Boss>()) return;

        else if (collider.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.PickupEffect();
            Destroy(pickup.gameObject);
            return;
        }
        base.ProcessCollision(collider, collisionPoint);
    }

    public override void ReduceShields(float damage)
    {
        ShieldActiveTimer -= damage;
    }
}
