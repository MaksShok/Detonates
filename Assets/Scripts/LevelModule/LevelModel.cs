using UnityEngine;

namespace LevelModule
{
    public class LevelModel : MonoBehaviour
    {
        [field: SerializeField] 
        public Transform TowerTransform { get; private set; }
        
        [field: SerializeField] 
        public int TowerHealth { get; private set; }
        
        [field: SerializeField] 
        public WaveGenerator WaveGenerator { get; private set; }
    }
}