using CyberCruiser.Audio;
using System.Collections;
using UnityEngine;


namespace CyberCruiser
{
    [RequireComponent(typeof(AudioSource))]
    public class BeamAttack : GameBehaviour
    {
        [SerializeField] private bool _isBeamActive;
        public bool IsBeamFiring;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _beamDamage;
        [SerializeField] private float _basicEnemyBeamDamage;
        [SerializeField] private float _beamSpeed;
        [SerializeField] private float _beamWidth;
        [SerializeField] private LayerMask _beamCollisionMask;
        private float _currentBeamLength;

        [SerializeField] bool _isBeamTimed;
        [SerializeField] private float beamDuration;
        private float _beamTimer;

        [SerializeField] private AudioSource _audioSource;
        private SoundControllerBase _beamSoundController;
        [SerializeField] private ClipInfo _beamClip;

        [SerializeField] private BoolReference _isGamePausedReference;

        public EnemyScriptableObject _owner = null;

        private Coroutine _fadeOutCoroutine = null;

        private void Awake()
        {
            _lineRenderer = GetComponentInChildren<LineRenderer>();
            _beamSoundController = GetComponent<SoundControllerBase>();
            _audioSource = GetComponent<AudioSource>();
            _beamSoundController = GetComponent<SoundControllerBase>();
        }

        public bool IsBeamActive { get => _isBeamActive; }

        private void OnDisable()
        {
            ResetBeam();
        }

        private void Start()
        {
            _lineRenderer.startWidth = _beamWidth;
            _lineRenderer.endWidth = _beamWidth;
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
                _lineRenderer.enabled = true;
            }
        }

        public void StartFiring()
        {
            IsBeamFiring = true;

            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }
            _audioSource.volume = 1.0f;
            _beamSoundController.PlayNewClip(_beamClip);
        }

        public void StopFiring()
        {
            IsBeamFiring = false;
            _isBeamActive = false;
            _lineRenderer.enabled = false;


            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }

            _fadeOutCoroutine = StartCoroutine(VolumeFadeCoroutine());
        }

        public void ExtendBeam()
        {
            if (_lineRenderer.enabled == false)
            {
                _lineRenderer.enabled = true;
            }

            if (_isBeamTimed)
            {
                _beamTimer -= Time.deltaTime;

                if (_beamTimer > 0)
                {
                    _currentBeamLength += _beamSpeed * Time.deltaTime;
                    Vector3 targetPosition = Vector3.zero + transform.right * _currentBeamLength;
                    _lineRenderer.SetPosition(1, new Vector3(Mathf.Abs(targetPosition.x), targetPosition.y));

                    BeamCollision();
                }

                if (_beamTimer <= 0)
                {
                    StopFiring();                
                }
            }

            else
            {
                _currentBeamLength += _beamSpeed * Time.deltaTime;
                _lineRenderer.SetPosition(1, Vector3.zero + transform.right * _currentBeamLength);
                BeamCollision();
            }
        }

        public void ResetBeam()
        {
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);
            _currentBeamLength = 0;
            _beamTimer = beamDuration;
            _lineRenderer.enabled = false;

            if (_fadeOutCoroutine != null)
            {
                StopCoroutine(_fadeOutCoroutine);
            }
        }

        private IEnumerator VolumeFadeCoroutine()
        {
            while (_audioSource.volume >0)
            {
                _audioSource.volume -= 0.1f;
                yield return new WaitForEndOfFrame();
            }

            _audioSource.Stop();
        }

        private void BeamCollision()
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 180, transform.right, GetDistanceXBetweenPoints(), _beamCollisionMask);
            if (hit.collider != null)
            {

                if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.GetComponent<IDamageable>().Damage(_basicEnemyBeamDamage, null);
                }

                else if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.Damage(_beamDamage, _owner);
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
            Vector2 localPosition = _lineRenderer.GetPosition(pointIndex);
            Vector2 worldPosition = _lineRenderer.transform.TransformPoint(localPosition);

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
            float distanceBetweenPoints = _lineRenderer.GetPosition(0).x + Mathf.Abs(_lineRenderer.GetPosition(1).x);
            distanceBetweenPoints *= transform.parent.localScale.x;
            //Debug.Log(distanceBetweenPoints);
            return distanceBetweenPoints;
        }
    }
}