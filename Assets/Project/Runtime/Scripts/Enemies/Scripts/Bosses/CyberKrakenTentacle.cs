using CyberCruiser.Audio;
using UnityEngine;

namespace CyberCruiser
{
    public class CyberKrakenTentacle : GameBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float _damage;
        [SerializeField] protected EnemyScriptableObject _owner;


        [SerializeField] protected Vector2 spawnPosition;
        [SerializeField] protected bool _moveForward;

        [SerializeField] protected bool _isWaiting;
        [SerializeField] protected float _waitTime;
        [SerializeField] protected float _waitTimer;

        private BoxCollider2D _col;

        [SerializeField] private BoolReference _isTimeStopped;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] protected SoundControllerBase _soundController;
        [SerializeField] protected ClipInfo _tentacleClip;

        protected void Awake()
        {
            spawnPosition = transform.parent.position;
            _moveForward = true;
            _isWaiting = false;
            _col = GetComponent<BoxCollider2D>();
        }

        protected void OnEnable()
        {
            _soundController.PlayNewClip(_tentacleClip);
        }

        private void Update()
        {
            if (_isTimeStopped.Value)
            {
                return;
            }

            TentacleMovement();
        }

        protected virtual void TentacleMovement()
        {
            if (_isWaiting)
            {
                WaitTimer();
                return;
            }
            if (_moveForward)
            {
                MoveForward();
            }
            else
            {
                MoveBackward();
            }
        }

        protected void MoveForward()
        {
            transform.parent.position += transform.right * speed * Time.deltaTime;
            if (Vector2.Distance(transform.parent.position, spawnPosition) > _col.size.x)
            {
                StartWaiting();
                _moveForward = false;
              
            }
        }

        private void MoveBackward()
        {
            transform.parent.position -= transform.right * speed * Time.deltaTime;

            if (Vector2.Distance(transform.parent.position, spawnPosition) < 0.5f)
            {
                Destroy(transform.parent.gameObject);
            }
        }

        protected void StartWaiting()
        {
            Debug.Log("Start waiting");
            _isWaiting = true;
            _waitTimer = _waitTime;
        }

        protected void WaitTimer()
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
            {
                _isWaiting = false;
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            ProcessCollision(collision.gameObject);
        }

        protected virtual void ProcessCollision(GameObject collider)
        {
            if(collider.TryGetComponent<PlayerManager>(out var player))
            {
                player.Damage(_damage, _owner);
            }
        }
    }
}