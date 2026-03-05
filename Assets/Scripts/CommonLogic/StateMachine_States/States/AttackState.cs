using CommonLogic.Conditions;
using DamageModule;
using HealthModule;

namespace CommonLogic.StateMachine_States.States
{
    public class AttackState : IState
    {
        public bool CanExit => !_spendHealth.Alive || !_checkClose.IsClose;

        private readonly IDamageProvider _damageProvider;
        private readonly ISpendHealth _spendHealth;
        private readonly float _attackCooldown;
        private readonly CheckTwoObjectsClose _checkClose;

        private float _distanceToTarget;
        private float _timeSinceFromLastAttack;

        public AttackState(IDamageProvider damageProvider, ISpendHealth spendHealth, 
            float attackCooldown, CheckTwoObjectsClose checkClose)
        {
            _damageProvider = damageProvider;
            _spendHealth = spendHealth;
            _attackCooldown = attackCooldown;
            _checkClose = checkClose;
        }

        public void Enter()
        {
            _timeSinceFromLastAttack = 0;
        }

        public void Update(float deltaTime)
        {
            _timeSinceFromLastAttack += deltaTime;
            
            // если куллдаун атаки прошел и мы рядом с противником то бъем его
            if (_timeSinceFromLastAttack >= _attackCooldown && _checkClose.IsClose)
            {
                _damageProvider.ApplyDamage(_spendHealth);
                _timeSinceFromLastAttack = 0;
            }
        }

        public void Exit() { }

        public void FixedUpdate(float fixedDeltaTime) { }
    }
}