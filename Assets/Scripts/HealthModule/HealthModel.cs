using System;

namespace HealthModule
{
    public class HealthModel : ISpendHealth
    {
        public event Action OnDie;
        public bool Alive => Health > 0;
        public int Health { get; private set; }
        
        public HealthModel(int value)
        {
            Set(value);
        }

        public void Spend(int value)
        {
            Set(Health - value);
        }

        public void SetHealth(int value)
        {
            Set(value);
        }

        private void Set(int value)
        {
            Health = Math.Max(0, value);
            if (Health == 0) OnDie?.Invoke();   
        }

        public void ClearAllSubscribers()
        {
            OnDie = null;
        }
    }
}