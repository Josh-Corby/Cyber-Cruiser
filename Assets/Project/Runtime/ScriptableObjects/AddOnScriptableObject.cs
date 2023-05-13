using UnityEngine;

[CreateAssetMenu(fileName = "AddOn", menuName = "ScriptableObject/New AddOn")]
public class AddOnScriptableObject : ScriptableObject
{
    public string Name;
    public int ID;
    public string Description;
    public int IonCost;
}
