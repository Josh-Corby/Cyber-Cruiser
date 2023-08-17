using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class WeaponInfoDisplay : MonoBehaviour
    {
        [SerializeField] private WeaponSO _weaponInfo;
        [SerializeField] private Image _weaponImage;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponDescription;
        [SerializeField] private TMP_Text _equips;

        private void Awake()
        {
            _weaponImage.sprite = _weaponInfo.WeaponUISprite;
            _weaponName.text = _weaponInfo.WeaponName;
            _weaponDescription.text = _weaponInfo.WeaponDescription;
        }
    }
}
