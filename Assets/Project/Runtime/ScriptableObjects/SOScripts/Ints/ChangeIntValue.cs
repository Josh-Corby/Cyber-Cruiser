namespace CyberCruiser
{
    public class ChangeIntValue : ScriptableObjectValueChanger<int>
    {
        public override void ChangeValue(ScriptableObjectValue<int> valueToChange, int valueToChangeWith)
        {
            valueToChange.Value += valueToChangeWith;
        }
    }
}
