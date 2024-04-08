using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class AddOnImageDisplay : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _addOnImages = new List<GameObject>();
        private int _currentAddOnIndex = 0;
        private float _addOnOffset = 20;

        [SerializeField] private GameObject _addOnImageBase;
        [SerializeField] private Image _image;

        private void OnEnable()
        {
            GameManager.OnMissionStart += ResetUI;
            Pickup.OnBossPickup += CreateNewAddOnImage;
        }

        private void OnDisable()
        {
            GameManager.OnMissionStart -= ResetUI;
            Pickup.OnBossPickup -= CreateNewAddOnImage;
        }

        private void ClearImage()
        {
            _image.enabled = false;
        }

        private void ResetUI()
        {
            _currentAddOnIndex = 0;

            for(int i = _addOnImages.Count - 1; i >= 0; i--)
            {
                Destroy(_addOnImages[i]);
                _addOnImages.Remove(_addOnImages[i]);
            }
        }

        private void ChangeAddOnImage(PickupInfo info)
        {
            _image.enabled = true;
            _image.sprite = info.Sprite;
        }

        private void CreateNewAddOnImage(PickupInfo info)
        {
            GameObject newAddOn = Instantiate(_addOnImageBase, transform.position, Quaternion.identity, transform);
            RectTransform imageRect = newAddOn.GetComponent<RectTransform>();
            imageRect.localPosition += new Vector3(0, _currentAddOnIndex * _addOnOffset, 0);
            newAddOn.GetComponent<Image>().sprite = info.Sprite;

            _currentAddOnIndex += 1;
            _addOnImages.Add(newAddOn);
        }
    }
}
