using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrainSurvival.Core;
using TrainSurvival.Enemies;
using TrainSurvival.UI;

namespace TrainSurvival.Waves
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private WaveConfig waveConfig;
        [SerializeField] private SpiderEnemy enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 0.25f;

        private readonly List<SpiderEnemy> aliveEnemies = new();

        public int CurrentWave { get; private set; }
        public WaveState State { get; private set; } = WaveState.Preparation;

        public event Action<int> OnWaveChanged;
        public event Action<WaveState> OnWaveStateChanged;

        public void StartNextWave()
        {
            if (State == WaveState.InWave || GameManager.Instance.IsGameOver())
                return;

            CurrentWave++;
            State = WaveState.InWave;
            OnWaveChanged?.Invoke(CurrentWave);
            OnWaveStateChanged?.Invoke(State);
            UIManager.Instance?.SetWaveState($"Wave {CurrentWave}: Hostiles incoming");

            StartCoroutine(SpawnWaveRoutine());
        }

        private IEnumerator SpawnWaveRoutine()
        {
            int enemyCount = waveConfig.baseEnemyCount + (CurrentWave - 1) * waveConfig.enemiesAddedPerWave;
            float hpMultiplier = 1f + (CurrentWave - 1) * waveConfig.hpMultiplierPerWave;
            float speedMultiplier = 1f + (CurrentWave - 1) * waveConfig.speedMultiplierPerWave;
            float damageMultiplier = 1f + (CurrentWave - 1) * waveConfig.damageMultiplierPerWave;

            for (int i = 0; i < enemyCount; i++)
            {
                Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                SpiderEnemy enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
                enemy.ApplyWaveModifiers(hpMultiplier, speedMultiplier, damageMultiplier);
                aliveEnemies.Add(enemy);
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        public void NotifyEnemyKilled(SpiderEnemy enemy)
        {
            aliveEnemies.Remove(enemy);

            if (State == WaveState.InWave && aliveEnemies.Count == 0)
                CompleteWave();
        }

        private void CompleteWave()
        {
            State = WaveState.WaveCompleted;
            OnWaveStateChanged?.Invoke(State);
            int reward = waveConfig.rewardBase + (CurrentWave - 1) * waveConfig.rewardGrowth;
            GameManager.Instance.CurrencySystem.Add(reward);
            UIManager.Instance?.SetWaveState($"Wave {CurrentWave} complete. +{reward}$ | Press E on console for next wave");
            State = WaveState.Preparation;
            OnWaveStateChanged?.Invoke(State);
        }
    }
}
