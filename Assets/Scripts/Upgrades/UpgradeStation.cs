using System.Collections.Generic;
using UnityEngine;
using TrainSurvival.Core;
using TrainSurvival.Player;
using TrainSurvival.Weapons;
using TrainSurvival.UI;

namespace TrainSurvival.Upgrades
{
    public class UpgradeStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private string stationName = "Upgrade Station";
        [SerializeField] private List<UpgradeDefinition> upgrades;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private WeaponController weaponController;

        private readonly Dictionary<UpgradeDefinition, int> levels = new();

        public string GetPrompt() => $"Press [E] to open {stationName}";

        public void Interact()
        {
            UIManager.Instance.OpenUpgradePanel(stationName, BuildLines(), TryPurchaseUpgrade);
        }

        private List<string> BuildLines()
        {
            var lines = new List<string>();
            foreach (var upgrade in upgrades)
            {
                int level = levels.GetValueOrDefault(upgrade, 0);
                int cost = upgrade.baseCost + level * upgrade.costGrowth;
                string suffix = level >= upgrade.maxLevel ? "MAX" : $"Lv {level}/{upgrade.maxLevel} - ${cost}";
                lines.Add($"{upgrade.displayName} | {upgrade.description} | {suffix}");
            }

            return lines;
        }

        private void TryPurchaseUpgrade(int index)
        {
            if (index < 0 || index >= upgrades.Count)
                return;

            UpgradeDefinition upgrade = upgrades[index];
            int level = levels.GetValueOrDefault(upgrade, 0);
            if (level >= upgrade.maxLevel)
                return;

            int cost = upgrade.baseCost + level * upgrade.costGrowth;
            if (!GameManager.Instance.CurrencySystem.TrySpend(cost))
                return;

            levels[upgrade] = level + 1;
            ApplyUpgrade(upgrade);
            UIManager.Instance.OpenUpgradePanel(stationName, BuildLines(), TryPurchaseUpgrade);
        }

        private void ApplyUpgrade(UpgradeDefinition upgrade)
        {
            float amount = upgrade.valuePerLevel;
            switch (upgrade.type)
            {
                case UpgradeType.PlayerMaxHealth:
                    playerHealth.RefreshMaxHealthAndHeal(amount);
                    break;
                case UpgradeType.PlayerMoveSpeed:
                    playerStats.AddMoveSpeed(amount);
                    break;
                case UpgradeType.PlayerDamageReduction:
                    playerStats.AddDamageReduction(amount / 100f);
                    break;
                case UpgradeType.WeaponDamage:
                    weaponController.IncreaseDamage(amount);
                    break;
                case UpgradeType.WeaponFireRate:
                    weaponController.IncreaseFireRate(amount / 10f);
                    break;
                case UpgradeType.WeaponReload:
                    weaponController.ReduceReloadTime(amount / 10f);
                    break;
                case UpgradeType.WeaponMagazine:
                    weaponController.IncreaseMagazine(Mathf.RoundToInt(amount));
                    break;
            }
        }
    }
}
