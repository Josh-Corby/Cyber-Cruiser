using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private MissionManager _missionManager;
        [SerializeField] private PlayerRankManager _playerRankManager;
        [SerializeField] private PlayerSaveManager _playerSaveManager;

        public void ClearSaveData()
        {
            _missionManager.ClearSaveData();
            _playerRankManager.ClearSaveData();
            _playerSaveManager.ClearSaveData();
        }
    }
}
