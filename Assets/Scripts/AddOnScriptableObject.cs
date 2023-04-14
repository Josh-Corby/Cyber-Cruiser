using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddOn")]
public class AddOnScriptableObject : ScriptableObject
{

    public AddOnTypes AddOnType;
    public string Description;
    public int IonCost;
}
