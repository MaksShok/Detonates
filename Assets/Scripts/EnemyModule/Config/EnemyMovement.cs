using HealthModule;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Настройки врага")]
    public float Speed = 3f;

    [Header("Цель")]
    public Transform Tower;

    [Header("Настройки атаки")]
    public float attackRange = 0.8f;
    public float attackCooldown = 1f;
    public int damage = 10;

    private float nextAttackTime;
    private ISpendHealth targetHealth;

    void Start()
    {
        if (Tower != null)
        {
            targetHealth = Tower.GetComponent<ISpendHealth>();
        }
    }

    void Update()
    {
        if (Tower == null) return;

        float distance = Vector2.Distance(transform.position, Tower.position);

        if (distance <= attackRange)
        {
            // Близко — атакуем
            Attack();
        }
        else
        {
            // Далеко — идём
            transform.position = Vector2.MoveTowards(
                transform.position,
                Tower.position,
                Speed * Time.deltaTime
            );
        }
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime) return;

        if (targetHealth != null && targetHealth.Alive)
        {
            targetHealth.Spend(damage);
            nextAttackTime = Time.time + attackCooldown;
            
            Debug.Log("Враг ударил статую!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}