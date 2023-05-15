using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class UISoundController : SoundControllerBase
        {
            [SerializeField] private AudioClip _buttonClip;

            public void PlaySound(AudioClip clip)
            {
                PlayOneShot(clip);
            }
        }
    }
}