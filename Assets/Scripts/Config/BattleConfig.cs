using DefaultNamespace;
using EnemyModule;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects", fileName = "BattleConfig")]
public class BattleGenerationConfig : ScriptableObject
{
    [field: SerializeField] 
    public bool IsLoopLastWave { get; private set; }
    
    [field: SerializeField] 
    public float WaveCooldownTimeSec { get; private set; }

    [field: SerializeField]
    public WaveInfo[] WaveInfos { get; private set; }

    public struct WaveInfo
    {
        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public EnemySpawnInfo[] EnemiesSpawnInfo { get; private set; }

        [field: SerializeField]
        public InteractableItems[] InteractableItems { get; private set; }
    }

    public struct EnemySpawnInfo
    {
        [field: SerializeField]
        public EnemyBehavior EnemyBehaviorPrefab { get; private set; }
        
        [field: SerializeField]
        public int Amount { get; private set; }
    }
    
    public struct InteractableItems
    {
        [field: SerializeField]
        public GameObject ItemPrefab { get; private set; }
        
        [field: SerializeField]
        public float SecondsCooldown { get; private set; }
    }
}