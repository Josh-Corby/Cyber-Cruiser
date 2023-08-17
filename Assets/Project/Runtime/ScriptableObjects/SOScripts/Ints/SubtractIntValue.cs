using UnityEngine;

namespace CyberCruiser
{
    public class SubtractIntValue : ScriptableObjectValueChanger<int>
    {
        public override void ChangeValue(ScriptableObjectValue<int> valueToChange, int valueToChangeWith)
        {
            valueToChange.Value -= valueToChangeWith;
        }
    }
}
