using TMPro;
using UnityEngine.UI;
public class PlayerIonTextDisplay : GameBehaviour
{
    private TMP_Text _ionTMPText;

    private void Awake()
    {
        _ionTMPText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnIonChange += (playerIon) => { UpdateIonText(); };
        UpdateIonText();
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnIonChange -= (playerIon) => { UpdateIonText(); };
    }

    private void UpdateIonText()
    {
        _ionTMPText.text = PSM.PlayerIon.ToString();
    }
}
