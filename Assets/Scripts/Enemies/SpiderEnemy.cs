using UnityEngine;
using TrainSurvival.Core;
using TrainSurvival.Player;
using TrainSurvival.Waves;

namespace TrainSurvival.Enemies
{
    [RequireComponent(typeof(CharacterController))]
    public class SpiderEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private ParticleSystem deathVfx;

        private CharacterController controller;
        private Transform target;
        private PlayerHealth playerHealth;
        private float currentHealth;
        private float attackTimer;
        private float moveSpeed;
        private float attackDamage;
        private float attackRange;
        private float attackCooldown;
        private Vector3 velocity;

        public bool WasKilledByLastHit { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            currentHealth = enemyData.maxHealth;
            moveSpeed = enemyData.moveSpeed;
            attackDamage = enemyData.attackDamage;
            attackRange = enemyData.attackRange;
            attackCooldown = enemyData.attackCooldown;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                playerHealth = GameManager.Instance.PlayerHealth;

            target = playerHealth.transform;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
                return;

            if (target == null)
                return;

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance > attackRange)
            {
                Vector3 desired = toTarget.normalized * moveSpeed;
                velocity = Vector3.Lerp(velocity, desired, acceleration * Time.deltaTime);
                controller.Move(velocity * Time.deltaTime);
                transform.forward = Vector3.Lerp(transform.forward, toTarget.normalized, 12f * Time.deltaTime);
            }
            else
            {
                velocity = Vector3.zero;
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    attackTimer = attackCooldown;
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

        public void ApplyWaveModifiers(float hpMultiplier, float speedMultiplier, float damageMultiplier)
        {
            currentHealth = enemyData.maxHealth * hpMultiplier;
            moveSpeed = enemyData.moveSpeed * speedMultiplier;
            attackDamage = enemyData.attackDamage * damageMultiplier;
        }

        public void TakeDamage(float amount)
        {
            WasKilledByLastHit = false;
            currentHealth -= amount;
            if (currentHealth > 0f)
                return;

            WasKilledByLastHit = true;
            if (deathVfx != null)
                Instantiate(deathVfx, transform.position + Vector3.up * 0.5f, Quaternion.identity);

            GameManager.Instance.WaveManager.NotifyEnemyKilled(this);
            Destroy(gameObject);
        }
    }
}
