using System;

namespace CyberCruiser
{
    [Serializable]
    public class BoolReference
    {
        public bool UseConstant = false;
        public bool ConstantValue;
        public BoolValue Variable;

        public bool Value
        {
            get { return UseConstant? ConstantValue : Variable.Value; }
        }
    }
}
