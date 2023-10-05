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

        [Header("Events")]
        [SerializeField] private GameEvent _onPickup;
        [SerializeField] private GameEvent _onDrop;

        public override void OnPickup()
        {
            if(_scriptableObjectValueToChange != null)
            _valueChanger.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWith.Value);

            if(_onPickup != null)
            _onPickup.Raise();
        }

        public override void OnDropped()
        {
            if (_scriptableObjectValueToChange != null)
                _valueChangerOnDropped.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWithOnDropped.Value);

            if(_onDrop != null)
            _onDrop.Raise();
        }
    }
}
