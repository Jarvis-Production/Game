using System;
using UnityEngine;
using TrainSurvival.Core;

namespace TrainSurvival.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private PlayerStats stats;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => stats.BaseMaxHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDied;

        private void Start()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;

            float reduced = amount * (1f - stats.DamageReduction);
            CurrentHealth = Mathf.Max(0f, CurrentHealth - reduced);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0f)
                OnDied?.Invoke();
        }

        public void HealFull()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void RefreshMaxHealthAndHeal(float bonusAmount)
        {
            stats.AddMaxHealth(bonusAmount);
            HealFull();
        }
    }
}
