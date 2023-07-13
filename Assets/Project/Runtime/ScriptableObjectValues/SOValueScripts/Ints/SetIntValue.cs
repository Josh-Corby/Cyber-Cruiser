namespace CyberCruiser
{
    public class SetIntValue : ScriptableObjectValueChanger<int>
    {
        public override void ChangeValue(ScriptableObjectValue<int> valueToChange, int valueToChangeWith)
        {
            valueToChange.Value = valueToChangeWith;
        }
    }
}
