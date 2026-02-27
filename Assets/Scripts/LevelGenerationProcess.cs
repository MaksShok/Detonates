using System.Collections;
using UnityEngine;

public class LevelGenerationProcess : MonoBehaviour
{
    public bool IsGenerate => _generateProcess != null;
    
    private GameObject _tower;
    private LevelConfig _levelConfig;
    private Coroutine _generateProcess;
    
    public void StartGenerate()
    {
        if (IsGenerate)
            return;

        //_generateProcess = StartCoroutine(GenerateCoroutine());
    }
    
    public void StopGenerate()
    {
        if (!IsGenerate)
            return;

        StopCoroutine(_generateProcess);
        _generateProcess = null;
    }
    
    // private IEnumerator GenerateCoroutine()
    // {
    //     if (_levelConfig.WaveInfos.Length == 0)
    //         StopGenerate();
    //
    //     foreach (var waveInfo in _levelConfig.WaveInfos)
    //     {
    //         float duration = waveInfo.Duration;
    //     }
    //     
    // }
}

[CreateAssetMenu(menuName = "ScriptableObjects/", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
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
        public GameObject EnemyPrefab { get; private set; }
        
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