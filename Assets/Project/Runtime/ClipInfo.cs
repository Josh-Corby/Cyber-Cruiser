using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        [System.Serializable]
        public class ClipInfo
        {
            public AudioClip Clip;
            public bool OverrideSourceVolume;
            [Range(0f, 1f)] public float Volume = 1;
            public bool PlayOneShot;
        }
    }
}