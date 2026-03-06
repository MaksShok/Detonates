using UnityEngine;

namespace CommonLogic.Conditions
{
    public class CheckTwoObjectsClose
    {
        public bool IsClose {
            get {
                float distanceToTarget = Vector2.Distance(_obj1.position, _obj2.position);
                return distanceToTarget <= _distanceTolerance;
            }
        }
        
        private readonly Transform _obj1;
        private readonly Transform _obj2;
        private readonly float _distanceTolerance;
        
        public CheckTwoObjectsClose(Transform obj1, Transform obj2, float toleranceInUnit)
        {
            _obj1 = obj1;
            _obj2 = obj2;
            _distanceTolerance = Mathf.Max(toleranceInUnit, 0);
        }
    }
}