using System;
using UnityEngine;

namespace CyberCruiser
{
    public class SaveManager : MonoBehaviour
    {
        public static event Action OnClearSaveData = null;
        public static event Action OnSaveData = null;

        public void SaveData()
        {
            OnSaveData?.Invoke();
        }

        public void ClearSaveData()
        {
            OnClearSaveData?.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
            if(!focus) SaveData();
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }
    }
}
