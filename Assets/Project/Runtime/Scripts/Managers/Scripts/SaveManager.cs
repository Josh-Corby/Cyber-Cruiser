using System;
using UnityEngine;

namespace CyberCruiser
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private PlayerRankManager _playerRankManager;
        [SerializeField] private PlayerSaveManager _playerSaveManager;
        [SerializeField] private PlayerStatistics _playerStatistics;

        public static event Action OnClearSaveData = null;
        public void ClearSaveData()
        {
            _missionManager.ClearSaveData();
            _playerRankManager.ClearSaveData();
            _playerSaveManager.ClearSaveData();
            _playerStatistics.ClearSaveData();
            OnClearSaveData?.Invoke();
        }
    }
}
