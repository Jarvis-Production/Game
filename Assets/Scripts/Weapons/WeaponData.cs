using UnityEngine;

namespace TrainSurvival.Weapons
{
    [CreateAssetMenu(menuName = "TrainSurvival/Weapon Data", fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Pistol";
        public float damage = 20f;
        public float fireRate = 4f;
        public float range = 80f;
        public int magazineSize = 12;
        public float reloadTime = 1.35f;
        public LayerMask hitMask;
        public int currencyPerKill = 8;
    }
}
