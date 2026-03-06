using System.Collections;
using System.Collections.Generic;
using EnemyModule;
using UnityEngine;

namespace LevelModule
{
    public class EnemyGenerator
    {
        private readonly Pool<EnemyBehavior> _pool;
        private readonly Transform[] _initialPoints;

        public int ObjectsOnScene => _pool.Size - _pool.AvailableObjectsCount; 
            
        private int _currentInitialPointIndex;

        public EnemyGenerator(Pool<EnemyBehavior> pool, Transform[] initialPoints)
        {
            _pool = pool;
            _initialPoints = initialPoints;
        }
        
        public IEnumerator Generate(int amount, float duration)
        {
            if (duration <= 0 || _initialPoints.Length == 0)
                yield break;
         
            float spawnedIntervalSec = duration / amount;

            for (int i = 0; i < amount; i++)
            {
                var initialPoint = GetNextInitialPoint();
                var enemy = _pool.GetObject();

                if (!_activeEnemies.Contains(enemy)) // Test
                    _activeEnemies.Add(enemy);
                
                SubscribeOnEnemyDie(enemy);
                
                enemy.transform.position = initialPoint.position;
                enemy.transform.rotation = initialPoint.rotation;

                yield return new WaitForSeconds(spawnedIntervalSec);
            }
        }

        private void SubscribeOnEnemyDie(EnemyBehavior enemy)
        {
            void OnEnemyDie()
            {
                _activeEnemies.Remove(enemy); //Test

                enemy.SpendHealth.OnDie -= OnEnemyDie;
                enemy.Reset();
                _pool.ReturnObject(enemy);
            }

            enemy.SpendHealth.OnDie += OnEnemyDie;
        }
        
        private Transform GetNextInitialPoint()
        {
            Transform point = _initialPoints[_currentInitialPointIndex];
            _currentInitialPointIndex = (_currentInitialPointIndex + 1) % _initialPoints.Length;
            return point;
        }

        
        
        private readonly List<EnemyBehavior> _activeEnemies = new (); // Test
        public void KillAllAlive()
        {
            if (_activeEnemies.Count == 0)
                return;

            var snapshot = _activeEnemies.ToArray();
            foreach (var enemy in snapshot)
            {
                if (enemy == null || enemy.SpendHealth == null)
                    continue;

                if (enemy.SpendHealth.Alive)
                {
                    enemy.SpendHealth.Spend(enemy.SpendHealth.Health);
                }
            }
        }
    }
}