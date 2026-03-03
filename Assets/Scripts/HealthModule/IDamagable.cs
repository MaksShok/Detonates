namespace HealthModule
{
    public interface IDamagable
    {
        void ApplyDamage(ISpendHealth health);
    }

    public class SimpleDamage : IDamagable
    {
        private readonly int _damage;

        public SimpleDamage(int damage)
        {
            _damage = damage;
        }

        public void ApplyDamage(ISpendHealth health)
        {
            health.Spend(_damage);
        }
    }
}