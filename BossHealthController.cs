using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthController : MonoBehaviour
{

    public static BossHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public Slider BossHealthSlider;

    public int currentHealth = 30;

    public BossBattle theBoss;

    // Start is called before the first frame update
    void Start()
    {
        BossHealthSlider.maxValue = currentHealth;
        BossHealthSlider.value = currentHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            currentHealth = 0;

            theBoss.EndBattle();
        }

        BossHealthSlider.value = currentHealth;
    }
}
