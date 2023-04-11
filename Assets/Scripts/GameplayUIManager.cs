using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class GameplayUIManager : GameBehaviour<GameplayUIManager>
{
    public UISlider bossHealthBar;
    public UISlider playerHealthBar;
    public UISlider playerShieldBar;
    public UISlider weaponUpgradeSlider;
    public UISlider weaponHeatBar;

    [SerializeField] private GameObject _gameplayPanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _bossWarningUI;
    [SerializeField] private GameObject _bossHealthBarUI;
    [SerializeField] private GameObject _playerHealthBarUI;
    [SerializeField] private GameObject _playerShieldBarUI;
    [SerializeField] private GameObject _weaponUpgradeBarUI;
    [SerializeField] private GameObject _weaponHeatBar;
    [SerializeField] private TMP_Text _waveCountdownText;
    [SerializeField] private TMP_Text _plasmaCountText;
    [SerializeField] private TMP_Text _distanceCounterText;
    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private TMP_Text _bossWarningText;

    private Coroutine _waveCountdownCoroutine;

    public static event Action OnCountdownDone = null;
    public static event Action<Action> OnMaxHeatRequested = null;

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


    private void OnEnable()
    {
        EnemySpawner.OnBossSpawned += EnableBossUI;
        EnemySpawner.OnBossSpawned += (e) => { DisableBossWarningUI(); };

        Boss.OnBossDamage += ChangeSliderValue;
        Boss.OnBossDied += (p,v) => { DisableBossUI(); };

        PlayerManager.OnPlayerMaxHealthChange += EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange += ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart += EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick += ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished += DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized += EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange += ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange += OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated += EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated += DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange += ChangeSliderValue;

        GameManager.OnLevelCountDownStart += StartMission;
        GameManager.OnGamePaused += EnablePauseUI;
        GameManager.OnGameResumed += DisablePauseUI;

        PlayerManager.OnPlayerDeath += GameOverUI;
        PlayerManager.OnPlasmaChange += UpdatePlasmaText;

        DistanceManager.OnDistanceChanged += UpdateDistanceText; 
    }

    private void OnDisable()
    {
        EnemySpawner.OnBossSpawned -= EnableBossUI;
        EnemySpawner.OnBossSpawned -= (e) => { DisableBossWarningUI(); };

        Boss.OnBossDamage -= ChangeSliderValue;
        Boss.OnBossDied -= (p,v) => { DisableBossUI(); };

        PlayerManager.OnPlayerMaxHealthChange -= EnableSlider;
        PlayerManager.OnPlayerCurrentHealthChange -= ChangeSliderValue;

        PlayerWeaponController.OnWeaponUpgradeStart -= EnableSlider;
        PlayerWeaponController.OnWeaponUpgradeTimerTick -= ChangeSliderValue;
        PlayerWeaponController.OnWeaponUpgradeFinished -= DisableSlider;
        PlayerWeaponController.OnWeaponHeatInitialized -= EnableAndSetSlider;
        PlayerWeaponController.OnHeatChange -= ChangeSliderValue;
        PlayerWeaponController.OnOverheatStatusChange -= OverheatUI;

        PlayerShieldController.OnPlayerShieldsActivated -= EnableSlider;
        PlayerShieldController.OnPlayerShieldsDeactivated -= DisableSlider;
        PlayerShieldController.OnPlayerShieldsValueChange -= ChangeSliderValue;

        GameManager.OnLevelCountDownStart -= StartMission;
        GameManager.OnGamePaused -= EnablePauseUI;
        GameManager.OnGameResumed -= DisablePauseUI;

        PlayerManager.OnPlayerDeath -= GameOverUI;
        PlayerManager.OnPlasmaChange -= UpdatePlasmaText;

        DistanceManager.OnDistanceChanged -= UpdateDistanceText;
    }

    private void StartMission()
    {
        ResetWaveCountdown();
        _gameplayPanel.SetActive(true);
        _gameOverPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _bossWarningUI.SetActive(false);
        DisableBossUI();
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
        slider.SetSliderMaxValue(maxValue);
    }

    private void EnableAndSetSlider(UISlider slider, float currentValue, int maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderValues(currentValue, maxValue);
    }

    private void DisableSlider(UISlider slider)
    {
        slider.gameObject.SetActive(false);
    }

    private void ChangeSliderValue(UISlider slider, float value)
    {
        if (slider.gameObject.activeSelf)
        {
            slider.SetSliderValue(value);
        }
    }

    private void OverheatUI(UISlider slider, bool status)
    {
        if (status)
        {
            slider.SetFillColour(Color.red);
        }
        else if (!status)
        {
            slider.SetFillColour(Color.cyan);
        }
    }

    public void EnableBossWarningUI(GameObject boss)
    {
        Debug.Log(boss.name + " spawning");
        _bossWarningText.text = "Warning!! " + boss.name + " approaching";
        _bossWarningUI.SetActive(true);
    }

    private void DisableBossWarningUI()
    {
        _bossWarningText.text = "";
        _bossWarningUI.SetActive(false);
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
