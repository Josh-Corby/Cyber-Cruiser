using System;
using UnityEngine;

namespace CyberCruiser
{
    [Serializable]
    public class PickupEffect<T> : PickupEffectBase
    {
        [SerializeField] private ScriptableObjectValue<T> _scriptableObjectValueToChange;
        [SerializeField] private ScriptableObjectValueChanger<T> _valueChanger;
        [SerializeField] private ScriptableObjectReference<T> _valueToChangeWith;

        public override void OnPickup()
        {
            _valueChanger.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWith.Value);
        }
    }
}
