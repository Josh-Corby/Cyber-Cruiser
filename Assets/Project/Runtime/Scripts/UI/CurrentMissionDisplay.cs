using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace CyberCruiser
{
    public class CurrentMissionDisplay : GameBehaviour
    {
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private TMP_Text _missionDescription;
        [SerializeField] private TMP_Text _missionStatus;
        [SerializeField] private Image[] _missionImages;
        [SerializeField] private GameObject[] _missionStars;
        [SerializeField] private MissionScriptableObject _currentMission;

        [SerializeField] private List<Vector2> _starPositions = new List<Vector2>();

        private void Awake()
        {
            GetInitialPositions();         
        }

        private void OnEnable()
        {
            ClearMissionUI();
            AssignMissionUI();
        }

        private void OnDisable()
        {
            ResetStarPositions();
        }

        private void GetInitialPositions()
        {
            for (int i = 0; i < _missionStars.Length-1; i++)
            {
                _starPositions.Add(_missionStars[i].transform.position);
            }
        }

        public void ResetStarPositions()
        {
            for (int i = 0; i < _missionStars.Length-1; i++)
            {
                _missionStars[i].transform.position = _starPositions[i];
                _missionStars[i].SetActive(true);
            }
        }

        private void ClearMissionUI()
        {
            _missionDescription.text = "";

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
                _missionStatus.text = "Mission Complete";
            }

            else
            {
                _missionStatus.text = "Current Mission";
            }

            string text = _currentMission.missionDescription;
            string formattedText = text.Replace("{x}", _missionManager.MissionProgressLeft.ToString());
            _missionDescription.text = formattedText;

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