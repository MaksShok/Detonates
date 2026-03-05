using System;
using ReactivePR;
using UnityEngine;

namespace HealthModule
{
    public class HealthModel : ISpendHealth
    {
        public bool Alive => Health > 0;
        public int Health { get; private set; }
        
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
            Health = Mathf.Max(0, value);
        }
    }
}