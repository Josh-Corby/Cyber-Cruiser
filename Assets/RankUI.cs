using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUI : GameBehaviour
{
    enum UIType
    {
        Animated, Static
    }

    [SerializeField] private Image _rankImageRenderer;
    [SerializeField] private Image _rankTextRenderer;
    [SerializeField] private GameObject[] _goldStars;
    [SerializeField] private UIType _type;
    [SerializeField] private Rank _currentRank;
    [SerializeField] private int _currentEnabledGoldStars;
    [SerializeField] private int _starsToGain;

    private void OnEnable()
    {
        _currentEnabledGoldStars = 0;
        //on enable check if current rank on UI is same as rank before star/rank up of player
        ValidateCurrentRank();

        if (_type == UIType.Static)
        {
            EnableStars();
            return;
        }
        //if the player is gaining any stars then start star gain process
        if (_starsToGain == 0) return;
        GainStarsAnimation();
    }

    private void ValidateCurrentRank()
    {
        switch (_type)
        {
            case UIType.Static:

                if(_currentRank != PSM.CurrentRank)
                {
                    _currentRank = PSM.CurrentRank;
                    AssignRankUI();
                }
                if(_currentEnabledGoldStars != PSM.CurrentStars)
                {
                    DisableAllStars();
                    _starsToGain = PSM.CurrentStars;
                }

                break;
                case UIType.Animated:
                if (PSM.TotalStarReward > 0)
                {
                    _starsToGain = PSM.TotalStarReward;
                }
                //make sure displayed rank is correct
                if (_currentRank != PSM.RankBeforeRankUp)
                {
                    _currentRank = PSM.RankBeforeRankUp;
                    AssignRankUI();
                }

                //make sure displayed stars are correct
                if (_currentEnabledGoldStars != PSM.StarsBeforeStarGain)
                {
                    _currentEnabledGoldStars = PSM.StarsBeforeStarGain;
                }
                break;
        }   
    }

    private void AssignRankUI()
    {
        _rankImageRenderer.sprite = _currentRank.Sprite;
        _rankTextRenderer.sprite = _currentRank.RankTextSprite;
    }

    private void EnableStars()
    {
        for (int i = 0; i < _starsToGain; i++)
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
        _currentEnabledGoldStars = 0;
    }

    private void GainStarsAnimation()
    {
        for (int i = _starsToGain; i > 0; i--)
        {
            Debug.Log(i + " stars left to gain");

            if (_currentEnabledGoldStars < _currentRank.StarsToRankUp)
            {
                Debug.Log("star earned");
                _goldStars[_currentEnabledGoldStars].SetActive(true);
                _currentEnabledGoldStars += 1;
            }

            if (_currentEnabledGoldStars >= _currentRank.StarsToRankUp)
            {
                _starsToGain -= _currentRank.StarsToRankUp;
                Debug.Log("Rank up");
                RankUp();
                return;
            }
        }
    }

    private void RankUp()
    {
        _currentEnabledGoldStars = 0;
        DisableAllStars();
        _currentRank = RM.RankUp(_currentRank.RankID);
        AssignRankUI();
        GainStarsAnimation();
    }
}
