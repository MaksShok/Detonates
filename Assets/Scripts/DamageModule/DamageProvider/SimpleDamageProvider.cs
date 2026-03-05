using System;
using HealthModule;

namespace DamageModule
{
    public class SimpleDamageProvider : IDamageProvider
    {
        public int Damage { get; private set; }
        
        public void ApplyDamage(ISpendHealth health)
        {
            health.Spend(Damage);
        }

        public void SetDamage(int value)
        {
            Damage = Math.Max(0, value);
        }
    }
}