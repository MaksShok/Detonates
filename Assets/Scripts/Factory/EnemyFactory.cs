using EnemyModule;
using HealthModule;
using UnityEngine;

namespace Factory
{
    public interface IFactory<T> where T : Object
    {
        T Create(T prefab, Vector3 position, Quaternion rotation, Transform root, string defaultName = null);
    }

    public class EnemyFactory : IFactory<EnemyBehavior>
    {
        private readonly Transform _towerTransform;
        private readonly ISpendHealth _towerSpendHealth;
        public EnemyFactory(Transform towerTransform, ISpendHealth towerSpendHealth)
        {
            _towerTransform = towerTransform;
            _towerSpendHealth = towerSpendHealth;
        }
        
        public EnemyBehavior Create(EnemyBehavior prefab, Vector3 position, Quaternion rotation,
            Transform root, string defaultName)
        {
            var instance = Object.Instantiate(prefab, position, rotation, root);
            instance.Initialize(_towerTransform, _towerSpendHealth);
            instance.name = defaultName;
            return instance;
        }
    }
}