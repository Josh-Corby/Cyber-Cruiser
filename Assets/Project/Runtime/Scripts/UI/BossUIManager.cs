using CyberCruiser.Audio;
using System;
using System.Collections;
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
        [SerializeField] private Sprite _bossHealthFillSprite;

        [Header("Boss Warnings")]
        [SerializeField] private Sprite _battleCruiserWarning;
        [SerializeField] private Sprite _behemothWarning;
        [SerializeField] private Sprite _cyberKrakenWarning;
        [SerializeField] private Sprite _robodactylWarning;

        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _bossWarningClip;

        private Coroutine _bossWarningCoroutine = null;

        private float _warningClipLength;
        private string SetBossNameText
        {
            set
            {
                _bossNameText.text = value;
                _bossNameText.enabled = true;
            }
        }

        public static event Action OnBossWarningComplete = null;

        private void OnEnable()
        {
            Boss.OnBossDamage += UpdateBossHealthBar;
            Boss.OnBossDiedPosition += (p, v) => DisableBossUI();
            EnemySpawner.OnBossSpawned += EnableBossUI;
            EnemySpawnerManager.OnBossSelected += StartBossWarningCoroutine;
            GameManager.OnMissionEnd += DisableBossUI;
        }

        private void OnDisable()
        {
            Boss.OnBossDamage -= UpdateBossHealthBar;
            Boss.OnBossDiedPosition -= (p, v) => DisableBossUI();
            EnemySpawner.OnBossSpawned -= EnableBossUI;
            EnemySpawnerManager.OnBossSelected -= StartBossWarningCoroutine;
            GameManager.OnMissionEnd -= DisableBossUI;
        }

        private void Start()
        {
            _bossWarningUI.SetActive(false);
            _warningClipLength = _bossWarningClip.Clip.length;
        }

        private void StopBossWarningCoroutine()
        {
            _bossWarningUI.SetActive(false);
            if (_bossWarningCoroutine != null)
            {
                StopCoroutine(_bossWarningCoroutine);
            }
        }

        private void StartBossWarningCoroutine(EnemyScriptableObject bossInfo)
        {
            if (_bossWarningCoroutine != null)
            {
                StopCoroutine(_bossWarningCoroutine);
            }

            _bossWarningCoroutine = StartCoroutine(EnableBossWarningUI(bossInfo));
        }

        public IEnumerator EnableBossWarningUI(EnemyScriptableObject bossInfo)
        {
            switch (bossInfo.GeneralStats.Type)
            {
                case EnemyTypes.Battlecruiser:
                    _bossWarningImage.sprite = _battleCruiserWarning;
                    break;
                case EnemyTypes.Behemoth:
                    _bossWarningImage.sprite = _behemothWarning;
                    break;
                case EnemyTypes.CyberKraken:
                    _bossWarningImage.sprite = _cyberKrakenWarning;
                    break;
                case EnemyTypes.Robodactyl:
                    _bossWarningImage.sprite = _robodactylWarning;
                    break;
            }

            _bossWarningUI.SetActive(true);
            _soundController.PlayNewClip(_bossWarningClip);

            yield return new WaitForSeconds(_warningClipLength + 1f);
            OnWarningCountdownComplete();
        }

        private void OnWarningCountdownComplete()
        {
            _bossWarningUI.SetActive(false);
            OnBossWarningComplete?.Invoke();
        }

        private void EnableBossUI(EnemyScriptableObject boss)
        {
            SetBossNameText = boss.GeneralStats.Name;
            _bossHealthSlider.SetFillImage(_bossHealthFillSprite);
            _bossHealthSlider.EnableAndSetSlider(boss.GeneralStats.MaxHealth,0, boss.GeneralStats.MaxHealth);
        }

        private void DisableBossUI()
        {
            _bossNameText.enabled = false;
            SetBossNameText = "";
            StopBossWarningCoroutine();
        }

        private void UpdateBossHealthBar(float value)
        {
            _bossHealthSlider.ChangeSliderValue(value);
        }
    }
}