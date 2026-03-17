using UnityEngine;
using UnityEngine.SceneManagement;
using TrainSurvival.Player;
using TrainSurvival.Waves;
using TrainSurvival.UI;

namespace TrainSurvival.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene References")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private CurrencySystem currencySystem;
        [SerializeField] private UIManager uiManager;

        public PlayerHealth PlayerHealth => playerHealth;
        public WaveManager WaveManager => waveManager;
        public CurrencySystem CurrencySystem => currencySystem;

        private bool gameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            playerHealth.OnDied += HandleGameOver;
            uiManager.Bind(this);
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.OnDied -= HandleGameOver;
        }

        private void HandleGameOver()
        {
            if (gameOver)
                return;

            gameOver = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            uiManager.ShowGameOver(true);
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public bool IsGameOver() => gameOver;
    }
}
