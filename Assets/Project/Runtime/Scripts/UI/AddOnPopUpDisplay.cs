using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnPopUpDisplay : MonoBehaviour
    {
        [SerializeField] private Image _popUpImage; 
        private void OnEnable()
        {
            Disable();
            Pickup.OnBossPickup += DisplayPopUp;
        }

        private void OnDisable()
        {
            Disable();
            Pickup.OnBossPickup -= DisplayPopUp;
        }

        private void DisplayPopUp(PickupInfo info)
        {
            Enable();
            _popUpImage.sprite = info.Popup;
            Invoke(nameof(Disable), 2f);
        }

        private void Enable()
        {
            _popUpImage.enabled = true;
        }

        private void Disable()
        {
            _popUpImage.sprite = null;
            _popUpImage.enabled = false;
        }
    }
}
