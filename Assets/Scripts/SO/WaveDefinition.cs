using System.Collections.Generic;
using UnityEngine;
using RetroHunter2;

[CreateAssetMenu(menuName = "Spawn/Wave Definition")]
public class WaveDefinition : ScriptableObject {
    public int maxEnemiesThisWave = 10;
    public float delayBeforeNextSpawn = 1f;
    public List<EnemySpawnEntry> spawnEntries = new();
}