using UnityEngine;

namespace TrainSurvival.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private float baseMoveSpeed = 6f;
        [SerializeField] private float baseDamageReduction = 0f;
        [SerializeField] private float baseMaxHealth = 100f;

        public float MoveSpeedBonus { get; private set; }
        public float DamageReductionBonus { get; private set; }

        public float MoveSpeed => baseMoveSpeed + MoveSpeedBonus;
        public float DamageReduction => Mathf.Clamp01(baseDamageReduction + DamageReductionBonus);
        public float BaseMaxHealth => baseMaxHealth;

        public void AddMoveSpeed(float value) => MoveSpeedBonus += value;
        public void AddDamageReduction(float value) => DamageReductionBonus += value;
        public void AddMaxHealth(float amount) => baseMaxHealth += amount;
    }
}
