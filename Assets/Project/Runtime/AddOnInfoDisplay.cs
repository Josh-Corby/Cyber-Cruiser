using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnInfoDisplay : MonoBehaviour
    {
        [SerializeField] private PickUpScriptableObject _addonInfo;
        [SerializeField] private Image _addonImageDisplay;
        [SerializeField] private TMP_Text _addonNameDisplay;
        [SerializeField] private TMP_Text _addonDescriptionDisplay;
        [SerializeField] private TMP_Text _numberOfEquipsDisplay;

        private void Awake()
        {
            _addonImageDisplay.sprite = _addonInfo.Info.Sprite;
            _addonNameDisplay.text = _addonInfo.Info.Name;
            _addonDescriptionDisplay.text = _addonInfo.Info.Description;
        }

        private void OnEnable()
        {
            _numberOfEquipsDisplay.text = _addonInfo.TimesPickedUp.ToString();
        }
    }
}
