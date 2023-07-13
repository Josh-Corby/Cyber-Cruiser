namespace CyberCruiser
{
    public class SetFloatValue : ScriptableObjectValueChanger<float>
    {
        public override void ChangeValue(ScriptableObjectValue<float> valueToChange, float valueToChangeWith)
        {
            valueToChange.Value = valueToChangeWith;
        }
    }
}
