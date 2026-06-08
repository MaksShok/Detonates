using HealthModule;
using UnityEngine;

public class StatueHealth : MonoBehaviour, ISpendHealth
{
    [Header("Настройки здоровья")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject destroyEffect; // эффект при разрушении (опционально)

    public int Health => currentHealth;
    public int MaxHealth => maxHealth;
    public bool Alive => currentHealth > 0;
    
    public event System.Action OnDie;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"Статуя создана. HP: {currentHealth}/{maxHealth}");
    }

    public void Spend(int value)
    {
        if (!Alive) return; // если уже мертва — не принимаем урон

        currentHealth -= value;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"Статуя получила {value} урона. Осталось HP: {currentHealth}/{maxHealth}");

        // Эффект получения урона (мигание, тряска и т.д.)
        OnDamageTaken();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnDamageTaken()
    {
        // Можно добавить анимацию получения урона
        // animator.SetTrigger("Hit");
        
        // Или мигание спрайта
        StartCoroutine(FlashDamage());
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    private void Die()
    {
        Debug.Log("Статуя разрушена!");

        OnDie?.Invoke();

        // Эффект разрушения
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Удаляем статую
        Destroy(gameObject);
    }
}