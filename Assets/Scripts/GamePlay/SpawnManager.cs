using UnityEngine;
using System.Collections.Generic;
using RetroHunter2;

/// <summary>
/// Controls enemy spawning based on wave definitions with area-specific weights.
/// </summary>
public class SpawnManager : MonoBehaviour {
    [SerializeField] private List<WaveDefinition> waveDefinitions;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HuntRankSystem huntRankSystem;

    private int currentWaveIndex = 0;
    private WaveDefinition currentWave;

    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int maxEnemies = 16;

    [Header("SpawnAreas")]
    [SerializeField] private List<SpawnArea> spawnAreas_Enemy01 = new();
    [SerializeField] private List<SpawnArea> spawnAreas_Enemy02 = new();
    [SerializeField] private List<SpawnArea> spawnAreas_Enemy03 = new();

    private float spawnTimer;
    private int currentEnemyCount;



    private void Update() {
        if (currentWave == null || currentWave.spawnEntries.Count == 0) return;

        spawnTimer += Time.deltaTime;

        float mult = Mathf.Max(0.1f, gameManager.upgradeManager.enemySpawnRateMult);
        float effectiveInterval = spawnInterval / mult;

        if (spawnTimer >= effectiveInterval && currentEnemyCount < maxEnemies) {
            TrySpawnEnemyFromWave();
            spawnTimer = 0f;
        }
    }

    /// <summary>
    /// Loads the next wave and initializes spawning parameters.
    /// </summary>
    public void StartNextWave() {
        if (currentWaveIndex >= waveDefinitions.Count) return;

        currentWave = waveDefinitions[currentWaveIndex];
        spawnTimer = 0f;
        currentEnemyCount = 0;
        maxEnemies = currentWave.maxEnemiesThisWave;
        spawnInterval = currentWave.delayBeforeNextSpawn;

        currentWaveIndex++;
    }

    /// <summary>
    /// Selects a SpawnArea based on weighted spawn chances and spawns an enemy there.
    /// </summary>
    private void TrySpawnEnemyFromWave() {
        float totalChance = 0f;
        foreach (var entry in currentWave.spawnEntries) {
            totalChance += entry.spawnChance;
        }

        float roll = Random.value * totalChance;
        float cumulative = 0f;
        foreach (var entry in currentWave.spawnEntries) {
            cumulative += entry.spawnChance;
            if (roll <= cumulative) {
                SpawnEnemy(entry.enemyType, entry.assignTargetOnSpawn);
                return;
            }
        }
    }

    /// <summary>
    /// Instantiates an enemy in the given area and optionally assigns a target transform.
    /// </summary>
    private void SpawnEnemy(EnemyType type, bool assignTarget) {
        SpawnArea area = GetRandomSpawnAreaForType(type);
        if (area == null) return;

        GameObject prefab = area.GetRandomAllowedEnemy();
        if (prefab == null) return;

        Vector3 spawnPos = area.GetRandomSpawnPosition();
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        currentEnemyCount++;

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null) {
            health.SetGameManager(gameManager);
            health.SetSpawnManager(this);
        }

        if (assignTarget && area.TargetTransform != null) {
            if (enemy.TryGetComponent(out EnemyFlyToCenterAndExplode bomber)) {
                bomber.SetTarget(area.TargetTransform);
            }
        }
    }

    /// <summary>
    /// Called when an enemy dies.
    /// </summary>
    public void NotifyEnemyKilled() {
        currentEnemyCount--;
        if (huntRankSystem != null) {
            huntRankSystem.AddXP();
        }
    }

    /// <summary>
    /// Called when an enemy is removed without dying (e.g., cleanup).
    /// </summary>
    public void NotifyEnemyRemoved() {
        currentEnemyCount--;
    }

    private SpawnArea GetRandomSpawnAreaForType(EnemyType type) {
        List<SpawnArea> pool = null;

        switch (type) {
            case EnemyType.Enemy_01:
                pool = spawnAreas_Enemy01;
                break;
            case EnemyType.Enemy_02:
                pool = spawnAreas_Enemy02;
                break;
            case EnemyType.Enemy_03:
                pool = spawnAreas_Enemy03;
                break;
        }

        if (pool == null || pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }
}