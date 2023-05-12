using TMPro;
using UnityEngine.UI;

public class PlayerPlasmaTextDisplay : GameBehaviour
{
    private TMP_Text _plasmaText;

    private void Awake()
    {
        _plasmaText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnPlasmaChange += (playerPlasma) => { UpdatePlasmaText(); } ;
        UpdatePlasmaText();
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnPlasmaChange -= (playerPlasma) => { UpdatePlasmaText(); } ;
    }

    private void UpdatePlasmaText()
    {
        _plasmaText.text = PSM.PlayerPlasma.ToString();
    }
}
