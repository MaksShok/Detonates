using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using EnemyModule;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [Header("Настройки пула врагов")]
    [SerializeField] [Range(0, 1f)]
    private float _percentageOfEnemyCountPullSize;
    [SerializeField] [Range(1, 100)]
    private int _expansionSize;
    
    [Header("Точки генерации врагов")]
    [SerializeField] 
    private Transform[] _enemyInitialPoints;

    public event Action<GameObject> OnEnemySpawned;
    public event Action OnWaveCompleted;
    public event Action<int, int> OnWaveProgress; // spawned, total
    
    public bool IsGenerating => _generateProcess != null;

    private Dictionary<string, Pool> _enemyPools;
    private Coroutine _generateProcess;
    private BattleGenerationConfig _battleConfig;
    private EnemyFactory _enemyFactory;
    private int _currentSpawnPointIndex;
    private Transform _currentInitialPointRef;
    
    private int _totalEnemies; // из LevelConfigReader

    public void Initialize(BattleGenerationConfig battleConfig, EnemyFactory enemyFactory)
    {
        _battleConfig = battleConfig;
        _enemyFactory = enemyFactory;

        // ----- обработку конфига нужно вынести в LevelConfigReader
        var enemyAmountDict = new Dictionary<string, (int amount, EnemyBehavior prefab)>();
        foreach (var waveInfo in _battleConfig.WaveInfos)
        {
            foreach (var enemyInfo in waveInfo.EnemiesSpawnInfo)
            {
                if (enemyInfo.EnemyBehaviorPrefab == null)
                    continue;
                
                string enemyName = enemyInfo.EnemyBehaviorPrefab.name;
                _totalEnemies += enemyInfo.Amount; // из LevelConfigReader
                if (!enemyAmountDict.ContainsKey(enemyName))
                {
                    enemyAmountDict.Add(enemyName, (enemyInfo.Amount, enemyInfo.EnemyBehaviorPrefab));
                }
                else
                {
                    var concreteEnemyAmount = enemyAmountDict[enemyName];
                    concreteEnemyAmount.amount += enemyInfo.Amount;
                }
            }
        }
        // -------
        
        _enemyPools = new Dictionary<string, Pool>();
        foreach (var concreteEnemyAmount in enemyAmountDict)
        { 
            string nameKey = concreteEnemyAmount.Key;
            EnemyBehavior prefab = concreteEnemyAmount.Value.prefab;
            int initialSize = Mathf.FloorToInt(concreteEnemyAmount.Value.amount * _percentageOfEnemyCountPullSize);

            Func<GameObject> createEnemyFunc = () => _enemyFactory.Create(
                prefab, _currentInitialPointRef.position, Quaternion.identity, transform, nameKey).gameObject;
            
            _enemyPools.Add(nameKey, new Pool(createEnemyFunc, initialSize));
            Debug.Log($"Пул для '{nameKey}': {initialSize} / {concreteEnemyAmount.Value.amount} объектов ({_percentageOfEnemyCountPullSize:P0})");
        }
    }

    public void StartGenerateWaveProcess(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
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
        
    private IEnumerator GenerateEnemyCoroutine(float duration, BattleGenerationConfig.EnemySpawnInfo[] enemySpawnInfos)
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
            EnemyBehavior prefab = enemySpawnInfos[totalSpawned].EnemyBehaviorPrefab;
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
    
        //тут должна быть логика --> дожидаемся когда все враги умрут
        
        OnWaveCompleted?.Invoke();
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
}