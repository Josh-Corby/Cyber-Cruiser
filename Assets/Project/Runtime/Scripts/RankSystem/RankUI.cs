using CyberCruiser.Audio;
using DG.Tweening;
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
        [SerializeField] private CurrentMissionDisplay _currentMissionDisplay;
        [SerializeField] private UIType _uiType;
        [SerializeField] private Image[] _rankImageRenderers;
        [SerializeField] private Image _rankTextRenderer;
        [SerializeField] private GameObject[] _greyStars;
        [SerializeField] private GameObject[] _goldStars;
        [SerializeField] private List <StarAnimation> _starAnimations = new();
        [SerializeField] private Rank _currentRank;

        [SerializeField] private SoundControllerBase _soundController;
        [SerializeField] private ClipInfo _starClip;
        //[SerializeField] private AudioSource _audioSource;
        //[SerializeField] private AudioClip _starClip;

        [SerializeField] private int _starsToGain;
        [SerializeField] private int _starsEnabled;
        private int _playerStarsBeforeMissionStart;
        private Rank _playerRankBeforeMissionStart;

        private float _starAnimationDelayInSeconds = 1.2f;
        private Coroutine _gainStarsCoroutine;

        [SerializeField] private RectTransform[] _movingStarRects;
        private int _starsPlaced = 0;
        private void Awake()
        {
            if (_uiType == UIType.Animated)
            {
                InitializeStarAnimationsList();
            }
        }

        private void InitializeStarAnimationsList()
        {
            for (int i = 0; i < _goldStars.Length; i++)
            {
                _starAnimations.Add(_goldStars[i].GetComponent<StarAnimation>());
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
                _starsPlaced = 0;
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
                _starsEnabled += 1;
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
            for (int i = 0; i < _greyStars.Length; i++)
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

            _gainStarsCoroutine = StartCoroutine(NewGainStarsAnimation());
        }

        private IEnumerator NewGainStarsAnimation()
        {
            Debug.Log("Starting star animation");
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < _starsToGain; i++)
            {
                Debug.Log("Moving star " + (_starsPlaced + 1) + " to position " + (_starsEnabled + 1));
                if (_starsEnabled < _currentRank.StarsToRankUp)
                {
                    //get star to move
                    RectTransform starToMove = _movingStarRects[_starsPlaced];

                    //get move location
                    RectTransform targetsprite = _greyStars[_starsEnabled].GetComponent<RectTransform>();
                    HorizontalLayoutGroup layoutGroup = targetsprite.GetComponentInParent<HorizontalLayoutGroup>();

                    if (layoutGroup != null)
                    {
                        layoutGroup.enabled = false;
                    }

                    Vector3 goalPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, targetsprite.position);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(starToMove.parent as RectTransform, goalPosition, Camera.main, out Vector2 localPoint);

                    //play animation
                    starToMove.DOLocalMove(localPoint, 1f);

                    //enable and replace gold star
                    yield return new WaitForSeconds(1f);

                    _goldStars[_starsEnabled].SetActive(true);
                    _starAnimations[_starsEnabled].PlayEffectAnimation();
                    starToMove.gameObject.SetActive(false);
                    _starsEnabled += 1;
                    _starsPlaced += 1;
                }

                if (_starsEnabled >= _currentRank.StarsToRankUp)
                {
                    yield return new WaitForSeconds(1f);
                    RankUp();
                    yield break;
                }
            }
        }   

        private void StarSoundEffect()
        {
            _soundController.PlayNewClip(_starClip);
        }

        private void RankUp()
        {
            _starsToGain -= (_currentRank.StarsToRankUp - _playerRankManager.StarsBeforeMissionStart);
            _starsEnabled = 0;
            DisableAllStars();
            _currentRank = RankManagerInstance.RankUp(_currentRank.RankID);
            EnableGreyStars();
            SetRankUI();
            if (_gainStarsCoroutine != null)
            {
                StopCoroutine(_gainStarsCoroutine);
            }
            _gainStarsCoroutine = StartCoroutine(NewGainStarsAnimation());
        }
    }
}