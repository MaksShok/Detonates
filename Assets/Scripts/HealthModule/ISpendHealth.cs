namespace HealthModule
{
    public interface ISpendHealth
    {
        bool Alive { get; }
        void Spend(int damage);
    }
}