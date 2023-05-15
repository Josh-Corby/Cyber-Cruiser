using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Battlecruiser : Boss, IBoss
{
    [SerializeField] private GameObject _mineReleasePoint;
    [SerializeField] private EnemyScriptableObject _seekerMineInfo;
    [SerializeField] private int _minesToFire = 1;
    private float _beamAttackDuration;

    [SerializeField] private GameObject _pulverizerBeam;
    [SerializeField] private BeamAttack _beamAttack;

    public static event Action OnDied = null;

    protected override void Awake()
    {
        base.Awake();
        _beamAttack = _pulverizerBeam.GetComponent<BeamAttack>();
        _beamAttackDuration = _beamAttack.beamDuration;
    }

    protected override void Update()
    {
        if (_attackTimer > 0)
        {
            _attackTimer -= Time.deltaTime;
        }

        if (_attackTimer <= 0)
        {
            ChooseRandomAttack();
        }
    }

    //check if beam is active
    //if so fire a mine
    protected override void ChooseRandomAttack()
    {
        _attackTimer = _attackCooldown;

        if (_beamAttack.IsBeamActive)
        {
            Attack1();
        }

        else
        {
            int randomAttackID = Random.Range(0, 2);
            PerformAttack(randomAttackID);
        }
    }

    //release seeker mines
    public void Attack1()
    {
        ReleaseMines();
    }

    private void ReleaseMines()
    {
        for (int i = 0; i < _minesToFire; i++)
        {
            GameObject mineObject = EnemyManagerInstance.CreateEnemyFromSO(_seekerMineInfo);
            GameObject seekermine = Instantiate(mineObject, _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
            seekermine.transform.SetParent(null);
        }
    }

    //fire laser
    public void Attack2()
    {
        _beamAttack.ResetBeam();
        _beamAttack.lineRenderer.enabled = true;
        _beamAttack.EnableBeam();
    }

    protected override void Crash()
    {
        base.Crash();
        if (OnDied != null) OnDied?.Invoke();
    }
}
