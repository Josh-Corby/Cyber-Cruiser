using System.Collections;
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
        [SerializeField] private Image _rankImageRenderer;
        [SerializeField] private Image _rankTextRenderer;
        [SerializeField] private Image[] _goldStars;
        [SerializeField] private Rank _currentRank;

        private int _starsToGain;
        private int _starsEnabled;
        private int _playerStarsBeforeMissionStart;
        private Rank _playerRankBeforeMissionStart;

        private float _starAnimationDelayInSeconds = 1f;

        private void OnEnable()
        {
            GetPlayerRankInfoBeforeMissionStart();
            SetStaticRankUI();
            DisableAllStars();
            EnableStaticStars();

            if (_uiType == UIType.Animated)
            {
                MissionScreenAnimation();
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
            _rankImageRenderer.sprite = _playerRankBeforeMissionStart.Sprite;
            _rankTextRenderer.sprite = _playerRankBeforeMissionStart.RankTextSprite;
        }

        private void EnableStaticStars()
        {
            for (int i = 0; i < _playerStarsBeforeMissionStart; i++)
            {
                _goldStars[i].enabled = true;
            }
        }

        private void DisableAllStars()
        {
            for (int i = 0; i < _currentRank.StarsToRankUp; i++)
            {
                _goldStars[i].enabled = false;
            }
            _starsEnabled = 0;
        }

        private void SetRankUI()
        {
            _rankImageRenderer.sprite = _currentRank.Sprite;
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
                    _goldStars[_starsEnabled].enabled = true;
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