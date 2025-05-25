using UnityEngine;
using System.Collections.Generic;
using RetroHunter2;

[System.Serializable]
public class CookieEntry {
    public GameObject prefab;
    [Range(0f, 1f)]
    public float weight = 1f;
}

public class AlbertCookieThrower : MonoBehaviour {
    [Header("Cookie Settings")]
    [SerializeField] private List<CookieEntry> cookiePool;
    [SerializeField] private Transform throwOrigin;

    [Header("Timing")]
    [SerializeField] private float baseThrowCooldown = 10f;
    [SerializeField] private float intervalScatter = 3f;
    [SerializeField] private float startScatter = 3f;

    [Header("References")]
    [SerializeField] private PanicWalkerHealth walkerHealth;

    private float throwTimer;
    private float currentCooldown;

    private void Start() {
        throwTimer = -Random.Range(0f, startScatter);
        currentCooldown = GetNextCooldown();
    }

    private void Update() {
        if (walkerHealth == null || walkerHealth.gameManager == null) return;
        if (walkerHealth.gameManager.gameState != GameState.play) return;

        throwTimer += Time.deltaTime;

        if (throwTimer >= currentCooldown) {
            throwTimer = 0f;
            currentCooldown = GetNextCooldown();

            ThrowCookie();
        }
    }

    private float GetNextCooldown() {
        return baseThrowCooldown + Random.Range(-intervalScatter, intervalScatter);
    }

    private void ThrowCookie() {
        if (cookiePool.Count == 0 || throwOrigin == null) return;

        walkerHealth.FlashColor(Color.yellow);
        string cheer = walkerHealth.gameManager.messagePool.GetRandomThrowMessage();
        walkerHealth.gameManager.ShowSimpleFloatingText(cheer, transform.position + Vector3.up * 20f, Color.wheat);

        GameObject prefab = GetWeightedCookie();
        GameObject cookie = Instantiate(prefab, throwOrigin.position, Quaternion.identity);
        cookie.GetComponent<CookieProjectile>().SetGameManager(walkerHealth.gameManager);

        AudioManager.Instance.PlaySFX(SoundIndexKey.albertCookieThrow);
    }

    private GameObject GetWeightedCookie() {
        float totalWeight = 0f;
        foreach (var entry in cookiePool) {
            totalWeight += entry.weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in cookiePool) {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry.prefab;
        }

        return cookiePool[0].prefab;
    }


}