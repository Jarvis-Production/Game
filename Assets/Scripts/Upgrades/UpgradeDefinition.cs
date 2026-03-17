using UnityEngine;

namespace TrainSurvival.Upgrades
{
    [CreateAssetMenu(menuName = "TrainSurvival/Upgrade Definition", fileName = "Upgrade")]
    public class UpgradeDefinition : ScriptableObject
    {
        public string displayName;
        [TextArea] public string description;
        public UpgradeType type;
        public int baseCost = 40;
        public int costGrowth = 20;
        public float valuePerLevel = 5f;
        public int maxLevel = 5;
    }
}
