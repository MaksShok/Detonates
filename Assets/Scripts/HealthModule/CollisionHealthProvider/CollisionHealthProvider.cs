using HealthModule;
using UnityEngine;

namespace DamageModule.DamageProvider
{
    public class CollisionHealthProvider : MonoBehaviour
    {
        [SerializeField] 
        private Collider2D _trigger;

        private ISpendHealth _health;

        public void Initialize(ISpendHealth health)
        {
            _health = health;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent<IDamageProvider>(out var damageProvider) && _health != null)
            {
                damageProvider.ApplyDamage(_health);
            }
        }
    }
}