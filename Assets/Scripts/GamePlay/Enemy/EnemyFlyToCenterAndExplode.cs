using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Moves the enemy toward a defined target and explodes upon reaching a close distance.
/// Increases movement speed near target and dynamically scales a shadow projector.
/// </summary>
public class EnemyFlyToCenterAndExplode : MonoBehaviour {
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float attackSpeed = 12f;
    [SerializeField] private float explodeDistance = 2f;

    [Header("Attack Trigger")]
    [SerializeField] private float attackRange = 7f;

    [Header("Shadow Settings")]
    [SerializeField] private DecalProjector shadowProjector;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2.5f;
    [SerializeField] private float shadowRayLength = 150f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float projectorDepth = 50f;

    private Transform target;
    private bool hasEnteredAttackMode = false;


    private void Update() {
        if (enemyHealth == null || enemyHealth.gameManager.gameState != RetroHunter2.GameState.play) return;
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= explodeDistance) {
            Explode();
            return;
        }

        if (distance <= attackRange && !hasEnteredAttackMode) {
            hasEnteredAttackMode = true;
            enemyHealth.FlashColor(); // flash once when entering attack mode
        }

        float currentSpeed = hasEnteredAttackMode ? attackSpeed : baseSpeed;
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * currentSpeed * Time.deltaTime;

        UpdateShadowProjector();
    }

    private void Explode() {
        enemyHealth.Escape();
    }

    private void UpdateShadowProjector() {
        if (shadowProjector == null) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, shadowRayLength, environmentLayer)) {
            float height = Vector3.Distance(transform.position, hit.point);

            if (height > projectorDepth) {
                if (shadowProjector.gameObject.activeSelf)
                    shadowProjector.gameObject.SetActive(false);
                return;
            }

            if (!shadowProjector.gameObject.activeSelf)
                shadowProjector.gameObject.SetActive(true);

            float t = Mathf.InverseLerp(0f, projectorDepth, height);
            float size = Mathf.Lerp(maxSize, minSize, t);
            shadowProjector.size = new Vector3(size, size, projectorDepth);
        }
        else {
            if (shadowProjector.gameObject.activeSelf)
                shadowProjector.gameObject.SetActive(false);
        }
    }

    public void SetTarget(Transform t) {
        target = t;
    }
}