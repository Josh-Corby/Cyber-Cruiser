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

        protected virtual void Awake()
        {


            if(_enemyInfo != null)
            {
                _enemyImage.sprite = _enemyInfo.EnemyImage;
                _enemyName.text = _enemyInfo.EnemyName;
            }     
        }
    }
}
