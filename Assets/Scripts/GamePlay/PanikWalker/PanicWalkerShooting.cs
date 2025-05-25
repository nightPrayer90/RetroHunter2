using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RetroHunter2;

public class PanicWalkerShooting : MonoBehaviour {
    [Header("Targeting")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private float shootCooldown = 5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float shootDelay = 0.6f;
    [SerializeField] private PanicWalkerHealth walkerHealth;
    private float shootTimer;
    private Transform currentTarget;
    private PanicWalkerTargetMarker activeMarker;

    private void Start() {
        shootTimer = -Random.Range(0, 3.2f);
        shootCooldown += Random.Range(0.5f, 2.5f);

        walkerHealth = GetComponent<PanicWalkerHealth>();

    }

    private void Update() {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown && currentTarget == null) {
            shootTimer = 0f;
            TryMarkNewTarget();
        }
    }

    private void TryMarkNewTarget() {
        //TODO: GameStateCheck

        EnemyHealth[] allEnemies = Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        List<EnemyHealth> unmarked = new();

        foreach (var enemy in allEnemies) {
            if (!enemy.isMarkedByKelvin)
                unmarked.Add(enemy);
        }

        if (unmarked.Count == 0) return;

        EnemyHealth selected = unmarked[Random.Range(0, unmarked.Count)];
        selected.SetKelvinMarked(true);

        GameObject markerGO = Instantiate(targetPrefab, selected.transform.position + Vector3.up * 1.5f, Quaternion.identity);
        PanicWalkerTargetMarker marker = markerGO.GetComponent<PanicWalkerTargetMarker>();
        marker.Init(selected);

        activeMarker = marker;
        currentTarget = selected.transform;

        // Shoot shortly after marking
        StartCoroutine(ShootAfterDelay(selected, marker.transform));
    }

    private IEnumerator ShootAfterDelay(EnemyHealth targetHealth, Transform markerTransform) {
        if (walkerHealth != null) {
            walkerHealth.FlashColor(Color.green);

            string throwLine = walkerHealth.gameManager.messagePool.GetRandomThrowMessage();
            walkerHealth.gameManager.ShowSimpleFloatingText(throwLine, transform.position + Vector3.up * 20f, Color.wheat);
        }

        yield return new WaitForSeconds(shootDelay);

        if (projectilePrefab == null || projectileSpawnPoint == null) yield break;
        if (targetHealth == null || markerTransform == null) yield break;

        GameObject projGO = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        KelvinProjectile proj = projGO.GetComponent<KelvinProjectile>();
        proj.Init(targetHealth, markerTransform);

        AudioManager.Instance.PlaySFX(SoundIndexKey.KelvinShooting);
    }

    public Transform GetCurrentTarget() {
        return activeMarker != null ? activeMarker.transform : null;
    }

    public bool HasActiveTarget() {
        return activeMarker != null;
    }
}