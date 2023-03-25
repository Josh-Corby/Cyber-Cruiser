using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlecruiser : Boss, IBoss
{
    [SerializeField] private GameObject _mineReleasePoint;
    [SerializeField] private GameObject _seekerMinePrefab;
    [SerializeField] private int _minesToFire;
    private int _mineDelay = 1;

    private float _beamAttackDuration;
    private float _mineAttackDuration = 0f;
    private float _timeAfterAttackFinish = 2f;

    [SerializeField] private GameObject _pulverizerBeam;
    [SerializeField] private BeamAttack _beamAttack;

    protected override void Awake()
    {
        base.Awake();
        _beamAttack = _pulverizerBeam.GetComponent<BeamAttack>();
        _beamAttackDuration = _beamAttack.beamDuration;
    }

    protected override void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attackTimer <= 0)
        {
            ChooseRandomAttack();
        }
    }

    //release seeker mines
    private IEnumerator ReleaseMines()
    {
        attackTimer = _mineAttackDuration + _timeAfterAttackFinish;
        for (int i = 0; i < _minesToFire; i++)
        {
            GameObject seekerMine = Instantiate(_seekerMinePrefab, _mineReleasePoint.transform.position, _mineReleasePoint.transform.rotation);
            seekerMine.transform.SetParent(null);
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
        attackTimer = _beamAttackDuration + _timeAfterAttackFinish;
    }
}
