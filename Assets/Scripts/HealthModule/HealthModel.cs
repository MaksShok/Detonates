using HealthModule;
using ReactivePR;
using UnityEngine;

public class HealthModel : ISpendHealth
{
    public bool Alive => _health.Value > 0;

    public IReadOnlyReactiveProperty<int> Health => _health;

    private ReactiveProperty<int> _health = new ReactiveProperty<int>(0);
    
    public HealthModel(int startHealth)
    {
        _health.Value = Mathf.Max(0, startHealth);
    }

    public void Spend(int damage)
    {
        _health.Value = Mathf.Max(0, _health.Value - damage);
    }

    public void AddHealth(int healthBuff)
    {
        _health.Value += Mathf.Abs(healthBuff);
    }

    public void SetHealth(int health)
    {
        _health.Value = health;
    }
}