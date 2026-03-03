namespace HealthModule
{
    public interface IHealth
    {
        public int Health { get; }
        bool Alive { get; }
    }
}