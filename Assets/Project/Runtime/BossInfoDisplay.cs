using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class BossInfoDisplay : EnemyInfoDisplay
    {
        [SerializeField] private TMP_Text _bossDescriptionText;
        [SerializeField] private Sprite _questionSprite;

        protected override void OnEnable()
        {
            if (_enemyInfo.KillData.TimesKilled > 0)
            {
                _bossDescriptionText.text = _enemyInfo.EnemyAbilities;
                _enemyImage.sprite = _enemyInfo.EnemyImage;
                _enemyName.text = _enemyInfo.GeneralStats.Name;
                _timesKilled.text = _enemyInfo.KillData.TimesKilled.ToString();
                _playerKills.text = _enemyInfo.KillData.PlayerKills.ToString();
            }
            else
            {
                _enemyName.text = "Unknown Entity";
                _bossDescriptionText.text = "";
                _enemyImage.sprite = _questionSprite;
                _timesKilled.text = "";
                _playerKills.text = "";
            }
        }
    }
}
