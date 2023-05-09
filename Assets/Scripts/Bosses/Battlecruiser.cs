using System;
using System.Collections;
using UnityEngine;

public class Battlecruiser : Boss, IBoss
{
    [SerializeField] private GameObject _mineReleasePoint;
    [SerializeField] private EnemyScriptableObject _seekerMineInfo;
    private readonly int _minesToFire = 2;
    private readonly int _mineDelay = 1;

    private float _beamAttackDuration;
    private readonly float _timeAfterAttackFinish = 1f;

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

    //release seeker mines
    private IEnumerator ReleaseMines()
    {
        _attackTimer = _timeAfterAttackFinish;
        for (int i = 0; i < _minesToFire; i++)
        {
            GameObject mineObject = EM.CreateEnemyFromSO(_seekerMineInfo);
            GameObject seekermine = Instantiate(mineObject, _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
            seekermine.transform.SetParent(null);
            yield return new WaitForSeconds(_mineDelay);
        }
        StopCoroutine(ReleaseMines());

    }

    public void Attack1()
    {
        StartCoroutine(ReleaseMines());
    }

    //fire laser
    public void Attack2()
    {
        _beamAttack.ResetBeam();
        _beamAttack.lineRenderer.enabled = true;
        _attackTimer = _beamAttackDuration + _timeAfterAttackFinish;
        _beamAttack.isBeamActive = true;
    }

    protected override void Crash()
    {
        base.Crash();
        if (OnDied != null) OnDied?.Invoke();
    }
}
