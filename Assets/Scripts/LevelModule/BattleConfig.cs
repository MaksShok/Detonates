using System;
using EnemyModule;
using UnityEngine;

namespace LevelModule
{
    [CreateAssetMenu(menuName = "ScriptableObjects/BattleConfig", fileName = "BattleConfig")]
    public class BattleGenerationConfig : ScriptableObject
    {
        [field: SerializeField] 
        public bool IsLoopLastWave { get; private set; }
    
        [field: SerializeField] 
        public float WaveCooldownTimeSec { get; private set; }

        [field: SerializeField]
        public WaveInfo[] WaveInfos { get; private set; }

        [Serializable]
        public struct WaveInfo
        {
            [field: SerializeField]
            public float Duration { get; private set; }

            [field: SerializeField]
            public EnemySpawnInfo[] EnemiesSpawnInfo { get; private set; }

            [field: SerializeField]
            public InteractableItems[] InteractableItems { get; private set; }
        }
    
        [Serializable]
        public struct EnemySpawnInfo
        {
            [field: SerializeField]
            public EnemyBehavior EnemyBehaviorPrefab { get; private set; }
        
            [field: SerializeField]
            public int Amount { get; private set; }
        }
    
        [Serializable]
        public struct InteractableItems
        {
            [field: SerializeField]
            public GameObject ItemPrefab { get; private set; }
        
            [field: SerializeField]
            public float SecondsCooldown { get; private set; }
        }
    }
}