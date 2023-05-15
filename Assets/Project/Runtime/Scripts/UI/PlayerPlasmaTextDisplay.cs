using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayerPlasmaTextDisplay : GameBehaviour
{
    private TMP_Text _plasmaText;

    private void Awake()
    {
        _plasmaText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        PlayerStatsManager.OnPlasmaChange += UpdatePlasmaText;
        UpdatePlasmaText(PlayerStatsManagerInstance.PlayerPlasma);
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnPlasmaChange -= UpdatePlasmaText;
    }

    private void UpdatePlasmaText(int plasma)
    {
        _plasmaText.text = plasma.ToString();
    }
}
