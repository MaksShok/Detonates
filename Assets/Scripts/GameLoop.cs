using EnemyModule;
using LevelModule;

public class GameLoop
{
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemyGenerator _enemyGenerator;

    public GameLoop(ICoroutineRunner coroutineRunner, EnemyGenerator enemyGenerator)
    {
        _coroutineRunner = coroutineRunner;
        _enemyGenerator = enemyGenerator;
    }

    public void StartGame()
    {
        _enemyGenerator.StopGeneration();
    }
}