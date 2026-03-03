using System;
using CommonLogic.Conditions;
using CommonLogic.StateMachine_States;
using CommonLogic.StateMachine_States.States;
using DamageModule;
using DamageModule.DamageProvider;
using EnemyModule.Config;
using HealthModule;
using UnityEngine;

namespace EnemyModule
{
    public class EnemyBehavior : MonoBehaviour
    {
        [SerializeField] private PhysicalDamageProvider _damageProvider;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private EnemyConfig _config;

        private Transform _towerTransform;
        private ISpendHealth _towerSpendHealth;
        private StateMachine _stateMachine;
        private HealthModel _healthModel;

        public void Initialize(Transform towerTransform, ISpendHealth towerSpendHealth)
        {
            _towerTransform = towerTransform;
            _towerSpendHealth = towerSpendHealth;

            _healthModel = new HealthModel(_config.Health);
            _damageProvider.Initialize(_healthModel);
            
            _stateMachine = new StateMachine();
            
            var enemyTowerDamage = new SimpleDamage(_config.Damage);
            
            var checkClose = new CheckTwoObjectsClose(transform, towerTransform, 0.5f);

            var followToTowerState = new FollowToPointState(_rb, _towerTransform, _config.MoveSpeed, checkClose);
            var attackState = new AttackState(enemyTowerDamage, transform, _towerTransform, 
                _towerSpendHealth, _config.AttackCooldownSec, checkClose);
            
            _stateMachine.AddTransition(followToTowerState, attackState, () => checkClose.IsClose);
            _stateMachine.AddTransition(attackState, followToTowerState, () => !checkClose.IsClose);
            _stateMachine.SetState(followToTowerState);
        }

        private void Update()
        {
            _stateMachine?.Update();

            if (_healthModel.Alive == false)
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            _stateMachine?.FixedUpdateState(Time.fixedDeltaTime);
        }
        
        
    }
}