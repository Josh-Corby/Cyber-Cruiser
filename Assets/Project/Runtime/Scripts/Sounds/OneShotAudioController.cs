using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class OneShotAudioController : SoundControllerBase
        {
            [HideInInspector] public AudioClip _oneShotClip;

            public override void PlayClip()
            {
                _audioSource.PlayOneShot(_oneShotClip);
            }
        }
    }
}