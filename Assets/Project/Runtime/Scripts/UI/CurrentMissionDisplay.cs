using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class CurrentMissionDisplay : GameBehaviour
    {
        [SerializeField] private TMP_Text _missionDescription;
        [SerializeField] private TMP_Text _missionProgress;
        [SerializeField] private Image[] _missionImages;
        [SerializeField] private Image[] _missionStars;
        private MissionScriptableObject _currentMission;

        private void OnEnable()
        {
            if (MissionManagerInstance.CurrentMission == null)
            {
                ClearMissionUI();
            }

            else
            {
                AssignMissionUI();
            }
        }

        private void ClearMissionUI()
        {
            _missionDescription.text = "";
            _missionProgress.text = "";

            for (int i = 0; i < _missionImages.Length; i++)
            {
                _missionImages[i].enabled = false;
            }

            for (int i = 0; i < _missionStars.Length; i++)
            {
                _missionStars[i].enabled = false;
            }

            _currentMission = null;
        }

        private void AssignMissionUI()
        {
            _currentMission = MissionManagerInstance.CurrentMission;

            _missionDescription.text = _currentMission.missionDescription;
            _missionProgress.text = MissionManagerInstance.CurrentMissionProgress.ToString();

            for (int i = 0; i < _missionImages.Length; i++)
            {
                _missionImages[i].sprite = _currentMission.missionIcon;
                _missionImages[i].enabled = true;
            }

            for (int i = 0; i < _currentMission.missionStarReward; i++)
            {
                _missionStars[i].enabled = true;
            }
        }
    }
}