using UnityEngine;

namespace EnemyModule.Config
{
    [CreateAssetMenu(menuName = "ScriptableObjects/EnemyConfig", fileName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject 
    {
        [field: SerializeField]
        public int Health { get; private set; }
        
        [field: SerializeField]
        public int Damage { get; private set; }
        
        [field: SerializeField]
        public int AttackCooldownSec { get; private set; }
        
        [field: SerializeField]
        public int MoveSpeed { get; private set; }
    }
}