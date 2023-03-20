using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlecruiser : Boss, IBoss
{
    [SerializeField] private GameObject mineReleasePoint;
    [SerializeField] private GameObject seekerMinePrefab;
    [SerializeField] private int minesToFire;
    private int mineDelay = 1;

    private float beamAttackDuration;
    private float mineAttackDuration = 0f;
    private float timeAfterAttackFinish = 2f;

    [SerializeField] private GameObject pulverizerBeam;
    [SerializeField] private BeamAttack beamAttack;

    protected override void Awake()
    {
        base.Awake();
        beamAttack = pulverizerBeam.GetComponent<BeamAttack>();
        beamAttackDuration = beamAttack.beamDuration;
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
        attackTimer = mineAttackDuration + timeAfterAttackFinish;
        for (int i = 0; i < minesToFire; i++)
        {
            GameObject seekerMine = Instantiate(seekerMinePrefab, mineReleasePoint.transform.position, mineReleasePoint.transform.rotation);
            seekerMine.transform.SetParent(null);
            yield return new WaitForSeconds(mineDelay);
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
        beamAttack.ResetBeam();
        beamAttack.lineRenderer.enabled = true;
        attackTimer = beamAttackDuration + timeAfterAttackFinish;
    }
}
