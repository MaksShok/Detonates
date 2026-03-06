using UnityEngine;
using UnityEngine.UI;

public class StatueHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    public Slider healthBar; 
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(5);
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth < 0)
            currentHealth = 0;
            
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
}