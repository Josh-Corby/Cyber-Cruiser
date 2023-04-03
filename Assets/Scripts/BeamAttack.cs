using System;
using UnityEngine;


public class BeamAttack : MonoBehaviour
{
    public bool isBeamActive;
    public LineRenderer lineRenderer;
    private float _beamSize;
    public float beamDuration;

    [SerializeField] private float _beamSpeed;
    [SerializeField] bool _isBeamTimed;
    [SerializeField] private float _beamTimer;
    [SerializeField] private LayerMask _beamCollisionMask;
    [SerializeField] private float _beamWidth;

    private float tempx;
    private void Start()
    {
        lineRenderer.startWidth = _beamWidth;
        lineRenderer.endWidth = _beamWidth;
    }
    private void Update()
    {
        if (isBeamActive)
        {
            ExtendBeam();
        }
    }

    public void ExtendBeam()
    {
        if (_isBeamTimed)
        {
            _beamTimer -= Time.deltaTime;

            if (_beamTimer > 0)
            {
                _beamSize += _beamSpeed * Time.deltaTime;
                Vector3 targetPosition = transform.localPosition + transform.right * _beamSize;
                //need to store potential negative values of x for raycasting
                tempx = targetPosition.x;
                lineRenderer.SetPosition(1, new Vector3(Mathf.Abs(targetPosition.x), targetPosition.y));
                //lineRenderer.SetPosition(1, new Vector3(Mathf.Abs(lineRenderer.GetPosition(1).x), lineRenderer.GetPosition(1).y));


                BeamCollision();
            }
            if (_beamTimer <= 0)
            {
                lineRenderer.enabled = false;
                isBeamActive = false;
            }
        }
        else
        {
            _beamSize += _beamSpeed * Time.deltaTime;
            lineRenderer.SetPosition(1, transform.localPosition + transform.right * _beamSize);
            BeamCollision();
        }
    }

    public void ResetBeam()
    {
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, transform.localPosition);
        _beamSize = 0;
        _beamTimer = beamDuration;
    }

    private void BeamCollision()
    {
        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, _beamWidth / 2, GetDirectionBetweenPoints(), GetDistanceXBetweenPoints(), _beamCollisionMask);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 180, transform.right, GetDistanceXBetweenPoints(), _beamCollisionMask);
        if (hit.collider != null)
        {
            Debug.Log("Enemy hit");
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
        Vector2 directionBetweenPoints = new Vector2(tempx, GetPointWorldPosition(1).y) - GetPointWorldPosition(0).normalized;
        directionBetweenPoints.x = -Mathf.Abs(directionBetweenPoints.x);
        Debug.Log(directionBetweenPoints);
        return directionBetweenPoints;
    }

    private float GetDistanceXBetweenPoints()
    {
        float distanceBetweenPoints = lineRenderer.GetPosition(0).x + Mathf.Abs(lineRenderer.GetPosition(1).x);
            //GetPointWorldPosition(0).x + Mathf.Abs(GetPointWorldPosition(1).x);
        Debug.Log(distanceBetweenPoints);
        return distanceBetweenPoints;
    }

}
