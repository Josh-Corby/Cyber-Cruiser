using CyberCruiser.Audio;
using System;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(SoundControllerBase))]
    public class PlayerDeathTrigger : MonoBehaviour
    {
        public static event Action OnPlayerDeadOffScreen = null;
        private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _deathClip;

        private void Awake()
        {
            _soundController = GetComponent<SoundControllerBase>(); 
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerManager>())
            {
                Debug.Log("Player off screen");
                OnPlayerDeadOffScreen?.Invoke();
                _soundController.PlayNewClip(_deathClip);
            }
        }
    }
}
