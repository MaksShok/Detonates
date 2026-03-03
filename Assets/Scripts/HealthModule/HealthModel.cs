using ReactivePR;
using UnityEngine;

namespace HealthModule
{
    public class HealthModel : ISpendHealth
    {
        public int Health { get; private set; }
        public bool Alive => Health > 0;
        
        public HealthModel(int value)
        {
            Health = Mathf.Max(0, value);
        }

        public void Spend(int value)
        {
            Health = Mathf.Max(0, Health - value);
        }

        public void SetHealth(int value)
        {
            Health = value;
        }
    }
}