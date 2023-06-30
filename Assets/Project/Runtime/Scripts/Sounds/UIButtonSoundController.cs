using UnityEngine;

namespace CyberCruiser
{
    namespace Audio
    {
        public class UIButtonSoundController : SoundControllerBase
        {
            [SerializeField] private ButtonClip[] _clips;

            [System.Serializable]
            private class ButtonClip
            {
                public ClipInfo ClipInfo;
                public ButtonAudioType AudioType;
            }

            private enum ButtonAudioType
            {
                ButtonHover,
                ButtonClick,
            }

            public void PlaySound(int clipIndex)
            {
                if(_audioSource.isPlaying) 
                    {
                        return;
                    } 
                PlayNewClip(_clips[clipIndex].ClipInfo);
            }
        }
    }
}