using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGenerator : MonoBehaviour
{
    [Header("Настройки пула врагов")]
    [SerializeField] [Range(0, 1f)]
    private float _percentageOfEnemyCountPullSize;
    [SerializeField] [Range(1, 100)]
    private int _expansionSize;
    
    [Header("Точки генерации врагов")]
    [SerializeField] 
    private Transform[] _initialPoints;

    public event Action<GameObject> OnEnemySpawned;
    public event Action OnWaveCompleted;
    public event Action<int, int> OnWaveProgress; // spawned, total
    
    public bool IsGenerating => _generateProcess != null;

    private Dictionary<string, EnemyPool> _enemyPools;
    private Coroutine _generateProcess;
    private LevelConfig _levelConfig;
    private int _currentSpawnPointIndex;
    
    private int _totalEnemiesInWave; // из обработчика конфига

    public void Initialize(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;

        // ----- обработку конфига нужно вынести в отдельный класс
        var concreteEnemyAmountDict = new Dictionary<string, (int amount, GameObject prefab)>();
        foreach (var concreteWave in _levelConfig.WaveInfos)
        {
            foreach (var enemyInfo in concreteWave.EnemiesSpawnInfo)
            {
                if (enemyInfo.EnemyPrefab == null)
                    continue;
                
                string enemyName = enemyInfo.EnemyPrefab.name;
                _totalEnemiesInWave += enemyInfo.Amount; // из обработчика конфига
                if (!concreteEnemyAmountDict.ContainsKey(enemyName))
                {
                    concreteEnemyAmountDict.Add(enemyName, (enemyInfo.Amount, enemyInfo.EnemyPrefab));
                }
                else
                {
                    var concreteEnemyAmount = concreteEnemyAmountDict[enemyName];
                    concreteEnemyAmount.amount += enemyInfo.Amount;
                }
            }
        }
        // -------

        _enemyPools = new Dictionary<string, EnemyPool>();
        foreach (var concreteEnemyAmount in concreteEnemyAmountDict)
        {
            string key = concreteEnemyAmount.Key;
            GameObject prefab = concreteEnemyAmount.Value.prefab;
            int initialSize = Mathf.FloorToInt(concreteEnemyAmount.Value.amount * _percentageOfEnemyCountPullSize);
                
            _enemyPools.Add(key, new EnemyPool(prefab, initialSize, _expansionSize));
            Debug.Log($"Пул для '{key}': {initialSize} / {concreteEnemyAmount.Value.amount} объектов ({_percentageOfEnemyCountPullSize:P0})");
        }
    }

    public void StartGenerateWaveProcess(float duration, LevelConfig.EnemySpawnInfo[] enemySpawnInfos)
    {
        StopGeneration();
        _generateProcess = StartCoroutine(GenerateEnemyCoroutine(duration, enemySpawnInfos));
    }
    
    public void StopGeneration()
    {
        if (_generateProcess != null)
        {
            StopCoroutine(_generateProcess);
            _generateProcess = null;
        }
    }
        
    private IEnumerator GenerateEnemyCoroutine(float duration, LevelConfig.EnemySpawnInfo[] enemySpawnInfos)
    {
        if (_totalEnemiesInWave == 0 || duration <= 0f)
        {
            Debug.LogWarning("Нет врагов для генерации или длительность волны равна 0");
            OnWaveCompleted?.Invoke();
            yield break;
        }

        float spawnInterval = duration / _totalEnemiesInWave;
        int totalSpawned = 0;
        
        while (totalSpawned < _totalEnemiesInWave && IsGenerating)
        {
            GameObject prefab = enemySpawnInfos[totalSpawned].EnemyPrefab;
            string enemyName = prefab.name;
            
            if (!_enemyPools.TryGetValue(enemyName, out var pool))
            {
                Debug.LogError($"Пул для врага '{enemyName}' не найден!");
                continue;
            }

            Transform spawnPoint = GetNextSpawnPoint();
            Vector3 spawnPosition = spawnPoint.position;
            
            GameObject enemy = pool.GetObject();
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = spawnPoint.rotation;
            OnEnemySpawned?.Invoke(enemy);
            
            totalSpawned++;
            OnWaveProgress?.Invoke(totalSpawned, _totalEnemiesInWave);
            
            yield return new WaitForSeconds(spawnInterval);
        }

        //yield return WaitForAllEnemiesDie();
        
        OnWaveCompleted?.Invoke();
        print("Волна закончилась");
    }

    private Transform GetNextSpawnPoint()
    {
        Transform point = _initialPoints[_currentSpawnPointIndex];
        _currentSpawnPointIndex = (_currentSpawnPointIndex + 1) % _initialPoints.Length;
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
}