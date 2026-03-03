using EnemyModule;
using UnityEngine;

namespace LevelModule
{
    public class LevelModel : MonoBehaviour
    {
        [Header("Башня")]
        [field: SerializeField] 
        public Transform TowerTransform { get; private set; }
        
        [field: SerializeField] 
        public int TowerHealth { get; private set; }
        
        [Header("Генерация врагов")]
        [field: SerializeField] 
        public EnemyGenerator EnemyGenerator { get; private set; }
        
        [field: SerializeField]
        public BattleGenerationConfig BattleStructure { get; private set; }
    }
}