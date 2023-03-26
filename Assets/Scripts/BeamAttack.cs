using System;
using UnityEngine;


public class BeamAttack : MonoBehaviour
{

    public LineRenderer lineRenderer;
    private float beamSize;
    [SerializeField] private float beamSpeed;

    public float beamDuration;
    [SerializeField] private float beamTimer;
    [SerializeField] private LayerMask beamCollisionMask;


    private void Update()
    {
        beamTimer -= Time.deltaTime;

        if (beamTimer > 0)
        {
            beamSize += beamSpeed * Time.deltaTime;
            lineRenderer.SetPosition(1, transform.localPosition + transform.right * beamSize);
            BeamCollision();
        }
        if (beamTimer <= 0)
        {
            lineRenderer.enabled = false;
        }
    }

    public void ResetBeam()
    {
        lineRenderer.SetPosition(0, transform.localPosition);
        lineRenderer.SetPosition(1, transform.localPosition);
        beamSize = 0;
        beamTimer = beamDuration;
    }

    private void BeamCollision()
    {
        RaycastHit2D hit = Physics2D.BoxCast(GetPointWorldPosition(0), new Vector2(1, 1), 0, GetDirectionBetweenPoints(), GetDistanceXBetweenPoints(), beamCollisionMask);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(1);
            }
        }
    }

    private Vector2 GetPointWorldPosition(int pointIndex)
    {
        Vector2 localPosition = lineRenderer.GetPosition(pointIndex);
        Vector2 worldPosition = lineRenderer.transform.TransformPoint(localPosition);

        return worldPosition;
    }
    private Vector2 GetDirectionBetweenPoints()
    {
        Vector2 directionBetweenPoints = GetPointWorldPosition(1) - GetPointWorldPosition(0);
        return directionBetweenPoints;
    }

    private float GetDistanceXBetweenPoints()
    {
        float distanceBetweenPoints = GetPointWorldPosition(0).x - GetPointWorldPosition(1).x;
        //Debug.Log(distanceBetweenPoints);
        return distanceBetweenPoints;
    }

}
