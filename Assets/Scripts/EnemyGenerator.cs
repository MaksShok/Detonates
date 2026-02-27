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
    
    private int _totalEnemies; // из LevelConfigReader

    public void Initialize(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;

        // ----- обработку конфига нужно вынести в LevelConfigReader
        var enemyAmountDict = new Dictionary<string, (int amount, GameObject prefab)>();
        foreach (var waveInfo in _levelConfig.WaveInfos)
        {
            foreach (var enemyInfo in waveInfo.EnemiesSpawnInfo)
            {
                if (enemyInfo.EnemyPrefab == null)
                    continue;
                
                string enemyName = enemyInfo.EnemyPrefab.name;
                _totalEnemies += enemyInfo.Amount; // из LevelConfigReader
                if (!enemyAmountDict.ContainsKey(enemyName))
                {
                    enemyAmountDict.Add(enemyName, (enemyInfo.Amount, enemyInfo.EnemyPrefab));
                }
                else
                {
                    var concreteEnemyAmount = enemyAmountDict[enemyName];
                    concreteEnemyAmount.amount += enemyInfo.Amount;
                }
            }
        }
        // -------

        _enemyPools = new Dictionary<string, EnemyPool>();
        foreach (var concreteEnemyAmount in enemyAmountDict)
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
        if (_totalEnemies == 0 || duration <= 0f)
        {
            Debug.LogWarning("Нет врагов для генерации или длительность волны равна 0");
            OnWaveCompleted?.Invoke();
            yield break;
        }

        float spawnInterval = duration / _totalEnemies;
        int totalSpawned = 0;
        
        while (totalSpawned < _totalEnemies && IsGenerating)
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
            OnWaveProgress?.Invoke(totalSpawned, _totalEnemies);
            
            yield return new WaitForSeconds(spawnInterval);
        }

        //дожидаемся когда все враги умрут
        
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