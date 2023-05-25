using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class RankUI : GameBehaviour
    {
        enum UIType
        {
            Animated, Static,
        }

        [SerializeField] private PlayerRankManager _playerRankManager;
        [SerializeField] private UIType _uiType;
        [SerializeField] private Image[] _rankImageRenderers;
        [SerializeField] private Image _rankTextRenderer;
        [SerializeField] private GameObject[] _greyStars;
        [SerializeField] private GameObject[] _goldStars;
        [SerializeField] private Rank _currentRank;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _starClip;

        [SerializeField] private int _starsToGain;
        [SerializeField] private int _starsEnabled;
        private int _playerStarsBeforeMissionStart;
        private Rank _playerRankBeforeMissionStart;

        private float _starAnimationDelayInSeconds = 1.2f;
        private Coroutine _gainStarsCoroutine;

        private void Awake()
        {
            if(_uiType == UIType.Animated)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }
        private void OnEnable()
        {
            GetPlayerRankInfoBeforeMissionStart();
            SetStaticRankUI();
            DisableAllStars();
            EnableGreyStars();
            EnableStaticStars();

            if (_uiType == UIType.Animated)
            {
                MissionScreenAnimation();
                StarAnimation.OnStarAtDestination += StarSoundEffect;
            }
        }

        private void OnDisable()
        {
            if (_uiType == UIType.Animated)
            {
                StarAnimation.OnStarAtDestination -= StarSoundEffect;
            }
        }

        private void GetPlayerRankInfoBeforeMissionStart()
        {
            _playerRankBeforeMissionStart = _playerRankManager.RankBeforeMissionStart;
            _currentRank = _playerRankBeforeMissionStart;
            _playerStarsBeforeMissionStart = _playerRankManager.StarsBeforeMissionStart;
            _starsEnabled = _playerStarsBeforeMissionStart;
        }

        private void SetStaticRankUI()
        {
            for (int i = 0; i < _rankImageRenderers.Length; i++)
            {
                _rankImageRenderers[i].sprite = _playerRankBeforeMissionStart.Sprite;

            }
            _rankTextRenderer.sprite = _playerRankBeforeMissionStart.RankTextSprite;
        }

        private void EnableStaticStars()
        {
            for (int i = 0; i < _playerStarsBeforeMissionStart; i++)
            {
                _goldStars[i].SetActive(true);
            }
        }

        private void EnableGreyStars()
        {
            int starsToEnable = _currentRank.StarsToRankUp;
            for (int i = 0; i < starsToEnable; i++)
            {
                _greyStars[i].SetActive(true);
            }
        }

        private void DisableAllStars()
        {
            for (int i = 0; i < _currentRank.StarsToRankUp; i++)
            {
                _goldStars[i].SetActive(false);
                _greyStars[i].SetActive(false);
            }
            _starsEnabled = 0;
        }

        private void SetRankUI()
        {
            for (int i = 0; i < _rankImageRenderers.Length; i++)
            {
                _rankImageRenderers[i].sprite = _playerRankBeforeMissionStart.Sprite;

            }
            _rankTextRenderer.sprite = _currentRank.RankTextSprite;
        }

        private void MissionScreenAnimation()
        {
            if (_playerRankManager.TotalStarReward > 0)
            {
                _starsToGain = _playerRankManager.TotalStarReward;
            }

            _gainStarsCoroutine = StartCoroutine(GainStarsAnimation());
        }

        private IEnumerator GainStarsAnimation()
        {
            for (int i = 0; i < _starsToGain; i++)
            {
                if (_starsEnabled < _currentRank.StarsToRankUp)
                {
                    yield return new WaitForSeconds(_starAnimationDelayInSeconds);
                    _goldStars[_starsEnabled].SetActive(true);
                    _starsEnabled += 1;
                }

                if (_starsEnabled >= _currentRank.StarsToRankUp)
                {
                    yield return new WaitForSeconds(_starAnimationDelayInSeconds);
                    _starsToGain -= _currentRank.StarsToRankUp;
                    RankUp();
                    i = 0;
                    StopCoroutine(_gainStarsCoroutine);
                }
            }
        }

        private void StarSoundEffect()
        {
            _audioSource.PlayOneShot(_starClip);
        }

        private void RankUp()
        {
            _starsEnabled = 0;
            DisableAllStars();
            _currentRank = RankManagerInstance.RankUp(_currentRank.RankID);
            EnableGreyStars();
            SetRankUI();
            _gainStarsCoroutine = StartCoroutine(GainStarsAnimation());
        }
    }
}