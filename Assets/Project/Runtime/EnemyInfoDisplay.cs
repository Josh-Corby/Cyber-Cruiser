using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberCruiser
{
    public class EnemyInfoDisplay : MonoBehaviour
    {

        [SerializeField] protected EnemyScriptableObject _enemyInfo;
        [SerializeField] protected Image _enemyImage;
        [SerializeField] protected TMP_Text _enemyName;
        [SerializeField] protected TMP_Text _timesKilled;
        [SerializeField] protected TMP_Text _playerKills;

        protected virtual void Awake()
        {
            if(_enemyInfo != null)
            {
                _enemyImage.sprite = _enemyInfo.EnemyImage;
                _enemyName.text = _enemyInfo.EnemyName;
            }     
        }

        protected virtual void OnEnable()
        {
            _timesKilled.text = _enemyInfo.KillData.TimesKilled.ToString();
            _playerKills.text = _enemyInfo.KillData.PlayerKills.ToString();
        }
    }
}
