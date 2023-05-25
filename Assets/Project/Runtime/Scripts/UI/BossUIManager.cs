using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class BossUIManager : GameBehaviour
    {
        [SerializeField] private GameObject _bossWarningUI, _bossHealthBarUI;
        [SerializeField] private TMP_Text _bossNameText;
        [SerializeField] private UISlider _bossHealthSlider;
        [SerializeField] private Image _bossWarningImage;

        [Header("Boss Warnings")]
        [SerializeField] private Sprite _battleCruiserWarning;
        [SerializeField] private Sprite _behemothWarning;
        [SerializeField] private Sprite _cyberKrakenWarning;
        [SerializeField] private Sprite _robodactylWarning;

        private string SetBossNameText
        {
            set
            {
                _bossNameText.text = value;
                _bossNameText.enabled = true;
            }
        }

        private void OnEnable()
        {
            Boss.OnBossDamage += UpdateBossHealthBar;
            Boss.OnBossDiedPosition += (p, v) => DisableBossUI();
            EnemySpawner.OnBossSpawned += EnableBossUI;
            EnemySpawner.OnBossSpawned += (e) => DisableBossWarningUI();
            EnemySpawnerManager.OnBossSelected += EnableBossWarningUI;
            GameManager.OnMissionEnd += DisableBossUI;
        }

        private void OnDisable()
        {
            Boss.OnBossDamage -= UpdateBossHealthBar;
            Boss.OnBossDiedPosition -= (p, v) => DisableBossUI();
            EnemySpawner.OnBossSpawned -= EnableBossUI;
            EnemySpawner.OnBossSpawned -= (e) => DisableBossWarningUI();
            EnemySpawnerManager.OnBossSelected -= EnableBossWarningUI;
            GameManager.OnMissionEnd -= DisableBossUI;
        }

        private void Start()
        {
            DisableBossUI();
        }

        public void EnableBossWarningUI(EnemyScriptableObject bossInfo)
        {
            switch (bossInfo.EnemyName)
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
            SetBossNameText = boss.EnemyName;
            _bossHealthSlider.EnableSliderAtMaxValue(boss.GeneralStats.MaxHealth);
        }

        private void DisableBossUI()
        {
            _bossHealthBarUI.SetActive(false);
            _bossNameText.enabled = false;
            SetBossNameText = "";
            DisableBossWarningUI();
        }

        private void UpdateBossHealthBar(float value)
        {
            _bossHealthSlider.ChangeSliderValue(value);
        }
    }
}