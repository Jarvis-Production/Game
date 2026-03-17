using System.Collections;
using UnityEngine;
using TrainSurvival.Core;
using TrainSurvival.Enemies;
using TrainSurvival.Player;
using TrainSurvival.UI;

namespace TrainSurvival.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponData weaponData;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform muzzle;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private TrailRenderer bulletTrailPrefab;
        [SerializeField] private float spread = 0.01f;

        private int ammoInMag;
        private float nextFireTime;
        private bool isReloading;

        public WeaponData Data => weaponData;
        public int AmmoInMag => ammoInMag;

        private void Start()
        {
            ammoInMag = weaponData.magazineSize;
            UIManager.Instance?.UpdateAmmo(ammoInMag, weaponData.magazineSize);
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
                return;

            if (Input.GetMouseButton(0))
                TryShoot();

            if (Input.GetKeyDown(KeyCode.R))
                TryReload();
        }

        public void IncreaseDamage(float amount) => weaponData.damage += amount;
        public void IncreaseFireRate(float amount) => weaponData.fireRate += amount;
        public void ReduceReloadTime(float amount) => weaponData.reloadTime = Mathf.Max(0.25f, weaponData.reloadTime - amount);
        public void IncreaseMagazine(int amount)
        {
            weaponData.magazineSize += amount;
            ammoInMag = Mathf.Min(ammoInMag + amount, weaponData.magazineSize);
            UIManager.Instance?.UpdateAmmo(ammoInMag, weaponData.magazineSize);
        }

        private void TryShoot()
        {
            if (isReloading || Time.time < nextFireTime)
                return;

            if (ammoInMag <= 0)
            {
                TryReload();
                return;
            }

            ammoInMag--;
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            FireHitscan();
            UIManager.Instance?.UpdateAmmo(ammoInMag, weaponData.magazineSize);
        }

        private void FireHitscan()
        {
            muzzleFlash?.Play();

            Vector3 dir = playerController.PlayerCamera.transform.forward;
            dir += Random.insideUnitSphere * spread;

            Vector3 origin = playerController.PlayerCamera.transform.position;
            Vector3 hitPoint = origin + dir * weaponData.range;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, weaponData.range, weaponData.hitMask))
            {
                hitPoint = hit.point;
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(weaponData.damage);

                SpiderEnemy enemy = hit.collider.GetComponentInParent<SpiderEnemy>();
                if (enemy != null && enemy.WasKilledByLastHit)
                    GameManager.Instance.CurrencySystem.Add(weaponData.currencyPerKill);
            }

            if (bulletTrailPrefab != null)
                StartCoroutine(SpawnTrail(muzzle.position, hitPoint));
        }

        private IEnumerator SpawnTrail(Vector3 from, Vector3 to)
        {
            TrailRenderer trail = Instantiate(bulletTrailPrefab, from, Quaternion.identity);
            float t = 0f;
            while (t < 1f)
            {
                trail.transform.position = Vector3.Lerp(from, to, t);
                t += Time.deltaTime * 80f;
                yield return null;
            }

            trail.transform.position = to;
            Destroy(trail.gameObject, trail.time);
        }

        private void TryReload()
        {
            if (isReloading || ammoInMag >= weaponData.magazineSize)
                return;

            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            isReloading = true;
            UIManager.Instance?.SetWaveState("Reloading...");
            yield return new WaitForSeconds(weaponData.reloadTime);
            ammoInMag = weaponData.magazineSize;
            UIManager.Instance?.UpdateAmmo(ammoInMag, weaponData.magazineSize);
            isReloading = false;
        }
    }
}
