using System;
using HealthModule;
using UnityEngine;

public class HealthModel : ISpendHealth
{
    public event Action<int> OnHealthChanged;
    
    public bool Alive => _health > 0;
    public int Health => _health;
    
    private int _health;
    
    public HealthModel(int startHealth)
    {
        _health = Mathf.Max(0, startHealth);
    }

    public void SpendHealth(int damage)
    {
        _health = Mathf.Max(0, _health - damage);
    }

    public void AddHealth(int healthBuff)
    {
        _health += Mathf.Abs(healthBuff);
    }
}