using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlayerPlasmaTextDisplay : GameBehaviour
    {
        private TMP_Text _plasmaText;
        [SerializeField] private IntReference _playerPlasmaReference;

        private void Awake()
        {
            _plasmaText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            PlayerManager.OnPlasmaChange += UpdatePlasmaText;
            UpdatePlasmaText(_playerPlasmaReference.Value);
        }

        private void OnDisable()
        {
            PlayerManager.OnPlasmaChange -= UpdatePlasmaText;
        }

        private void UpdatePlasmaText(int plasma)
        {
            _plasmaText.text = plasma.ToString();
        }
    }
}