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

        public FollowToPointState(Rigidbody2D rb, Transform targetPoint, float moveSpeed, CheckTwoObjectsClose checkClose)
        {
            _rb = rb;
            _targetPoint = targetPoint;
            _moveSpeed = moveSpeed;
            _checkClose = checkClose;
        }

        public void Enter()
        {
        }

        public void Exit() { }

        public void FixedUpdate(float fixedDeltaTime)
        {
            if (_checkClose.IsClose)
                return;
            
            Vector2 currentPosition = _rb.position;
            Vector2 toTarget = (Vector2)_targetPoint.position - currentPosition;
            float distanceToTarget = toTarget.magnitude;

            if (distanceToTarget <= 0f)
                return;

            float maxStep = _moveSpeed * fixedDeltaTime;
            Vector2 step;

            if (maxStep >= distanceToTarget)
            {
                step = toTarget;
            }
            else
            {
                step = toTarget.normalized * maxStep;
            }

            Vector2 newPosition = currentPosition + step;
            _rb.MovePosition(newPosition);
        }

        public void Update(float deltaTime) { }
    }
}