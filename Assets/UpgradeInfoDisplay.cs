using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class UpgradeInfoDisplay : MonoBehaviour
    {
        [SerializeField] private PickupInfo _upgradeInfo;
        [SerializeField] private Image _upgradeImage;
        [SerializeField] private TMP_Text _upgradeName;
        [SerializeField] private TMP_Text _upgradeDescription;
        [SerializeField] private TMP_Text _equips;

        private void Awake()
        {
            _upgradeImage.sprite = _upgradeInfo.Sprite;
            _upgradeName.text = _upgradeInfo.Name;
            _upgradeDescription.text = _upgradeInfo.Description;
        }
    }
}
