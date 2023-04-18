using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerShieldController : ShieldControllerBase, IShield
{
    private const string PLAYER_PROJECTILE_LAYER_NAME = "PlayerProjectile";

    [SerializeField] private int _shieldActiveDuration;
    [SerializeField] private float _shieldActiveTimer;

    [SerializeField] private bool _isPulseDetonator;
    private PulseDetonator _pulseDetonator;

    public static event Action<UISlider, float> OnPlayerShieldsActivated = null;
    public static event Action<UISlider> OnPlayerShieldsDeactivated = null;
    public static event Action<UISlider, float> OnPlayerShieldsValueChange = null;

    public int ShieldActiveDuration
    {
        get
        {
            return _shieldActiveDuration;
        }
        set
        {
            _shieldActiveDuration = value;
        }
    }

    public float ShieldActiveTimer
    {
        get
        {
            return _shieldActiveTimer;
        }

        set
        {
            _shieldActiveTimer = value;
            OnPlayerShieldsValueChange(GUIM.playerShieldBar, _shieldActiveTimer);
        }
    }

    protected override bool ShieldsActive
    {
        get
        {
            return _shieldsActive;
        }
        set
        {
            _shieldsActive = value;
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


    protected override void Awake()
    {
        base.Awake();


        _isPulseDetonator = PSM.IsPulseDetonator;
        if (_isPulseDetonator)
        {
            _pulseDetonator = GetComponentInChildren<PulseDetonator>();
        }
    }

    private void Update()
    {
        if (ShieldsActive)
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

    public override void ProcessCollision(GameObject collider)
    {
        if (collider.TryGetComponent<Boss>(out var boss))
        {
            return;
        }

        else if (collider.TryGetComponent<Pickup>(out var pickup))
        {
            pickup.PickupEffect();
            Destroy(pickup.gameObject);
            return;
        }

        base.ProcessCollision(collider);
    }

    public override void ActivateShields()
    {
        if (_isPulseDetonator)
        {
            _pulseDetonator.Detonate();
        }

        if (!_isPulseDetonator)
        {
            ShieldActiveTimer = ShieldActiveDuration;
            base.ActivateShields();
        }
    }


    public override void ReduceShields(float damage)
    {
        ShieldActiveTimer -= damage;
    }

    private void PulseDetonation()
    {

    }

    public override void ReflectProjectile(Bullet bulletToReflect)
    {
        base.ReflectProjectile(bulletToReflect);
        bulletToReflect.gameObject.layer = LayerMask.NameToLayer(PLAYER_PROJECTILE_LAYER_NAME);
    }
}
