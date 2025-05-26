using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using RetroHunter2;

/// <summary>
/// Handles player shooting logic via mouse and manages ammo with reloading using the new Input System.
/// </summary>
public class PlayerController : MonoBehaviour {
    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CrosshairController crosshairController;

    [Header("Shooting Settings")]
    [SerializeField] private float shootDelay = 0.2f;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private int maxAmmo = 5;

    private int currentAmmo;
    private bool isReloading = false;
    private bool canShoot = true;

    private Camera mainCam;

    private InputAction shootAction;
    private InputAction reloadAction;

    [Header("Hit Feedback")]
    [SerializeField] private float hitForce = 50f;

    private void Awake() {
        // Create actions
        shootAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        reloadAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");

        shootAction.Enable();
        reloadAction.Enable();
    }

    private void Start() {
        currentAmmo = maxAmmo;
        gameManager.InitAmmoBar(maxAmmo);
        mainCam = Camera.main;
    }

    private void Update() {
        if (isReloading) return;
        if (gameManager.gameState != GameState.play) return;

        if (shootAction.WasPressedThisFrame() && canShoot) {
            Shoot();
            gameManager.uiManager.FlashBtn(0);
        }
        else if (reloadAction.WasPressedThisFrame()) {
            if (currentAmmo < maxAmmo) {
                StartCoroutine(Reload());
                gameManager.uiManager.FlashBtn(1);
            }
        }
    }

    private void Shoot() {
        if (crosshairController.IsOverUI) return;

        if (currentAmmo <= 0) {
            AudioManager.Instance.PlaySFX(SoundIndexKey.playerAmmoEmpty);
            return;
        }

        // Check: skip ammo usage via upgrade bonus
        bool skipAmmo = Random.value < gameManager.upgradeManager.noAmmoUseChance;

        if (!skipAmmo) {
            currentAmmo--;
            AudioManager.Instance.PlaySFXWithRandomPitch(SoundIndexKey.playerShootWithNoAmmo, 0.5f, 0.2f);
        }
        else {
            // Flash crosshair in yellow/orange to indicate ammo was not consumed
            crosshairController.FlashCrosshair(new Color(1f, 0.85f, 0.3f)); // soft gold/yellow
            AudioManager.Instance.PlaySFXWithRandomPitch(SoundIndexKey.playerShoot, 0.5f, 0.2f);
        }

        gameManager.UpdateAmmoBarAfterShooting(currentAmmo);

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        float maxDistance = 1000f;

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) {
            //Debug.Log($"Raycast hit: {hit.collider.name}, Tag: {hit.collider.tag}");

            if (hit.collider.CompareTag("Enemy")) {
                //Debug.Log("Hit enemy: " + hit.collider.name);

                EnemyHealth health = hit.collider.GetComponent<EnemyHealth>();
                if (health != null) {
                    health.TakeDamage(1);
                    gameManager.ComboHit();

                    // Flash green for hit confirmation
                    crosshairController.AnimateHitFeedback();
                    crosshairController.FlashCrosshair(Color.green);
                }

                float chance = gameManager.upgradeManager.ricochetChance;
                if (Random.value < chance) {
                    StartCoroutine(DelayedChainHit(hit.point, hit.collider));
                }

                // Add force if Rigidbody is present
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null && !rb.isKinematic) {
                    rb.AddForce(ray.direction * hitForce, ForceMode.Impulse);
                }
            }
            else if (hit.collider.CompareTag("Cookie")) {
                CookieProjectile cookie = hit.collider.GetComponent<CookieProjectile>();
                if (cookie != null) {
                    cookie.OnHitByPlayer();

                    // Feedback
                    crosshairController.AnimateHitFeedback();
                    crosshairController.FlashCrosshair(new Color(1f, 0.7f, 0.3f));
                   
                    gameManager.ComboHit();
                }
            }
        }
        else {
            crosshairController.FlashCrosshair(new Color(0.8f, 0.8f, 0.8f)); // Light gray
        }

        StartCoroutine(ShootCooldown());
    }

    private IEnumerator DelayedChainHit(Vector3 origin, Collider originalHit) {
        yield return new WaitForSeconds(0.15f); 

        float chainHitRadius = 40;
        Collider[] hits = Physics.OverlapSphere(origin, chainHitRadius, LayerMask.GetMask("Enemy"));

        foreach (var col in hits) {
            if (col == originalHit) continue;

            EnemyHealth extraHealth = col.GetComponent<EnemyHealth>();
            if (extraHealth != null) {
                extraHealth.TakeDamage(1);
                break; 
            }
        }
    }

    private IEnumerator ShootCooldown() {
        canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    private IEnumerator Reload() {
        float effectiveReload = reloadTime * gameManager.upgradeManager.reloadSpeedMult;
        AudioManager.Instance.PlaySFX(SoundIndexKey.playerReload);

        gameManager.uiManager.PlayReloadAnimation(effectiveReload, currentAmmo);

        isReloading = true;
        yield return new WaitForSeconds(effectiveReload);

        currentAmmo = maxAmmo;
        gameManager.UpdateAmmoBar(currentAmmo);
        isReloading = false;
    }

    private void OnDisable() {
        shootAction.Disable();
        reloadAction.Disable();
    }

    /// <summary>
    /// Increases the max ammo and refreshes the ammo bar UI.
    /// </summary>
    public void IncreaseMaxAmmo(int amount) {
        maxAmmo += amount;
        currentAmmo = maxAmmo;

        gameManager.InitAmmoBar(maxAmmo);
        gameManager.UpdateAmmoBar(currentAmmo);
    }

    public void InstantReload() {
        gameManager.uiManager.PlayInstantReloadAnimation(currentAmmo);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public float GetReloadTime() {
        return reloadTime * gameManager.upgradeManager.reloadSpeedMult;
    }
}