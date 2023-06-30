using CyberCruiser.Audio;
using System.Xml.Serialization;
using UnityEngine;


namespace CyberCruiser
{
    [RequireComponent(typeof(AudioSource))]
    public class BeamAttack : GameBehaviour
    {
        private bool _isBeamActive;
        public bool IsBeamFiring;
        public LineRenderer lineRenderer;
        private float _beamSize;
        public float beamDuration;
        [SerializeField] private float _beamDamage;
        [SerializeField] private float _basicEnemyBeamDamage;

        [SerializeField] private float _beamSpeed;
        [SerializeField] bool _isBeamTimed;
        [SerializeField] private float _beamTimer;
        [SerializeField] private LayerMask _beamCollisionMask;
        [SerializeField] private float _beamWidth;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SoundControllerBase _beamSoundController;
        [SerializeField] private ClipInfo _beamClip;

        [SerializeField] private BoolReference _isGamePausedReference;
        private void Awake()
        {
            _beamSoundController = GetComponent<SoundControllerBase>();
        }

        public bool IsBeamActive { get => _isBeamActive; }

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
            if (_isGamePausedReference.Value != true)
            {
                if (IsBeamActive)
                {
                    if (IsBeamFiring)
                    {
                        ExtendBeam();
                    }
                }
            }
        }

        public void EnableBeam()
        {
            if (IsBeamActive == false)
            {
                _isBeamActive = true;
                _beamSoundController.PlayNewClip(_beamClip);
            }
        }

        public void DisableBeam()
        {
            IsBeamFiring = false;
            _isBeamActive = false;
            lineRenderer.enabled = false;
            _audioSource.Stop();
        }

        public void StartFiring()
        {
            IsBeamFiring = true;
            _beamSoundController.PlayNewClip(_beamClip);
        }

        public void StopFiring()
        {
            IsBeamFiring = false;
            _audioSource.Stop();
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
                    _isBeamActive = false;
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
            _audioSource.Stop();
        }

        private void BeamCollision()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 180, transform.right, GetDistanceXBetweenPoints(), _beamCollisionMask);
            if (hit.collider != null)
            {

                if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.GetComponent<IDamageable>().Damage(_basicEnemyBeamDamage);
                }

                else if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.Damage(_beamDamage);
                }

                if (hit.collider.TryGetComponent<Shield>(out var shield))
                {
                    //Debug.Log("beam hit shield");
                    shield._shieldController.ReduceShields(_beamDamage);
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
}