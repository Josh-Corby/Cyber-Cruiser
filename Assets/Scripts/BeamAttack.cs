using UnityEngine;


public class BeamAttack : MonoBehaviour
{
    public bool isBeamActive;
    public LineRenderer lineRenderer;
    private float _beamSize;
    public float beamDuration;
    [SerializeField] private float beamDamage;

    [SerializeField] private float _beamSpeed;
    [SerializeField] bool _isBeamTimed;
    [SerializeField] private float _beamTimer;
    [SerializeField] private LayerMask _beamCollisionMask;
    [SerializeField] private float _beamWidth;


    private void OnDisable()
    {
        ResetBeam();
    }

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
        else
        {
            ResetBeam();
        }
    }

    public void ExtendBeam()
    {
        if (lineRenderer.enabled == false)
        {
            lineRenderer.enabled = true;
        }

        if (_isBeamTimed)
        {
            _beamTimer -= Time.deltaTime;

            if (_beamTimer > 0)
            {
                _beamSize += _beamSpeed * Time.deltaTime;
                Vector3 targetPosition = Vector3.zero + transform.right * _beamSize;
                lineRenderer.SetPosition(1, new Vector3(Mathf.Abs(targetPosition.x), targetPosition.y));

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
            lineRenderer.SetPosition(1, Vector3.zero + transform.right * _beamSize);
            BeamCollision();
        }
    }

    public void ResetBeam()
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        _beamSize = 0;
        _beamTimer = beamDuration;
        lineRenderer.enabled = false;
    }

    private void BeamCollision()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 180, transform.right, GetDistanceXBetweenPoints(), _beamCollisionMask);
        if (hit.collider != null)
        {
            //Debug.Log("Enemy hit");
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(beamDamage);
            }

            if (hit.collider.TryGetComponent<Shield>(out var shield))
            {
                //Debug.Log("beam hit shield");
                shield._shieldController.ReduceShields(beamDamage);
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
        Vector2 directionBetweenPoints = GetPointWorldPosition(1) - GetPointWorldPosition(0).normalized;
        directionBetweenPoints.x = -Mathf.Abs(directionBetweenPoints.x);
        //Debug.Log(directionBetweenPoints);
        return directionBetweenPoints;
    }

    private float GetDistanceXBetweenPoints()
    {
        float distanceBetweenPoints = lineRenderer.GetPosition(0).x + Mathf.Abs(lineRenderer.GetPosition(1).x);
        distanceBetweenPoints *= transform.parent.localScale.x;
        //Debug.Log(distanceBetweenPoints);
        return distanceBetweenPoints;
    }
}
