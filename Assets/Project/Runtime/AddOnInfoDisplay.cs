using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnInfoDisplay : MonoBehaviour
    {
        [SerializeField] private PickUpScriptableObject _addonInfo;
        [SerializeField] private Image _addonImageDisplay;
        [SerializeField] private Sprite _unknownSprite;
        [SerializeField] private TMP_Text _addonNameDisplay;
        [SerializeField] private TMP_Text _addonDescriptionDisplay;
        [SerializeField] private TMP_Text _numberOfEquipsDisplay;

        private void OnEnable()
        {
            if (_addonInfo.TimesPickedUp == 0)
            {
                _addonImageDisplay.sprite = _unknownSprite;
                _addonNameDisplay.text = "UNKNOWN";
                _addonDescriptionDisplay.text = "";
                _numberOfEquipsDisplay.text = "";
            }
            else
            {
                _addonImageDisplay.sprite = _addonInfo.Info.Sprite;
                _addonNameDisplay.text = _addonInfo.Info.Name;
                _addonDescriptionDisplay.text = _addonInfo.Info.Description;
                _numberOfEquipsDisplay.text = _addonInfo.TimesPickedUp.ToString();
            }
        }
    }
}
