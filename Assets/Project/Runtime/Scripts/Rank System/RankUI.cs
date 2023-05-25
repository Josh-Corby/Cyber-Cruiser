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
        [SerializeField] private GameObject[] _goldStars;
        private List<StarAnimation> _starAnimations = new();
        [SerializeField] private Rank _currentRank;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _starClip;

        private int _starsToGain;
        private int _starsEnabled;
        private int _playerStarsBeforeMissionStart;
        private Rank _playerRankBeforeMissionStart;

        private float _starAnimationDelayInSeconds = 1f;

        private void Awake()
        {
            if(_uiType == UIType.Animated)
            {
                _audioSource = GetComponent<AudioSource>();
                for (int i = 0; i < _goldStars.Length; i++)
                {
                    _starAnimations.Add(_goldStars[i].GetComponent<StarAnimation>());
                }
            }
        }
        private void OnEnable()
        {
            GetPlayerRankInfoBeforeMissionStart();
            SetStaticRankUI();
            DisableAllStars();
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

        private void DisableAllStars()
        {
            for (int i = 0; i < _currentRank.StarsToRankUp; i++)
            {
                _goldStars[i].SetActive(false);
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

            StartCoroutine(GainStarsAnimation());
        }

        private IEnumerator GainStarsAnimation()
        {
            for (int i = 0; i < _starsToGain; i++)
            {
                if (_starsEnabled < _currentRank.StarsToRankUp)
                {
                    yield return new WaitForSeconds(_starAnimationDelayInSeconds);
                    _goldStars[_starsEnabled].SetActive(true);
                    _starAnimations[i].PlayMoveAnimation();

                    _starsEnabled += 1;
                }

                if (_starsEnabled >= _currentRank.StarsToRankUp)
                {
                    yield return new WaitForSeconds(_starAnimationDelayInSeconds);
                    _starsToGain -= _currentRank.StarsToRankUp;
                    RankUp();
                    StopCoroutine(GainStarsAnimation());
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
            SetRankUI();
            StartCoroutine(GainStarsAnimation());
        }
    }
}