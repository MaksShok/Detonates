using System.Collections;
using HealthModule;
using LevelModule;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameLoop
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly WaveGenerator _waveGenerator;
    private readonly BattleGenerationConfig _battleConfig;
    private readonly IHealth _towerHealth;

    private Coroutine _gameProcess;
    private Coroutine _waveGenerateProcess;

    public GameLoop(ICoroutineRunner coroutineRunner, WaveGenerator waveGenerator, 
        BattleGenerationConfig battleConfig, IHealth towerHealth)
    {
        _coroutineRunner = coroutineRunner;
        _waveGenerator = waveGenerator;
        _battleConfig = battleConfig;
        _towerHealth = towerHealth;
    }

    public void StartGameProcess()
    {
        if (_gameProcess != null)
        {
            return;
        }
        
        _towerHealth.OnDie += Defeat;
        _gameProcess = _coroutineRunner.StartCoroutine(GameProcess());
    }

    public void StopGame()
    {
        if (_gameProcess == null)
            return;

        _towerHealth.OnDie -= Defeat;
        
        _coroutineRunner.StopCoroutine(_waveGenerateProcess);
        _coroutineRunner.StopCoroutine(_gameProcess);
        
        _waveGenerator.ClearAllPools();
    }
    
    private IEnumerator GameProcess()
    {
        int wavesCount = _battleConfig.WaveInfos.Length;
        _waveGenerateProcess = null;
        
        for (int i = 0; i < wavesCount; i++)
        {
            BattleGenerationConfig.WaveInfo waveInfo = _battleConfig.WaveInfos[i];
            _waveGenerateProcess = _waveGenerator.StartGenerateWaveProcess(waveInfo.Duration, waveInfo.EnemiesSpawnInfo);

            Debug.Log($"Генерируется {i+1} волна...");
            yield return _waveGenerateProcess;

            Debug.Log($"Волна {i+1} завершилась.");

            if (i == wavesCount - 1)
                break;
            
            Debug.Log($"Перерыв {_battleConfig.WaveCooldownTimeSec} секунд...");

            yield return new WaitForSeconds(_battleConfig.WaveCooldownTimeSec);
        }

        if (_battleConfig.IsLoopLastWave == false)
        {
            if (_towerHealth.Alive)
            {
                Debug.Log("Игра пройдена");
            }
        }

        yield return new WaitForSeconds(_battleConfig.WaveCooldownTimeSec);
        
        BattleGenerationConfig.WaveInfo cycledWaveInfo = _battleConfig.WaveInfos[wavesCount - 1];
        while (_gameProcess != null)
        {
            _waveGenerateProcess = _waveGenerator.StartGenerateWaveProcess(cycledWaveInfo.Duration, cycledWaveInfo.EnemiesSpawnInfo);
            
            Debug.Log($"Повторяется {wavesCount} волна...");
            yield return _waveGenerateProcess;
            
            Debug.Log($"Волна {wavesCount} завершилась.");
            Debug.Log($"Перерыв {_battleConfig.WaveCooldownTimeSec} секунд...");

            yield return new WaitForSeconds(_battleConfig.WaveCooldownTimeSec);
        }
    }
    
    private void Defeat()
    {
        StopGame();
        
        Debug.Log("Башня разрушена");
        Debug.Log("Игра закончена");
    }
}