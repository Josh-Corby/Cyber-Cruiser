using CyberCruiser;
using UnityEngine;

[CreateAssetMenu(fileName = "AddOn", menuName = "ScriptableObject/New AddOn")]
public class AddOnSO : ScriptableObject
{
    [SerializeField] private PickupInfo _info;

    //Bool SO used to track if the player has this addon
    [SerializeField] private BoolReference _equipBoolToTrack;
    [SerializeField] private int _timesPickedUp = 0;
    public PickupInfo Info { get => _info; }
    public bool DoesPlayerHave { get => _equipBoolToTrack.Value; }
    public int TimesPickedUp { get => _timesPickedUp; private set => _timesPickedUp = value; }

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
