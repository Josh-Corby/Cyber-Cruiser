using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class AnimatedPanelSoundController : SoundControllerBase
        {
            [SerializeField] private PanelClip[] _clips;

            [System.Serializable]
            private class PanelClip
            {
                public ClipInfo ClipInfo;
                public PanelAudioType AudioType;
            }

            private enum PanelAudioType
            {
                PanelUp,
                PanelDown
            }

            public void PlaySound(int clipIndex)
            {
                PlayNewClip(_clips[clipIndex].ClipInfo);
            }
        }
    }
}