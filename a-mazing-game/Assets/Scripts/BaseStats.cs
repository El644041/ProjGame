using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int attackDamage = 40;
    public int maxHealth = 100;
    public int maxStamina;
    public int currentHealth;
    public int currentDamage;
    public int currentStamina;

    //public IDictionary<string, int> baseStats = new Dictionary<string, int>();
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentDamage = attackDamage;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseHealth()
    {
        maxHealth+=5;
    }

    public void IncreaseAttack()
    {
        attackDamage+=5;
    }
}
