using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class BossInfoDisplay : EnemyInfoDisplay
    {
        [SerializeField] private TMP_Text _bossDescriptionText;


        protected override void OnEnable()
        {
            base.OnEnable();
            if (_enemyInfo.KillData.TimesKilled > 0)
            {
                _bossDescriptionText.text = _enemyInfo.EnemyAbilities;
            }
            else
            {
                _bossDescriptionText.text = "";
            }
        }
    }
}
