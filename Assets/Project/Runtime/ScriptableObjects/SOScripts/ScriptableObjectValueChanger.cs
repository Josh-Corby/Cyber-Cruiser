using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    public class ScriptableObjectValueChanger<T> : ScriptableObject
    {
        protected T ValueType;
        public virtual void ChangeValue(ScriptableObjectValue<T> valueToChange, T valueToChangeWith)
        {

        }
    }
}
