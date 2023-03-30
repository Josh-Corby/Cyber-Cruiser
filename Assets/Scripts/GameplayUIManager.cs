using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class GameplayUIManager : GameBehaviour<GameplayUIManager>
{
    public UISlider bossHealthBar;
    public UISlider playerHealthBar;
    public UISlider weaponUpgradeSlider;

    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _bossHealthBarUI;
    [SerializeField] private GameObject _playerHealthBarUI;
    [SerializeField] private GameObject _weaponUpgradeBarUI;

    [SerializeField] private TMP_Text _waveCountdownText;
    [SerializeField] private TMP_Text _plasmaCountText;
    [SerializeField] private TMP_Text _distanceCounterText;
    [SerializeField] private TMP_Text _bossNameText;

    private Coroutine _waveCountdownCoroutine;

    public static event Action OnCountdownDone = null;

    public string BossName
    {
        set
        {
            _bossNameText.text = value;
            _bossNameText.enabled = true;
        }
    }
    public string DistanceCounter
    {
        set
        {
            _distanceCounterText.text = value;
        }
    }
    public string PlasmaCount
    {
        set
        {
            _plasmaCountText.text = value;
        }
    }
    public string WaveCountDown
    {
        set
        {
            _waveCountdownText.text = value;
        }
    }

    private void Awake()
    {
        bossHealthBar = _bossHealthBarUI.GetComponent<UISlider>();
        playerHealthBar = _playerHealthBarUI.GetComponent<UISlider>();
    }
    private void OnEnable()
    {
        EnemySpawner.OnBossSpawned += EnableBossUI;
        Boss.OnBossDamage += ChangeSliderValue;
        Boss.OnBossDied += (v) => { DisableBossUI(); };

        PlayerManager.OnPlayerMaxHealthChange += EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange += ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart += EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick += ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished += DisableSlider;

        GameManager.OnLevelCountDownStart += StartMission;
        GameManager.OnGamePaused += EnablePauseUI;
        GameManager.OnGameResumed += DisablePauseUI;
        PlayerManager.OnPlayerDeath += GameOverUI;
        GameManager.OnDistanceChanged += UpdateDistanceText;
        PlayerManager.OnPlasmaChange += UpdatePlasmaText;     
    }

    private void OnDisable()
    {
        EnemySpawner.OnBossSpawned -= EnableBossUI;
        Boss.OnBossDamage -= ChangeSliderValue;
        Boss.OnBossDied -= (v) => { DisableBossUI(); };

        PlayerManager.OnPlayerMaxHealthChange -= EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange -= ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart -= EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick -= ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished -= DisableSlider;

        GameManager.OnLevelCountDownStart -= StartMission;
        GameManager.OnGamePaused -= EnablePauseUI;
        GameManager.OnGameResumed -= DisablePauseUI;
        PlayerManager.OnPlayerDeath -= GameOverUI;    
        GameManager.OnDistanceChanged -= UpdateDistanceText;
        PlayerManager.OnPlasmaChange -= UpdatePlasmaText;
    }

    private void StartMission()
    {
        ResetWaveCountdown();
        _gameplayPanel.SetActive(true);
        _gameOverPanel.SetActive(false);
        _pausePanel.SetActive(false);
    }

    private void ResetWaveCountdown()
    {
        if (_waveCountdownCoroutine != null)
        {
            StopCoroutine(_waveCountdownCoroutine);
        }
        StartWaveCountdown();
    }

    private void StartWaveCountdown()
    {
        _waveCountdownCoroutine = StartCoroutine(WaveCountdown());
    }

    private void EnableSlider(UISlider slider, float maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderValues(maxValue);
    }

    private void DisableSlider(UISlider slider)
    {
        slider.gameObject.SetActive(false);
    }

    private void ChangeSliderValue(UISlider slider, float value)
    {
        slider.SetSliderValue(value);
    }

    private void EnableBossUI(Enemy boss)
    {
        BossName = boss.gameObject.name;
        EnableSlider(bossHealthBar, boss.MaxHealth);
    }
    private void DisableBossUI()
    {
        _bossHealthBarUI.SetActive(false);
        DisableSlider(bossHealthBar);
        _bossNameText.enabled = false;
        BossName = "";
    }

    private IEnumerator WaveCountdown()
    {
        _waveCountdownText.enabled = true;
        float waveCountdown = 3f;
        float startActiveTimer = 1f;

        while (waveCountdown >= 0)
        {
            WaveCountDown = waveCountdown.ToString("F2");
            waveCountdown -= Time.deltaTime;
            yield return null;
        }

        OnCountdownDone?.Invoke();
        WaveCountDown = "GO!";

        while(startActiveTimer >= 0)
        {
            startActiveTimer -= Time.deltaTime;
            yield return null;
        }
        _waveCountdownText.enabled = false;
    }

    private void UpdatePlasmaText(int plasmaCount)
    {
        PlasmaCount = plasmaCount.ToString();
    }

    private void UpdateDistanceText(int distance)
    {
        DistanceCounter = distance.ToString();
    }

    private void GameOverUI()
    {
        Cursor.visible = true;
        _gameOverPanel.SetActive(true);
    }

    private void EnablePauseUI()
    {
        _pausePanel.SetActive(true);
    }

    private void DisablePauseUI()
    {
        _pausePanel.SetActive(false);
    }
}
