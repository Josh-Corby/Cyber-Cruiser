using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnPopUpDisplay : MonoBehaviour
    {

        [SerializeField] private TMP_Text _addOnNameText;
        [SerializeField] private TMP_Text _addOnDescriptionText;
        [SerializeField] private Image _addOnImage; 
        private void OnEnable()
        {
            Pickup.OnBossPickup += DisplayPickupDescription;
        }

        private void OnDisable()
        {
            DisableText();
            Pickup.OnBossPickup -= DisplayPickupDescription;
        }

        private void DisplayPickupDescription(PickupInfo info)
        {
            EnableText();
            _addOnNameText.text = info.Name;
            _addOnDescriptionText.text = info.Description;
            _addOnImage.sprite = info.Sprite;
            Invoke(nameof(DisableText), 2f);
        }

        private void EnableText()
        {
            _addOnNameText.enabled = true;
            _addOnDescriptionText.enabled = true;
            _addOnImage.enabled = true;
        }

        private void DisableText()
        {
            _addOnNameText.text = "";
            _addOnDescriptionText.text = "";
            _addOnImage.sprite = null;

            _addOnNameText.enabled = false;
            _addOnDescriptionText.enabled = false;
            _addOnImage.enabled = false;
        }
    }
}
