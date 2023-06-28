using System;

namespace CyberCruiser
{
    [Serializable]
    public class IntReference
    {
        public bool UseConstant = false;
        public int ConstantValue;
        public IntValue Variable;

        public int Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }
    }
}
