using System;
using CommonLogic.Conditions;
using CommonLogic.StateMachine_States;
using CommonLogic.StateMachine_States.States;
using DamageModule;
using DamageModule.DamageProvider;
using EnemyModule.Config;
using HealthModule;
using HealthModule.CollisionHealthProvider;
using UnityEngine;

namespace EnemyModule
{
    public class EnemyBehavior : MonoBehaviour
    {
        [SerializeField] private CollisionHealthProvider _collisionHealthProvider;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private EnemyConfig _config;

        public ISpendHealth SpendHealth => _healthModel;
        private HealthModel _healthModel;
        
        private Transform _towerTransform;
        private ISpendHealth _towerSpendHealth;
        
        private StateMachine _stateMachine;

        public void Initialize(Transform towerTransform, ISpendHealth towerSpendHealth)
        {
            _towerTransform = towerTransform;
            _towerSpendHealth = towerSpendHealth;

            _healthModel = new HealthModel(_config.Health);
            _collisionHealthProvider.Initialize(_healthModel);

            _stateMachine = new StateMachine();
            
            var enemyTowerDamage = new SimpleDamageProvider(_config.Damage);
            
            var checkClose = new CheckTwoObjectsClose(transform, towerTransform, 0.5f);

            var followToTowerState = new FollowToPointState(_rb, _towerTransform, _config.MoveSpeed, checkClose);
            var attackState = new AttackState(enemyTowerDamage, _towerSpendHealth, _config.AttackCooldownSec, checkClose);
            
            _stateMachine.AddTransition(followToTowerState, attackState, () => checkClose.IsClose);
            _stateMachine.AddTransition(attackState, followToTowerState, () => !checkClose.IsClose);
            _stateMachine.SetState(followToTowerState);
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine?.FixedUpdateState(Time.fixedDeltaTime);
        }

        public void Reset()
        {
            _healthModel.SetHealth(_config.Health);

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        private void OnDestroy()
        {
            _healthModel.ClearAllSubscribers();
        }
    }
}