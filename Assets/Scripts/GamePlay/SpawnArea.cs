using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines an area where specific enemy types can spawn.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class SpawnArea : MonoBehaviour {
    [SerializeField] private List<GameObject> allowedEnemyPrefabs = new();
    private BoxCollider box;

    [SerializeField] private Transform targetTransform;
    public Transform TargetTransform => targetTransform;

    private void Awake() {
        box = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Returns a random point within the box collider area.
    /// </summary>
    public Vector3 GetRandomSpawnPosition() {
        Vector3 center = transform.TransformPoint(box.center);
        Vector3 size = Vector3.Scale(box.size, transform.lossyScale);

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Returns a random enemy prefab assigned to this area.
    /// </summary>
    public GameObject GetRandomAllowedEnemy() {
        if (allowedEnemyPrefabs.Count == 0) return null;
        return allowedEnemyPrefabs[Random.Range(0, allowedEnemyPrefabs.Count)];
    }

}