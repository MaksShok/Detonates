using System.Collections.Generic;

namespace DefaultNamespace.Config
{
    public class LevelConfigReader
    {
        public Dictionary<string, EnemyInfo> _enemyInfo = new Dictionary<string, EnemyInfo>();

        private readonly BattleGenerationConfig _battleConfig;
        public LevelConfigReader(BattleGenerationConfig battleConfig)
        {
            _battleConfig = battleConfig;
        }
        
        public struct EnemyInfo
        {
            public int Amount { get; }

            public EnemyInfo(int amount)
            {
                Amount = amount;
            }
        }
    }
}