using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class ChangeIntValue : ScriptableObjectValueChanger<int>
    {
        public override void ChangeValue(ScriptableObjectValue<int> valueToChange, int valueToChangeWith)
        {
            valueToChange.Value += valueToChangeWith;
        }
    }
}
