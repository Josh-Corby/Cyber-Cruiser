using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUI : GameBehaviour
{
    [SerializeField] private SpriteRenderer _rankImageRenderer;
    [SerializeField] private SpriteRenderer _rankTextRenderer;
    [SerializeField] private Image[] _goldStars;
    private Rank _currentRank;
    private int _currentEnabledGoldStars;
    private int _starsToGain;

    private void OnEnable()
    {
        //on enable check if current rank on UI is same as rank before star/rank up of player
        ValidateCurrentRank();

        //if the player is gaining any stars then start star gain process
        if (PSM.StarsToGain > 0)
        {
            _starsToGain = PSM.StarsToGain;
            StartCoroutine(GainStarsAnimation());
        }
    }

    private void ValidateCurrentRank()
    {
        //make sure displayed rank is correct
        if (_currentRank != PSM.RankBeforeRankUp)
        {
            _currentRank = PSM.RankBeforeRankUp;
            AssignRankUI();
        }

        //make sure displayed stars are correct
        if(_currentEnabledGoldStars != PSM.StarsBeforeStarGain) 
        {
            _currentEnabledGoldStars = PSM.StarsBeforeStarGain;
            AssignStarsUI();
        }
    }

    private void AssignRankUI()
    {
        _rankImageRenderer.sprite = _currentRank.Sprite;
        _rankTextRenderer.sprite = _currentRank.RankTextSprite;
    }

    private void AssignStarsUI()
    {  
        for (int i = 0; i < _currentRank.StarsToRankUp; i++)
        {
            _goldStars[i].enabled = i < _currentEnabledGoldStars;
        }
    }

    private IEnumerator GainStarsAnimation()
    {
        for (int i = _starsToGain; i > 0; i--)
        {
            if (_currentEnabledGoldStars < _currentRank.StarsToRankUp)
            {
                _goldStars[_currentEnabledGoldStars].enabled = true;
                _currentEnabledGoldStars++;
                yield return new WaitForSeconds(1f);
            }

            else if (_currentEnabledGoldStars == _currentRank.StarsToRankUp)
            {
                RankUp();
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void RankUp()
    {
        _currentEnabledGoldStars = 0;
        RM.RankUp(_currentRank.RankID);
        AssignRankUI();
    }
}
