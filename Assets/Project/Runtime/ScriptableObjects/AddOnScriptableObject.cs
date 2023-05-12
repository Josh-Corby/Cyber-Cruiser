using UnityEngine;

[CreateAssetMenu(fileName = "AddOn", menuName = "ScriptableObject/New AddOn")]
public class AddOnScriptableObject : ScriptableObject
{

    public AddOnType AddOnType;
    public string Description;
    public int IonCost;
}
