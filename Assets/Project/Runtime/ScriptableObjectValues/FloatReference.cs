using UnityEngine;

namespace TowerDefenseGame
{
    namespace CyberCruiser
    {
        public bool UseConstant = false;
        public float ConstantValue;
        public FloatValue Variable;

        public float Value
        {
            get
            {
                return UseConstant ? ConstantValue : Variable.Value;
            }
        }
    }
}
