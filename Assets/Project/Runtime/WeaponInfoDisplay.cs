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
        [SerializeField] private Sprite _unknownSprite;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponDescription;
        [SerializeField] private TMP_Text _equips;


        private void OnEnable()
        {
            if (_weaponInfo.Equips == 0)
            {
                _weaponImage.sprite = _unknownSprite;
                _weaponName.text = "UNKNOWN";
                _weaponDescription.text = "";
                _equips.text = "";

            }
            else
            {
                _weaponImage.sprite = _weaponInfo.WeaponUISprite;
                _weaponName.text = _weaponInfo.WeaponName;
                _weaponDescription.text = _weaponInfo.WeaponDescription;
                _equips.text = "Equipped:_" + _weaponInfo.Equips.ToString();
            }
        }
    }
}
