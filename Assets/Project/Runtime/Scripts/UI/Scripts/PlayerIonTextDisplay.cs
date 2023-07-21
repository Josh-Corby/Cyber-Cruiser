using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlayerIonTextDisplay : GameBehaviour
    {
        private TMP_Text _ionTMPText;
        [SerializeField] private IntReference _playerIonReference;

        private void Awake()
        {
            _ionTMPText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            PlayerSaveManager.OnIonChange += UpdateIonText;
            UpdateIonText(_playerIonReference.Value);
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