using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnImageDisplay : MonoBehaviour
    {

        [SerializeField] private Image _image;

        private void OnEnable()
        {
            GameManager.OnMissionStart += ClearImage;
            Pickup.OnBossPickup += ChangeAddOnImage;
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= ClearImage;
            Pickup.OnBossPickup -= ChangeAddOnImage;
        }

        private void ClearImage()
        {
            _image.enabled = false;
        }

        private void ChangeAddOnImage(PickupInfo info)
        {
            _image.enabled = true;
            _image.sprite = info.Sprite;
        }
    }
}
