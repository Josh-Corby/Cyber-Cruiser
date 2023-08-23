using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class EnemyInfoDisplay : MonoBehaviour
    {

        [SerializeField] protected EnemyScriptableObject _enemyInfo;
        [SerializeField] protected Image _enemyImage;
        [SerializeField] private Sprite _unknownSprite;
        [SerializeField] protected TMP_Text _enemyName;
        [SerializeField] protected TMP_Text _timesKilled;
        [SerializeField] protected TMP_Text _playerKills;

        protected virtual void OnEnable()
        {
            if (_enemyInfo != null)
            {

                if (_enemyInfo.KillData.TimesKilled == 0)
                {
                    _enemyImage.sprite = _unknownSprite;
                    _enemyName.text = "UNKNOWN";
                    _timesKilled.text = "";
                    _playerKills.text = "";
                }

                else
                {
                    _enemyImage.sprite = _enemyInfo.EnemyImage;
                    _enemyName.text = _enemyInfo.GeneralStats.Name;
                    _timesKilled.text = _enemyInfo.KillData.TimesKilled.ToString();
                    _playerKills.text = _enemyInfo.KillData.PlayerKills.ToString();
                }

            }
         
        }
    }
}
