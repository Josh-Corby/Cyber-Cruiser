using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUI : GameBehaviour
{
    enum UIType
    {
        Animated, Static, 
    }

    [SerializeField] private Image _rankImageRenderer;
    [SerializeField] private Image _rankTextRenderer;
    [SerializeField] private GameObject[] _goldStars;
    [SerializeField] private UIType _uiType;


    [SerializeField] private Rank _currentRank;
    [SerializeField] private int _currentEnabledGoldStars;
    [SerializeField] private int _starsToGain;

    private Rank _playerRankBeforeMissionStart;
    private int _playerStarsBeforeMissionStart;

    //all screens will
    //get stars and rank before mission start

    //victory screen requirements
    /*
     * show stars and rank before mission start on enable
     * perform rank up animation
     * end on current rank and stars
     */
    //private void OnEnable()
    //{
    //    //get stars and rank before mission started
    //    GetPlayerRankInfoBeforeMissionStart();

    //    switch (_uiType)
    //    {
    //        case UIType.Static:
    //            break;
    //        case UIType.Animated:
    //            break;        
    //    }
    //}

    //private void GetPlayerRankInfoBeforeMissionStart()
    //{
    //    _playerRankBeforeMissionStart = PSM.RankBeforeMissionStart;
    //    _playerStarsBeforeMissionStart = PSM.StarsBeforeMissionStart;
    //}

    //private void DisplayRank()
    //{

    //}
    //private void DIsplayStars()
    //{

    //}

    private void OnEnable()
    {
        //on enable check if current rank on UI is same as rank before star/rank up of player
        ValidateCurrentRank();

        if (_uiType == UIType.Static)
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
        switch (_uiType)
        {
            case UIType.Static:

                if (_currentRank != PSM.CurrentRank)
                {
                    _currentRank = PSM.CurrentRank;
                    AssignRankUI();
                }
                if (_currentEnabledGoldStars != PSM.CurrentStars)
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
                if (_currentRank != PSM.RankBeforeMissionStart)
                {
                    _currentRank = PSM.RankBeforeMissionStart;
                    AssignRankUI();
                }

                //make sure displayed stars are correct
                if (_currentEnabledGoldStars != PSM.StarsBeforeMissionStart)
                {
                    _currentEnabledGoldStars = PSM.StarsBeforeMissionStart;
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


    #region Animated UI Functions
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
    #endregion
}
