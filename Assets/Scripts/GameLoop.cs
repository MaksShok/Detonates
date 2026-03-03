using EnemyModule;
using LevelModule;

public class GameLoop
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemyWaveGenerator _enemyWaveGenerator;

    public GameLoop(ICoroutineRunner coroutineRunner, EnemyWaveGenerator enemyWaveGenerator)
    {
        _coroutineRunner = coroutineRunner;
        _enemyWaveGenerator = enemyWaveGenerator;
    }

    public void StartGame()
    {
        _enemyWaveGenerator.StopGeneration();
    }
}