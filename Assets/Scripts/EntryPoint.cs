using System;
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
    private WaveGenerator _waveGenerator;

    private void Start()
    {
        // открытие шторки загрузки
        LoadSave();
        Initialization();

        _gameLoop.StartGameProcess();
    }

    private void Initialization()
    {
        HealthModel towerHealth = new HealthModel(_levelModel.TowerHealth);
        EnemyFactory enemyFactory = new EnemyFactory(_levelModel.TowerTransform, towerHealth);

        _waveGenerator = _levelModel.WaveGenerator;
        _waveGenerator.Initialize(_battleConfig.WaveInfos, enemyFactory); // возможно делать асинхронно, хз
            
        _gameLoop = new GameLoop(this, _waveGenerator, _battleConfig, towerHealth);
    }

    private void LoadSave() { }
}