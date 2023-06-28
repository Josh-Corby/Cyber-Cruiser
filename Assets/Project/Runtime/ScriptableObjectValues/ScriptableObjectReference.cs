using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberCruiser
{
    [Serializable]
    public class ScriptableObjectReference<T>
    {
        public bool UseConstant = false;
        public T ConstantValue;
        public ScriptableObjectValue<T> Variable;

        public T Value
        {
            get
            {
                return UseConstant ? ConstantValue : Variable.Value;
            }
        }
    }
}
