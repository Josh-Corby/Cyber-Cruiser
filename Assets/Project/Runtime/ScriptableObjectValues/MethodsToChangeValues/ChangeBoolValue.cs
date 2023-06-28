using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class ChangeBoolValue : ScriptableObjectValueChanger<bool>
    {
        public override void ChangeValue(ScriptableObjectValue<bool> valueToChange, bool valueToChangeWith)
        {
            valueToChange.Value = valueToChangeWith;
        }
    }
}
