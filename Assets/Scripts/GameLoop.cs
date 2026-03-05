using System.Collections;
using HealthModule;
using LevelModule;
using UnityEngine;

public class GameLoop
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemyWaveGenerator _enemyWaveGenerator;
    private readonly BattleGenerationConfig _battleConfig;

    private IHealth _towerHealth;
    private int _breakDurationSec = 5;

    public GameLoop(ICoroutineRunner coroutineRunner, EnemyWaveGenerator enemyWaveGenerator, BattleGenerationConfig battleConfig)
    {
        _coroutineRunner = coroutineRunner;
        _enemyWaveGenerator = enemyWaveGenerator;
        _battleConfig = battleConfig;
    }

    public IEnumerator StartGame()
    {
        int wavesCount = _battleConfig.WaveInfos.Length;
        Coroutine process;
        
        for (int i = 0; i < wavesCount; i++)
        {
            BattleGenerationConfig.WaveInfo waveInfo = _battleConfig.WaveInfos[i];
            process = _enemyWaveGenerator.StartGenerateWaveProcess(waveInfo.Duration, waveInfo.EnemiesSpawnInfo);

            Debug.Log($"Генерируется {i} волна...");
            yield return process;

            Debug.Log($"Волна {i} завершилась.");

            if (i == wavesCount - 1)
                break;
            
            Debug.Log($"Перерыв {_breakDurationSec} секунд...");

            yield return new WaitForSeconds(_breakDurationSec);
        }

        if (_battleConfig.IsLoopLastWave == false)
        {
            // конец игры )))
        }

        BattleGenerationConfig.WaveInfo cycledWaveInfo = _battleConfig.WaveInfos[wavesCount - 1];

        while (_towerHealth.Alive)
        {
            process = _enemyWaveGenerator.StartGenerateWaveProcess(cycledWaveInfo.Duration, cycledWaveInfo.EnemiesSpawnInfo);
            
            Debug.Log($"Генерируется {wavesCount} волна...");
            yield return process;
            
            Debug.Log($"Волна {wavesCount} завершилась.");
            Debug.Log($"Перерыв {_breakDurationSec} секунд...");

            yield return new WaitForSeconds(_breakDurationSec);
        }
        
    }
}