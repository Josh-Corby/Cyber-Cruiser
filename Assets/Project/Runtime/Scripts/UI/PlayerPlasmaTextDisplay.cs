using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlayerPlasmaTextDisplay : GameBehaviour
    {
        private TMP_Text _plasmaText;

        private void Awake()
        {
            _plasmaText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            PlayerSaveManager.OnPlasmaChange += UpdatePlasmaText;
            UpdatePlasmaText(PlayerStatsManagerInstance.PlayerPlasma);
        }

        private void OnDisable()
        {
            PlayerSaveManager.OnPlasmaChange -= UpdatePlasmaText;
        }

        private void UpdatePlasmaText(int plasma)
        {
            _plasmaText.text = plasma.ToString();
        }
    }
}