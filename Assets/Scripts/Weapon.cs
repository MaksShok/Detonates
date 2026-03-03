using HealthModule;
using UnityEngine;

namespace DefaultNamespace
{
    public class Weapon : MonoBehaviour, IDamagable
    {
        [SerializeField] private int _damage;
        
        public void ApplyDamage(ISpendHealth health)
        {
            health.Spend(_damage);
        }
    }
}