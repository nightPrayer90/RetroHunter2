using UnityEngine;
using RetroHunter2;
using System.Collections;

public class CookieProjectile : MonoBehaviour {
    [Header("Setup")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float lifetime = 8f;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float angleSpread = 30f;
    [SerializeField] private CookieType cookieType;
    [SerializeField] private LayerMask environmentLayer;
    private GameManager gameManager;
    [SerializeField] private LayerMask explosionAffectLayers;
    [SerializeField] private GameObject crumbleVFXPrefab;
    [SerializeField] private GameObject[] explosionVFXPrefabs; 
    private bool hasExploded = false;

    private void OnEnable() {
        InitMovement();
        StartCoroutine(LifetimeDespawn());
    }

    private void InitMovement() {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearVelocity = CalculateLaunchVelocity();
    }

    private Vector3 CalculateLaunchVelocity() {
        float angle = Random.Range(-angleSpread, angleSpread);
        Vector3 dir = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
        return dir.normalized * throwForce;
    }

    public void OnHitByPlayer() {
        switch (cookieType) {
            case CookieType.Red:
                Explode();
                break;
            case CookieType.Blue:
                AddWaveTime();
                break;
            case CookieType.Yellow:
                RefillAmmo();
                break;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (((1 << other.gameObject.layer) & environmentLayer) != 0) {
            AudioManager.Instance.PlaySFX(SoundIndexKey.cookieCrumble);
            PlayCrumbleVFX();
            Destroy(gameObject); 
        }
    }

    // --- Behavior per Type ---
    private void Explode() {
        if (hasExploded) return;
        hasExploded = true;

        Collider[] hits = Physics.OverlapSphere(transform.position, 50f, explosionAffectLayers);

        foreach (var col in hits) {
            EnemyHealth enemy = col.GetComponent<EnemyHealth>();
            if (enemy != null) {
                enemy.TakeDamage(1);
                continue;
            }

            if (col.CompareTag("Cookie")) {
                CookieProjectile cookie = col.GetComponent<CookieProjectile>();
                if (cookie != null && cookie != this) {
                    cookie.TriggerDelayedHit(0.2f);
                }
            }
        }

        if (explosionVFXPrefabs != null && explosionVFXPrefabs.Length > 0) {
            int index = Random.Range(0, explosionVFXPrefabs.Length);
            Instantiate(explosionVFXPrefabs[index], transform.position, Quaternion.identity);
            gameManager.cameraController.Shake(0.25f, 2);
        }

        AudioManager.Instance.PlaySFX(SoundIndexKey.cookieCrumble); // Optional
    }

    private void AddWaveTime() {
        if (hasExploded) return;
        hasExploded = true;

        if (gameManager != null) {
            gameManager.waveTimeRemaining += 2f;
            gameManager.uiManager.AnimateWaveTimeText();

            gameManager.ShowSimpleFloatingText(
           "Wave +2s",
           transform.position + Vector3.up * 20f,
           Color.cyan);

        }
        PlayCrumbleVFX();
        AudioManager.Instance.PlaySFX(SoundIndexKey.blueCookieCrumble); // Optional
    }

    private void RefillAmmo() {
        if (hasExploded) return;
        hasExploded = true;

        gameManager.playerController.InstantReload();
        PlayCrumbleVFX();

        gameManager.ShowSimpleFloatingText(
            "Ammo Refreshed",
            transform.position + Vector3.up * 20f,
            Color.yellow);

        AudioManager.Instance.PlaySFX(SoundIndexKey.yellowCookieCrumble); // Optional
    }

    public void SetGameManager(GameManager gm) {
        gameManager = gm;
    }

    private void PlayCrumbleVFX() {
        if (crumbleVFXPrefab != null) {
            Instantiate(crumbleVFXPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator LifetimeDespawn() {
        yield return new WaitForSeconds(lifetime);

        AudioManager.Instance.PlaySFX(SoundIndexKey.cookieCrumble);

        PlayCrumbleVFX();
        Destroy(gameObject);
    }

    public void TriggerDelayedHit(float delay = 0.1f) {
        StartCoroutine(DelayedHitRoutine(delay));
    }

    private IEnumerator DelayedHitRoutine(float delay) {
        yield return new WaitForSeconds(delay);
        OnHitByPlayer();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (cookieType != CookieType.Red || hasExploded) return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, 50f);  

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 50f);
    }
#endif
}