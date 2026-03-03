namespace EnemyModule
{
    public interface IState
    {
        bool CanExit { get; }
        void Enter();
        void Exit();
        void Update(float deltaTime);
        void FixedUpdate(float fixedDeltaTime);
    }
}