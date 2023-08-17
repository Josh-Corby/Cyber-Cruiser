using System;
using UnityEngine;

namespace CyberCruiser
{
    [Serializable]
    public class PickupEffect<T> : PickupEffectBase
    {
        [SerializeField] private ScriptableObjectValue<T> _scriptableObjectValueToChange;

        [Header("On Picked Up")]
        [SerializeField] private ScriptableObjectValueChanger<T> _valueChanger;
        [SerializeField] private ScriptableObjectReference<T> _valueToChangeWith;

        [Header("On Dropped")]
        [SerializeField] private ScriptableObjectValueChanger<T> _valueChangerOnDropped;
        [SerializeField] private ScriptableObjectReference<T> _valueToChangeWithOnDropped;

        public override void OnPickup()
        {
            _valueChanger.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWith.Value);
        }

        public override void OnDropped()
        {
            _valueChangerOnDropped.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWithOnDropped.Value);
        }
    }
}
