using HealthModule;

namespace DamageModule
{
    public interface IDamageProvider
    {
        int Damage { get; }
        void ApplyDamage(ISpendHealth health);
    }
}