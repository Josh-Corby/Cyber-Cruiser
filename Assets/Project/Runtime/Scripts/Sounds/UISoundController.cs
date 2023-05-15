using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class UISoundController : SoundControllerBase
        {
            [SerializeField] private AudioClip _buttonClip;

            public void OnButtonClick()
            {
                PlayClip();
            }
        }
    }
}