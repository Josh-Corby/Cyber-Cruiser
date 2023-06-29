using System;
using TowerDefenseGame;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerSaveManager : GameBehaviour<PlayerSaveManager>
    {
        private const string PLAYER_PLASMA = "PlayerPlasma";
        private const string PLAYER_ION = "PlayerIon";


        //these values are scriptable object references that are edited both here and in the playermanager 
        [SerializeField] private IntValue _playerPlasma;
        [SerializeField] private IntValue _playerIon;
        private int PlayerPlasma { get => _playerPlasma.Value; set => _playerPlasma.Value = value; }

        private int PlayerIon
        {
            get => _playerIon.Value;
            set
            {
                value = value < 0 ? 0 : value;
                _playerIon.Value = value;
                OnIonChange?.Invoke(PlayerIon);
            }
        }

        public static event Action<int> OnIonChange = null;


        private void OnEnable()
        {
            GameManager.OnSaveDataCleared += ClearSaveData;
            RestoreValues();
        }

        private void OnDisable()
        {
            GameManager.OnSaveDataCleared -= ClearSaveData;
        }

        private void RestoreValues()
        {
            _playerIon.Value = PlayerPrefs.GetInt(PLAYER_ION);
            PlayerPlasma = PlayerPrefs.GetInt(PLAYER_PLASMA);
        }

        public void ChangeIon(int value)
        {
            _playerIon.Value += value;
            _playerIon.Value = ValidateValue(_playerIon.Value);
            OnIonChange?.Invoke(_playerIon.Value);
        }

        private int ValidateValue(int value)
        {
            value = value < 0 ? 0 : value;
            return value;
        }

        private void SaveValues()
        {
            PlayerPrefs.SetInt(PLAYER_PLASMA, PlayerPlasma);
            PlayerPrefs.SetInt(PLAYER_ION, _playerIon.Value);
        }

        private void ClearSaveData()
        {
            PlayerPlasma = 0;
            _playerIon.Value = 0;
        }

        private void OnApplicationQuit()
        {
            SaveValues();
        }
    }
}