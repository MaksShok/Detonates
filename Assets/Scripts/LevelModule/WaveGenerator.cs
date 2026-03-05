using System;
using System.Collections;
using System.Collections.Generic;
using EnemyModule;
using Factory;
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

        public event Action<int, int> OnWaveProgress; // spawned, total

        private EnemyFactory _enemyFactory;
        
        private Dictionary<string, Pool<EnemyBehavior>> _enemyPools = new ();
        private Coroutine _generateProcess;
        private int _currentSpawnPointIndex;
        private int _totalEnemies;
        private int _aliveEnemiesInWave;

        public void Initialize(EnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;

            // ----- обработку конфига нужно вынести в LevelConfigReader
            var enemyAmountDict = new Dictionary<string, (int amount, EnemyBehavior prefab)>();
            foreach (var waveInfo in _battleConfig.WaveInfos)
            {
                foreach (var enemyInfo in waveInfo.EnemiesSpawnInfo)
                {
                    if (enemyInfo.EnemyBehavior == null)
                        continue;
                
                    string enemyName = enemyInfo.EnemyBehavior.name;
                    _totalEnemies += enemyInfo.Amount; // из LevelConfigReader
                    if (!enemyAmountDict.ContainsKey(enemyName))
                    {
                        enemyAmountDict.Add(enemyName, (enemyInfo.Amount, enemyInfo.EnemyBehavior));
                    }
                    else
                    {
                        var concreteEnemyAmount = enemyAmountDict[enemyName];
                        concreteEnemyAmount.amount += enemyInfo.Amount;
                    }
                }
            }
            // -------
        
            foreach (var concreteEnemyAmount in enemyAmountDict)
            { 
                string nameKey = concreteEnemyAmount.Key;
                EnemyBehavior prefab = concreteEnemyAmount.Value.prefab;
                int initialSize = Mathf.CeilToInt(concreteEnemyAmount.Value.amount * _percentageOfEnemyCountPullSize);

                Func<EnemyBehavior> createEnemyFunc = () =>
                {
                    var enemy = _enemyFactory.Create(prefab, Vector3.zero, Quaternion.identity, _enemyContainer,
                            nameKey);
                    
                    return enemy;
                };
            
                _enemyPools.Add(nameKey, new Pool<EnemyBehavior>(createEnemyFunc, initialSize));
                Debug.Log($"Пул для '{nameKey}': {initialSize} / {concreteEnemyAmount.Value.amount} объектов ({_percentageOfEnemyCountPullSize:P0})");
            }
        }

        public Coroutine StartGenerateWaveProcess(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
        {
            StopGeneration();
            _generateProcess = StartCoroutine(GenerateEnemyCoroutine(duration, enemySpawnInfos));
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

        private IEnumerator GenerateEnemyCoroutine(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
        {
            if (enemySpawnInfos == null || enemySpawnInfos.Length == 0 || duration <= 0f)
            {
                Debug.LogWarning("Нет врагов для генерации или длительность волны равна 0");
                yield break;
            }
    
            int enemiesInWave = 0;
            for (int i = 0; i < enemySpawnInfos.Length; i++)
            {
                enemiesInWave += Mathf.Max(0, enemySpawnInfos[i].Amount);
            }

            if (enemiesInWave == 0)
            {
                Debug.LogWarning("В текущей волне количество врагов равно 0");
                yield break;
            }

            int totalSpawned = 0;
            _aliveEnemiesInWave = 0;

            var spawnedPerType = new int[enemySpawnInfos.Length];
            float elapsed = 0f;

            while (totalSpawned < enemiesInWave)
            {
                int chosenIndex = -1;
                float chosenTime = float.PositiveInfinity;

                for (int i = 0; i < enemySpawnInfos.Length; i++)
                {
                    int amount = Mathf.Max(0, enemySpawnInfos[i].Amount);

                    if (spawnedPerType[i] >= amount)
                        continue;

                    float nextTime = (spawnedPerType[i] + 1) * (duration / amount);
                    if (nextTime < chosenTime)
                    {
                        chosenTime = nextTime;
                        chosenIndex = i;
                    }
                }

                if (chosenIndex < 0)
                {
                    Debug.LogWarning("Не удалось выбрать врага для спавна: проверь EnemySpawnInfo в конфиге");
                    break;
                }

                float waitTime = chosenTime - elapsed;
                if (waitTime > 0f)
                {
                    yield return new WaitForSeconds(waitTime);
                    elapsed = chosenTime;
                }

                EnemyBehavior prefab = enemySpawnInfos[chosenIndex].EnemyBehavior;
                string enemyName = prefab.name;
            
                if (!_enemyPools.TryGetValue(enemyName, out var pool))
                {
                    Debug.LogError($"Пул для врага '{enemyName}' не найден!");
                    continue;
                }
    
                Transform spawnPoint = GetNextSpawnPoint();
                EnemyBehavior enemy = pool.GetObject();
                enemy.ResetForReuse();
                enemy.Died -= OnEnemyDied;
                enemy.Died += OnEnemyDied;
                enemy.transform.position = spawnPoint.position;
                enemy.transform.rotation = spawnPoint.rotation;
                _aliveEnemiesInWave++;
                
                spawnedPerType[chosenIndex]++;
                totalSpawned++;
                OnWaveProgress?.Invoke(totalSpawned, enemiesInWave);
            }
    
            while (_aliveEnemiesInWave > 0)
            {
                yield return null;
            }

            print("Волна закончилась");
        }
        
        

        private Transform GetNextSpawnPoint()
        {
            Transform point = _enemyInitialPoints[_currentSpawnPointIndex];
            _currentSpawnPointIndex = (_currentSpawnPointIndex + 1) % _enemyInitialPoints.Length;
            return point;
        }

        public void ClearAllPools()
        {
            foreach (var pool in _enemyPools.Values)
            {
                pool.Clear();
            }
            _enemyPools.Clear();
        }

        private void OnEnemyDied(EnemyBehavior enemy)
        {
            if (enemy == null)
                return;

            if (_enemyPools.TryGetValue(enemy.name, out var pool))
            {
                pool.ReturnObject(enemy);
            }
            else
            {
                Debug.LogWarning($"Не найден пул для врага с именем '{enemy.name}' при возврате в пул");
            }

            if (_aliveEnemiesInWave > 0)
            {
                _aliveEnemiesInWave--;
            }
        }
    }
}