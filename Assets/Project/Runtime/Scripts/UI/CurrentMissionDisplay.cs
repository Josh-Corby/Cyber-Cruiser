using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class CurrentMissionDisplay : GameBehaviour
    {
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private TMP_Text _missionDescription;
        [SerializeField] private TMP_Text _missionProgress;
        [SerializeField] private TMP_Text _missionStatus;
        [SerializeField] private Image[] _missionImages;
        [SerializeField] private GameObject[] _missionStars;
        [SerializeField] private MissionScriptableObject _currentMission;

        private void OnEnable()
        {
            ClearMissionUI();
            AssignMissionUI();
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
                _missionStars[i].SetActive(false);
            }
        }

        private void AssignMissionUI()
        {
            _currentMission = _missionManager.MissionBeforeLevelStart;

            if(_currentMission == null)
            {
                return;
            }
            if (_currentMission.isComplete)
            {
                _missionProgress.text = "";
                _missionStatus.text = "Mission Complete";
                return;
            }

            else
            {
                _missionStatus.text = "Current Mission";
                _missionProgress.text = _missionManager.MissionProgressLeft.ToString() + " left";
            }

            _missionDescription.text = _currentMission.missionDescription;

            for (int i = 0; i < _missionImages.Length; i++)
            {
                _missionImages[i].sprite = _currentMission.missionIcon;
                _missionImages[i].enabled = true;
            }

            for (int i = 0; i < _currentMission.missionStarReward; i++)
            {
                _missionStars[i].SetActive(true);
            }
        }
    }
}