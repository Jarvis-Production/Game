using UnityEngine;

namespace TrainSurvival.Waves
{
    [CreateAssetMenu(menuName = "TrainSurvival/Wave Config", fileName = "WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public int baseEnemyCount = 6;
        public int enemiesAddedPerWave = 3;
        public float hpMultiplierPerWave = 0.15f;
        public float speedMultiplierPerWave = 0.06f;
        public float damageMultiplierPerWave = 0.08f;
        public int rewardBase = 40;
        public int rewardGrowth = 15;
    }
}
