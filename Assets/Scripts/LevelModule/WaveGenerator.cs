using System;
using System.Collections;
using System.Collections.Generic;
using EnemyModule;
using Factory;
using Unity.VisualScripting;
using UnityEngine;

namespace LevelModule
{
    public class WaveGenerator : MonoBehaviour
    {
        [SerializeField] [Range(0, 1f)]
        private float _percentageOfEnemyCountPullSize;

        [SerializeField] 
        private Transform[] _enemyInitialPoints;

        [SerializeField] 
        private Transform _enemyContainer;
        
        private EnemyFactory _enemyFactory;
        private Coroutine _generateProcess;

        private Dictionary<string, Pool<EnemyBehavior>> _enemyPools = new ();
        private Dictionary<string, EnemyGenerator> _enemyGenerators = new ();

        public void Initialize(BattleGenerationConfig.WaveInfo[] waveInfos, EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;

            ClearAllPools();

            int totalEnemies = 0;
            var enemyAmountDict = new Dictionary<string, (int amount, EnemyBehavior prefab)>();
            foreach (var waveInfo in waveInfos)
            {
                foreach (var enemyInfo in waveInfo.EnemiesSpawnInfo)
                {
                    if (enemyInfo.EnemyBehavior == null)
                        continue;
                
                    string enemyName = enemyInfo.EnemyBehavior.name;
                    totalEnemies += enemyInfo.Amount;
                    if (!enemyAmountDict.ContainsKey(enemyName))
                    {
                        enemyAmountDict.Add(enemyName, (enemyInfo.Amount, enemyInfo.EnemyBehavior));
                    }
                    else
                    {
                        var concreteEnemyAmount = enemyAmountDict[enemyName];
                        concreteEnemyAmount.amount += enemyInfo.Amount;
                        enemyAmountDict[enemyName] = concreteEnemyAmount;
                    }
                }
            }
        
            foreach (var concreteEnemyAmount in enemyAmountDict)
            { 
                string nameKey = concreteEnemyAmount.Key;
                EnemyBehavior prefab = concreteEnemyAmount.Value.prefab;
                int initialSize = Mathf.CeilToInt(concreteEnemyAmount.Value.amount * _percentageOfEnemyCountPullSize);

                Func<EnemyBehavior> createEnemyFunc = () =>
                    _enemyFactory.Create(prefab, Vector3.zero, Quaternion.identity, _enemyContainer, nameKey);

                var pool = new Pool<EnemyBehavior>(createEnemyFunc, initialSize);
                _enemyPools.Add(nameKey, pool);
                _enemyGenerators.Add(nameKey, new EnemyGenerator(pool, _enemyInitialPoints));
                Debug.Log($"Пул для '{nameKey}': {initialSize} / {concreteEnemyAmount.Value.amount} объектов ({_percentageOfEnemyCountPullSize:P0})");
            }
        }

        public Coroutine StartGenerateWaveProcess(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
        {
            StopGeneration();
            _generateProcess = StartCoroutine(GenerateWaveCoroutine(duration, enemySpawnInfos));
            return _generateProcess;
        }

        public void StopGeneration()
        {
            if (_generateProcess != null)
            {
                StopCoroutine(_generateProcess);
                _generateProcess = null;
            }
        }

        private IEnumerator GenerateWaveCoroutine(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
        {
            if (enemySpawnInfos == null || enemySpawnInfos.Length == 0)
                 yield break;

            List<Coroutine> coroutines = new ();
            foreach (var spawnInfo in enemySpawnInfos)
            {
                string nameKey = spawnInfo.EnemyBehavior.name;
                var enemyGenerator = _enemyGenerators[nameKey];
                coroutines.Add(StartCoroutine(enemyGenerator.Generate(spawnInfo.Amount, duration)));
            }
            // Ждем, пока идет генерация
            foreach (var coroutine in coroutines) 
                yield return coroutine;
            
            // Тут ждем, что все сгенерированные объекты умерли (вернулись в пул)
            foreach (var dictElement in _enemyGenerators)
            {
                var generator = dictElement.Value;
                yield return new WaitUntil(() => generator.ObjectsOnScene == 0);
            }
        }

        public void ClearAllPools()
        {
            StopGeneration();

            foreach (var pool in _enemyPools.Values)
            {
                pool.Clear();
            }

            _enemyPools.Clear();
            _enemyGenerators.Clear();
        }
        
        [ContextMenu("Убить всех врагов")] //---Test---
        public void KillAllAliveEnemies() 
        {
            foreach (var generator in _enemyGenerators.Values)
            {
                generator.KillAllAlive();
            }
        }
    }
}