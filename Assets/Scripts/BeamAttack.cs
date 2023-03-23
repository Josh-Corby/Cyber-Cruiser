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


            //BeamCollision();
        }
        if(beamTimer <= 0)
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
        RaycastHit2D hit = Physics2D.BoxCast(lineRenderer.GetPosition(0), new Vector2 (GetDistanceBetweenPoints(),1), 90, GetDirectionBetweenPoints(), GetDistanceBetweenPoints(),beamCollisionMask);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }
    }
    private void GetPointPosition(int pointIndex)
    {
        Vector2 pointPosition = new Vector2( transform.parent.position.x + lineRenderer.GetPosition(pointIndex).x, transform.parent.position.y + lineRenderer.GetPosition(pointIndex).y);
    }
    private Vector2 GetDirectionBetweenPoints()
    {
        Vector2 directionBetweenPoints = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
        return directionBetweenPoints.normalized;
    }

    private float GetDistanceBetweenPoints()
    {
        float distanceBetweenPoints = lineRenderer.GetPosition(0).y - lineRenderer.GetPosition(1).y;
        //Debug.Log(distanceBetweenPoints);
        return distanceBetweenPoints;
    }

}
