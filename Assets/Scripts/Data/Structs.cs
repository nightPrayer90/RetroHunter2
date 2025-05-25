using UnityEngine;

namespace RetroHunter2 {
    [System.Serializable]
    public struct EnemySpawnEntry {
        public EnemyType enemyType;
        public float spawnChance;
        public bool assignTargetOnSpawn;
    }
}