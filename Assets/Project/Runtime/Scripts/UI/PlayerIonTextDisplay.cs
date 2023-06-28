using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlayerIonTextDisplay : GameBehaviour
    {
        private TMP_Text _ionTMPText;

        private void Awake()
        {
            _ionTMPText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            PlayerSaveManager.OnIonChange += UpdateIonText;
            UpdateIonText(PlayerStatsManagerInstance.PlayerIon);
        }

        private void OnDisable()
        {
            PlayerSaveManager.OnIonChange -= UpdateIonText;
        }

        private void UpdateIonText(int ion)
        {
            _ionTMPText.text = ion.ToString();
        }
    }
}