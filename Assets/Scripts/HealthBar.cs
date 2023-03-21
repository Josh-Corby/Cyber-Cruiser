using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar;

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
    }
    private void OnEnable()
    {
        Boss.OnBossDamage += SetHP;
    }

    private void OnDisable()
    {
        Boss.OnBossDamage -= SetHP;
    }

    public void SetHealthBar(Enemy enemy)
    {
        healthBar.maxValue = enemy.maxHealth;
        healthBar.value = healthBar.maxValue;
    }

    private void SetHP(float hp)
    {
        healthBar.value = hp;
    }
}
