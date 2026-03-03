using CommonLogic.Conditions;
using UnityEngine;

namespace CommonLogic.StateMachine_States.States
{
    public class FollowToPointState : IState
    {
        public bool CanExit => _checkClose.IsClose;

        private readonly Rigidbody2D _rb;
        private readonly Transform _targetPoint;
        private readonly float _moveSpeed;
        private readonly CheckTwoObjectsClose _checkClose;
        
        private Vector2 _startPosition;
        private float _totalDistance;

        public FollowToPointState(Rigidbody2D rb, Transform targetPoint, float moveSpeed, CheckTwoObjectsClose checkClose)
        {
            _rb = rb;
            _targetPoint = targetPoint;
            _moveSpeed = moveSpeed;
            _checkClose = checkClose;
        }

        public void Enter()
        {
            _startPosition = _rb.position;
            _totalDistance = Vector2.Distance(_targetPoint.position, _startPosition);
        }

        public void Exit() { }

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_checkClose.IsClose)
                return;
            
            Vector2 direction = ((Vector2)_targetPoint.position - _rb.position).normalized;
            Vector2 newPosition = _rb.position + direction * _moveSpeed * fixedDeltaTime;
            
            if (Vector2.Distance(newPosition, _startPosition) > _totalDistance)
            {
                newPosition = _targetPoint.position;
            }
            
            _rb.MovePosition(newPosition);
            _rb.transform.LookAt(_targetPoint);
        }

        public void Update(float deltaTime) { }
    }
}