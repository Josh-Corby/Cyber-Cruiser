using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : GameBehaviour
{
    [SerializeField] private GameObject _bossWarningUI, _bossHealthBarUI;
    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private UISlider _bossHealthBar;
    [SerializeField] private Image _bossWarningImage;

    [Header("Boss Warnings")]
    [SerializeField] private Sprite _battleCruiserWarning;
    [SerializeField] private Sprite _behemothWarning;
    [SerializeField] private Sprite _cyberKrakenWarning;
    [SerializeField] private Sprite _robodactylWarning;

    public string BossNameText
    {
        set
        {
            _bossNameText.text = value;
            _bossNameText.enabled = true;
        }
    }
    private void OnEnable()
    {
        EnemySpawnerManager.OnBossSelected += EnableBossWarningUI;
        GameManager.OnMissionEnd += DisableBossUI;
        EnemySpawner.OnBossSpawned += (e) => { DisableBossWarningUI(); };
        EnemySpawner.OnBossSpawned += EnableBossUI;
        Boss.OnBossDied += (p, v) => { DisableBossUI(); };
        Boss.OnBossDamage += UpdateBossHealthBar;
    }

    private void OnDisable()
    {
        EnemySpawnerManager.OnBossSelected -= EnableBossWarningUI;
        GameManager.OnMissionEnd -= DisableBossUI;
        EnemySpawner.OnBossSpawned -= (e) => { DisableBossWarningUI(); };
        Boss.OnBossDamage -= UpdateBossHealthBar;
        EnemySpawner.OnBossSpawned -= EnableBossUI;
        Boss.OnBossDied -= (p, v) => { DisableBossUI(); };
    }

    private void Start()
    {
        DisableBossUI();
    }
    public void EnableBossWarningUI(EnemyScriptableObject bossInfo)
    {
        switch (bossInfo.unitName)
        {
            case "BattleCruiser":
                _bossWarningImage.sprite = _battleCruiserWarning;
                break;
            case "Behemoth":
                _bossWarningImage.sprite = _behemothWarning;
                break;
            case "Cyber Kraken":
                _bossWarningImage.sprite = _cyberKrakenWarning;
                break;
            case "Robodactyl":
                _bossWarningImage.sprite = _robodactylWarning;
                break;
        }
        _bossWarningUI.SetActive(true);
    }

    private void DisableBossWarningUI()
    {
        _bossWarningUI.SetActive(false);
    }

    private void EnableBossUI(EnemyScriptableObject boss)
    {
        BossNameText = boss.unitName;
        EnableSlider(_bossHealthBar, boss.maxHealth);
    }

    private void DisableBossUI()
    {
        _bossHealthBarUI.SetActive(false);
        DisableSlider(_bossHealthBar);
        _bossNameText.enabled = false;
        BossNameText = "";
        DisableBossWarningUI();
    }

    private void UpdateBossHealthBar(float value)
    {
        ChangeSliderValue(_bossHealthBar, value);
    }
}
