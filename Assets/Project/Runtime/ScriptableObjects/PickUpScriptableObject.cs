using CyberCruiser;
using UnityEngine;

[CreateAssetMenu(fileName = "AddOn", menuName = "ScriptableObject/New AddOn")]
public class PickUpScriptableObject : ScriptableObject
{
    public PickupInfo Info;
    public int TimesPickedUp = 0;

    private void OnEnable()
    {
        SaveManager.OnClearSaveData += () => { TimesPickedUp = 0; };
    }

    private void OnDisable()
    {
        SaveManager.OnClearSaveData -= () => { TimesPickedUp = 0; };
    }

    public void OnPickedUp()
    {
        TimesPickedUp++;
    }
}
