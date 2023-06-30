using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class PickupDisplayManager : MonoBehaviour
    {

        [SerializeField] private TMP_Text _pickupDisplayText;

        private void OnEnable()
        {
            _pickupDisplayText.enabled = false;
            Pickup.OnBossPickup += DisplayPickupDescription;
        }

        private void OnDisable()
        {
            _pickupDisplayText.enabled = false;
            Pickup.OnBossPickup -= DisplayPickupDescription;
        }

        private void DisplayPickupDescription(string pickupName)
        {
            _pickupDisplayText.enabled = true;
            _pickupDisplayText.text = pickupName + " Picked up";
            Invoke(nameof(DisableText), 2f);
        }

        private void DisableText()
        {
            _pickupDisplayText.text = "";
            _pickupDisplayText.enabled = false;
        }
    }
}
