using TMPro;
using UnityEngine;

namespace CyberCruiser
{
    public class BossInfoDisplay : EnemyInfoDisplay
    {
        [SerializeField] private TMP_Text _bossDescriptionText;

        protected override void Awake()
        {
            base.Awake();
            _bossDescriptionText.text = _enemyInfo.EnemyAbilities;
        }
    }
}
