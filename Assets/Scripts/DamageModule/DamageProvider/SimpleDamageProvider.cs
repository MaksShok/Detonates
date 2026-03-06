using System;
using HealthModule;

namespace DamageModule.DamageProvider
{
    public class SimpleDamageProvider : IDamageProvider
    {
        public int Damage { get; private set; }

        public SimpleDamageProvider(int defaultValue = 0)
        {
            Damage = Math.Max(0, defaultValue);
        }
        
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