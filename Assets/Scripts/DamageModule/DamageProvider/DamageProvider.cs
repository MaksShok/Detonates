using System;
using HealthModule;

namespace DamageModule.DamageProvider
{
    public class DamageProvider
    {
        public bool HasCurrentDamage => Damage > 0;
        public int Damage { get; private set; }

        public void ApplyDamage(ISpendHealth spendHealth)
        {
            spendHealth.Spend(Damage);
            Damage = 0;
        }

        public void SetDamage(int damage)
        {
            Damage = Math.Clamp(damage, 0, Int32.MaxValue);
        }

        public void ResetDamage()
        {
            Damage = 0;
        }
    }
}