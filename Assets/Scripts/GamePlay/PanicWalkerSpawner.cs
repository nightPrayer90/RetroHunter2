using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RetroHunter2;

/// <summary>
/// Spawns PanicWalker units or specific Snorbles inside defined BoxColliders.
/// </summary>
public class PanicWalkerSpawner : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;

    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> panicWalkerPrefabs = new();

    [Header("Snorble Prefabs")]
    [SerializeField] private GameObject kelvinPrefab;
    [SerializeField] private GameObject albertPrefab;

    [Header("Spawn Areas (BoxColliders)")]
    [SerializeField] private List<BoxCollider> spawnAreas = new();

    [Header("Walker Names")]
    [SerializeField]
    private string[] firstNames = {
        "Duke", "Gordon", "Samus", "Doom", "Kyle", "Jill", "Leon", "Ash",
        "Shep", "Ellie", "Chell", "Simon", "Mega", "Snake", "Turok",
        "Albert", "Kelvin"
    };

    [SerializeField]
    private string[] lastNames = {
        "Slab", "Grub", "Flux", "Ratt", "Nash", "Gore", "Spit",
        "Claw", "Drip", "Snap", "Slug", "Murk", "Hack", "Gnaw"
    };

    [Header("Visual Portals")]
    [SerializeField] private List<MeshRenderer> portalRenderers = new();
    [SerializeField] private Material portalMaterialOn;
    [SerializeField] private Material portalMaterialOff;

    private Coroutine spawnRoutine;

    /// <summary>
    /// Spawns a number of random PanicWalkers across all spawn areas. Only at beginning
    /// </summary>
    public void SpawnWalkers(int count) {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnInWaves(count));
    }

    private IEnumerator SpawnInWaves(int targetCount) {
        int waveAdd = 0;

        while (waveAdd < targetCount) {
            foreach (var area in spawnAreas) {
                if (waveAdd >= targetCount) break;

                Vector3 spawnPos = GetRandomPointInBox(area);
                if (spawnPos != Vector3.zero && panicWalkerPrefabs.Count > 0) {
                    GameObject prefab = panicWalkerPrefabs[Random.Range(0, panicWalkerPrefabs.Count)];
                    GameObject walker = Instantiate(prefab, spawnPos, Quaternion.identity);

                    PanicWalkerHealth health = walker.GetComponent<PanicWalkerHealth>();
                    health.SetGameManager(gameManager);
                    health.SetWalkerName(GenerateWalkerName());

                    waveAdd++;
                }
            }

            yield return null;
        }

        spawnRoutine = null;
    }

    /// <summary>
    /// Spawns a specific Snorble in a given spawn area.
    /// </summary>
    public void SpawnSnorble(SnorbleType type, int areaIndex) {
        if (areaIndex < 0 || areaIndex >= spawnAreas.Count) {
            Debug.LogWarning("Invalid spawn area index for Snorble.");
            return;
        }

        BoxCollider area = spawnAreas[areaIndex];
        Vector3 spawnPos = GetRandomPointInBox(area);
        GameObject prefab = GetSnorblePrefab(type);

        if (prefab == null) {
            Debug.LogWarning("Snorble prefab is missing.");
            return;
        }

        GameObject snorble = Instantiate(prefab, spawnPos, Quaternion.identity);

        PanicWalkerHealth health = snorble.GetComponent<PanicWalkerHealth>();
        if (health != null) {
            health.SetGameManager(gameManager);
            health.SetWalkerName(type.ToString());
        }

        // Update portal materials
        for (int i = 0; i < portalRenderers.Count; i++) {
            if (portalRenderers[i] == null) continue;

            Material[] materials = portalRenderers[i].materials;
            if (materials.Length > 0) {
                materials[0] = (i == areaIndex) ? portalMaterialOn : portalMaterialOff;
                portalRenderers[i].materials = materials;
            }
        }
    }

    /// <summary>
    /// Returns the prefab associated with the given Snorble type.
    /// </summary>
    private GameObject GetSnorblePrefab(SnorbleType type) {
        return type switch {
            SnorbleType.Kelvin => kelvinPrefab,
            SnorbleType.Albert => albertPrefab,
            _ => null,
        };
    }

    /// <summary>
    /// Picks a random position inside a BoxCollider in world space.
    /// </summary>
    private Vector3 GetRandomPointInBox(BoxCollider area) {
        if (area == null) return Vector3.zero;

        Vector3 localPoint = new Vector3(
            Random.Range(-0.5f, 0.5f) * area.size.x,
            Random.Range(-0.5f, 0.5f) * area.size.y,
            Random.Range(-0.5f, 0.5f) * area.size.z
        );

        return area.transform.TransformPoint(area.center + localPoint);
    }

    /// <summary>
    /// Combines a random first and last name for a walker.
    /// </summary>
    private string GenerateWalkerName() {
        if (firstNames.Length == 0 || lastNames.Length == 0) return "Unnamed";
        string first = firstNames[Random.Range(0, firstNames.Length)];
        string last = lastNames[Random.Range(0, lastNames.Length)];
        return $"{first} {last}";
    }
}