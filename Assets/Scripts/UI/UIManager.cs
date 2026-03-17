using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TrainSurvival.Core;
using TrainSurvival.Player;
using TrainSurvival.Weapons;
using TrainSurvival.Waves;

namespace TrainSurvival.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private TMP_Text currencyText;
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text waveStateText;
        [SerializeField] private TMP_Text interactionText;

        [Header("Panels")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private TMP_Text upgradeTitle;
        [SerializeField] private RectTransform upgradeListParent;
        [SerializeField] private Button upgradeEntryPrefab;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Runtime refs")]
        [SerializeField] private WeaponController weapon;

        private readonly List<Button> activeButtons = new();

        private void Awake()
        {
            Instance = this;
            ShowGameOver(false);
            if (upgradePanel != null) upgradePanel.SetActive(false);
        }

        public void Bind(GameManager gameManager)
        {
            PlayerHealth ph = gameManager.PlayerHealth;
            WaveManager wm = gameManager.WaveManager;

            ph.OnHealthChanged += (current, max) => healthText.text = $"HP  {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            wm.OnWaveChanged += wave => waveText.text = $"WAVE {wave}";
            wm.OnWaveStateChanged += state => waveStateText.text = state == WaveState.Preparation ? "Preparation" : "In Combat";
            gameManager.CurrencySystem.OnCurrencyChanged += value => currencyText.text = $"$ {value}";

            waveText.text = "WAVE 0";
            waveStateText.text = "Preparation";
            currencyText.text = $"$ {gameManager.CurrencySystem.Current}";
            UpdateAmmo(weapon.AmmoInMag, weapon.Data.magazineSize);
        }

        public void UpdateAmmo(int current, int max)
        {
            if (ammoText != null)
                ammoText.text = $"AMMO {current}/{max}";
        }

        public void SetWaveState(string value)
        {
            if (waveStateText != null)
                waveStateText.text = value;
        }

        public void SetInteractionPrompt(string prompt)
        {
            if (interactionText != null)
                interactionText.text = prompt;
        }

        public void OpenUpgradePanel(string station, List<string> lines, Action<int> onSelect)
        {
            if (upgradePanel == null)
                return;

            upgradePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            upgradeTitle.text = station;

            foreach (var btn in activeButtons)
                Destroy(btn.gameObject);
            activeButtons.Clear();

            for (int i = 0; i < lines.Count; i++)
            {
                int index = i;
                Button btn = Instantiate(upgradeEntryPrefab, upgradeListParent);
                btn.GetComponentInChildren<TMP_Text>().text = lines[i];
                btn.onClick.AddListener(() => onSelect(index));
                activeButtons.Add(btn);
            }
        }

        public void CloseUpgradePanel()
        {
            if (upgradePanel == null)
                return;

            upgradePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ShowGameOver(bool value)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(value);
        }
    }
}
