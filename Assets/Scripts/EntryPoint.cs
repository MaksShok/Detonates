using EnemyModule;
using Factory;
using HealthModule;
using LevelModule;
using UnityEngine;

public class EntryPoint : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] 
    private LevelModel _levelModel;
    [SerializeField]
    private BattleGenerationConfig _battleConfig;

    private GameLoop _gameLoop;
    private EnemyWaveGenerator _enemyWaveGenerator;
        
    private void Start()
    {
        // открытие шторки загрузки
        LoadSave();
        Initialization();
    }

    private void Initialization()
    {
        HealthModel towerHealth = new HealthModel(_levelModel.TowerHealth);
        EnemyFactory enemyFactory = new EnemyFactory(_levelModel.TowerTransform, towerHealth);

        _enemyWaveGenerator = _levelModel.EnemyWaveGenerator;
        _enemyWaveGenerator.Initialize(_battleConfig, enemyFactory); // возможно делать асинхронно, хз
            
        _gameLoop = new GameLoop(this, _enemyWaveGenerator);
    }

    private void LoadSave() { }
}