using CyberCruiser.Audio;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerSoundController : SoundControllerBase
    {
        [SerializeField] private UpgradeClip[] _upgradeClips;

        [System.Serializable]
        private class UpgradeClip
        {
            public ClipInfo ClipInfo;
            public UpgradeAudioType AudioType;
        }

        private enum UpgradeAudioType
        {
            UpgradeStart,
            UpgradeEnd
        }

        public void PlaySound(int clipIndex)
        {
            PlayNewClip(_upgradeClips[clipIndex].ClipInfo);
        }
    }
}
