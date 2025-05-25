using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Moves the object downward in discrete steps with delay and random scatter.
/// Dynamically scales a decal shadow based on distance to the ground.
/// </summary>
public class EnemyStepDownMovement : MonoBehaviour {
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Step Settings")]
    [SerializeField] private float baseStepDistance = 2f;
    [SerializeField] private float distanceScatter = 0.25f;
    [SerializeField] private float baseStepInterval = 1f;
    [SerializeField] private float intervalScatter = 0.25f;

    [Header("Start Delay")]
    [SerializeField] private float startDelay = 0.2f;

    [Header("Shadow Settings")]
    [SerializeField] private DecalProjector shadowProjector;
    [SerializeField] private float minSize = 0.5f;
    [SerializeField] private float maxSize = 2.5f;
    [SerializeField] private float shadowRayLength = 150f;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float projectorDepth = 50f;

    [Header("Attack Behavior")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackStepInterval = 0.2f;

    private float stepTimer;
    private float effectiveStepInterval;
    private float effectiveStepDistance;
    private bool hasEnteredAttackMode = false;

    private void Start() {
        effectiveStepInterval = baseStepInterval + Random.Range(-intervalScatter, intervalScatter);
        effectiveStepDistance = baseStepDistance + Random.Range(-distanceScatter, distanceScatter);
        stepTimer = -Random.Range(0f, startDelay);
    }

    private void Update() {
        if (enemyHealth.gameManager.gameState != RetroHunter2.GameState.play) return;

        stepTimer += Time.deltaTime;

        if (stepTimer >= effectiveStepInterval) {
            transform.position += Vector3.down * effectiveStepDistance;
            stepTimer = 0f;
        }

        UpdateShadowScaleByHeight();
    }

    private void UpdateShadowScaleByHeight() {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, shadowRayLength, environmentLayer)) {
            float height = Vector3.Distance(transform.position, hit.point);

            if (height < attackRange) {
                effectiveStepInterval = attackStepInterval;

                if (!hasEnteredAttackMode) {
                    hasEnteredAttackMode = true;

                    // Trigger flash effect
                    if (enemyHealth != null) {
                        enemyHealth.FlashColor();
                    }
                }
            }

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
    }
}