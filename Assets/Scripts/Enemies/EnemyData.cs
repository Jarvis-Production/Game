using UnityEngine;

namespace TrainSurvival.Enemies
{
    [CreateAssetMenu(menuName = "TrainSurvival/Enemy Data", fileName = "EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName = "Spider";
        public float maxHealth = 45f;
        public float moveSpeed = 4f;
        public float attackRange = 1.5f;
        public float attackDamage = 8f;
        public float attackCooldown = 0.8f;
        public int reward = 8;
    }
}
