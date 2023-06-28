using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class AddOnPickup<T> : MonoBehaviour, IPickup
    {
        [SerializeField] private ScriptableObjectValue<T> _scriptableObjectValueToChange;
        [SerializeField] private ScriptableObjectValueChanger<T> _valueChanger;
        [SerializeField] private ScriptableObjectReference<T> _valueToChangeWith;

        public void OnPickup()
        {
            _valueChanger.ChangeValue(_scriptableObjectValueToChange, _valueToChangeWith.Value);
        }
    }
}
