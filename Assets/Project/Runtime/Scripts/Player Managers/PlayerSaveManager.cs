using System;
using UnityEngine;

namespace CyberCruiser
{
    public class PlayerSaveManager : GameBehaviour<PlayerSaveManager>
    {
        private const string PLAYER_PLASMA = "PlayerPlasma";
        private const string PLAYER_ION = "PlayerIon";

        [SerializeField] private IntValue _playerPwlasma;
        [SerializeField] private IntValue _playerIon;
        private int _playerPlasma;

        public int PlayerPlasma { get => _playerPlasma; }

        public static event Action<int> OnIonChange = null;
        public static event Action<int> OnPlasmaChange = null;

        private void OnEnable()
        {
            GameManager.OnSaveDataCleared += ClearSaveData;
            PlayerManager.OnIonPickup += ChangeIon;
            PlayerManager.OnPlasmaChange += SetPlasma;
            RestoreValues();
        }

        private void OnDisable()
        {
            GameManager.OnSaveDataCleared -= ClearSaveData;
            PlayerManager.OnIonPickup -= ChangeIon;
            PlayerManager.OnPlasmaChange -= SetPlasma;
        }

        private void RestoreValues()
        {
            _playerIon.Value = PlayerPrefs.GetInt(PLAYER_ION);
            _playerPlasma = PlayerPrefs.GetInt(PLAYER_PLASMA);
        }

        public void ChangeIon(int value)
        {
            _playerIon.Value += value;
            _playerIon.Value = ValidateValue(_playerIon.Value);
            OnIonChange?.Invoke(_playerIon.Value);
        }

        public void ChangePlasma(int value)
        {
            _playerPlasma += value;
            _playerPlasma = ValidateValue(_playerPlasma);
            OnPlasmaChange?.Invoke(_playerPlasma);
        }

        public void SetPlasma(int value)
        {
            _playerPlasma = value;
            _playerPlasma = ValidateValue(PlayerPlasma);
            OnPlasmaChange?.Invoke(_playerPlasma);
        }

        private int ValidateValue(int value)
        {
            value = value < 0 ? 0 : value;
            return value;
        }

        private void SaveValues()
        {
            PlayerPrefs.SetInt(PLAYER_PLASMA, _playerPlasma);
            PlayerPrefs.SetInt(PLAYER_ION, _playerIon.Value);
        }

        private void ClearSaveData()
        {
            _playerPlasma = 0;
            _playerIon.Value = 0;
        }

        private void OnApplicationQuit()
        {
            SaveValues();
        }
    }
}