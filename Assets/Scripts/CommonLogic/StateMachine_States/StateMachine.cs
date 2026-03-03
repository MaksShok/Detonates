using System;
using System.Collections.Generic;
using CommonLogic.StateMachine_States.States;
using UnityEngine;

namespace CommonLogic.StateMachine_States
{
    public class StateMachine
    {
        private static readonly List<StateTransition> EmptyCollection = new List<StateTransition>();
    
        private IState _currentState;
        private List<StateTransition> _currentTransitions;
        private Dictionary<Type, List<StateTransition>> _transitionMap = new Dictionary<Type, List<StateTransition>>();

        public void Update()
        {
            if (_currentState == null || _currentState.CanExit)
            {
                foreach (var transition in _currentTransitions)
                {
                    if (transition.CheckCanGo())
                    {
                        IState toState = transition.To;
                        SetState(toState);
                        break;
                    }
                }
            }
        
            _currentState?.Update(Time.deltaTime);
        }

        public void FixedUpdateState(float fixedDeltaTime)
        {
            _currentState?.FixedUpdate(Time.fixedDeltaTime);
        }

        public void SetState(IState state)
        {
            if (state == _currentState)
                return;
        
            _currentState?.Exit();
            _currentState = state;

            _currentTransitions = _transitionMap.GetValueOrDefault(_currentState.GetType(), EmptyCollection);
            _currentState?.Enter();
        }

        public void AddTransition(IState fromState, IState toState, Func<bool> predicate)
        {
            if (!_transitionMap.TryGetValue(fromState.GetType(), out var transitions))
            {
                List<StateTransition> newTransitions = new List<StateTransition>();
                newTransitions.Add(new StateTransition(toState, predicate));
                _transitionMap.Add(fromState.GetType(), newTransitions);
            }
        
            transitions.Add(new StateTransition(toState, predicate));
        }
    }

    public class StateTransition
    {
        public IState To { get; }
        
        private readonly Func<bool> _condition;

        public StateTransition(IState to, Func<bool> condition)
        {
            To = to;
            _condition = condition;
        }

        public bool CheckCanGo()
        {
            return _condition.Invoke();
        }
    }
}