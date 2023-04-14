using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddOnManager : MonoBehaviour
{
    private bool _isBatteryPack, _isHydrocoolant, _isPlasmaCache, _isPulseDetonator;

    private void OnEnable()
    {
        AddOn.OnAddOnToggled += ToggleAddOnEffect;
    }

    private void OnDisable()
    {
        AddOn.OnAddOnToggled -= ToggleAddOnEffect;
    }

    private void ToggleAddOnEffect(AddOnTypes addOnType, bool value)
    {
        switch (addOnType)
        {
            case AddOnTypes.BatteryPack:
                break;
        }
    }

    private void ToggleBatteryPack()
    {

    }


}
