using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// Moves the enemy left and right around its spawn point. Reverses direction after reaching max distance
/// or colliding with a wall. Adds randomized scatter to speed and movement distance, and applies a cooldown
/// to avoid multiple direction changes in one frame.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpaceInvaderMovement : MonoBehaviour {
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 1f;
    [SerializeField] private float speedScatter = 0.2f;

    [SerializeField] private float baseMaxDistance = 15f;
    [SerializeField] private float distanceScatter = 3f;

    [SerializeField] private float stepDownDistance = 0.25f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float directionChangeCooldown = 0.1f;

    private float moveSpeed;
    private float maxDistance;
    private int direction;
    private Vector3 spawnPosition;
    private float lastDirectionChangeTime = -999f;

    [Header("Shadow Settings")]
    [SerializeField] private DecalProjector shadowProjector;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float shadowRayInterval = 0.1f;
    [SerializeField] private float shadowRayDistance = 50f;
    [SerializeField] private float projectorDepth = 50f;

    private float shadowRayTimer;

    private Transform targetWalker;
    private bool isDiving = false;
    private float diveTimer = 0f;
    private float diveDuration = 0.6f;
    private float divePause = 0.1f;   
    private float nextDiveTime = 0f;


    /// <summary>
    /// Initializes spawn position, movement direction and randomized movement parameters.
    /// </summary>
    private void Start() {
        spawnPosition = transform.position;

        direction = Random.value < 0.5f ? -1 : 1;
        moveSpeed = Mathf.Max(0.1f, baseMoveSpeed + Random.Range(-speedScatter, speedScatter));
        maxDistance = Mathf.Max(0.5f, baseMaxDistance + Random.Range(-distanceScatter, distanceScatter));

        GameObject[] walkers = GameObject.FindGameObjectsWithTag("Walker");
        float closestDistance = float.MaxValue;

        foreach (var walker in walkers) {
            float dist = Vector3.Distance(transform.position, walker.transform.position);
            if (dist < closestDistance) {
                closestDistance = dist;
                targetWalker = walker.transform;
            }
        }
    }

    /// <summary>
    /// Handles enemy movement logic, including horizontal patrol and dive behavior toward the nearest PanicWalker.
    /// </summary>
    private void Update() {
        // Only move if game is in play state
        if (enemyHealth == null || enemyHealth.gameManager.gameState != RetroHunter2.GameState.play) return;

        // If the target walker was destroyed, exit dive mode and resume normal behavior
        if (targetWalker == null || targetWalker.gameObject == null) {
            isDiving = false;
            targetWalker = null;
        }

        // If not diving and the walker is far enough below, initiate dive mode
        if (!isDiving && targetWalker != null) {
            float yDiff = transform.position.y - targetWalker.position.y;
            if (yDiff <= 40f) {
                isDiving = true;
                nextDiveTime = Time.time;
                enemyHealth.FlashColor();
            }
        }

        // Dive mode active
        if (isDiving && targetWalker != null) {
            // Move toward the walker in pulses
            if (Time.time >= nextDiveTime) {
                Vector3 dir = (targetWalker.position - transform.position).normalized;
                transform.position += dir * moveSpeed * Time.deltaTime;

                diveTimer += Time.deltaTime;
                if (diveTimer >= diveDuration) {
                    diveTimer = 0f;
                    nextDiveTime = Time.time + divePause;
                }
            }

            // If close enough to the walker, deal damage and self-destruct
            float distanceToWalker = Vector3.Distance(transform.position, targetWalker.position);
            if (distanceToWalker <= 5f) {
                isDiving = false;
                diveTimer = 0f;
                nextDiveTime = 0f;

                // Deal 1 damage to the walker
                PanicWalkerHealth walkerHealth = targetWalker.GetComponent<PanicWalkerHealth>();
                if (walkerHealth != null) {
                    walkerHealth.TakeDamage(1);
                }

                // Destroy self
                if (enemyHealth != null) {
                    enemyHealth.Die();
                }

                return; // Skip further movement
            }

            // Keep shadow projector updated during dive
            shadowRayTimer += Time.deltaTime;
            if (shadowRayTimer >= shadowRayInterval) {
                shadowRayTimer = 0f;
                UpdateShadowProjector();
            }

            return; // Skip normal movement
        }

        // Normal horizontal movement
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // Reverse direction if max distance is reached
        float horizontalDistance = Mathf.Abs(transform.position.x - spawnPosition.x);
        if (horizontalDistance >= maxDistance && Time.time - lastDirectionChangeTime >= directionChangeCooldown) {
            ReverseDirection();
        }

        // Update shadow projector in normal mode
        shadowRayTimer += Time.deltaTime;
        if (shadowRayTimer >= shadowRayInterval) {
            shadowRayTimer = 0f;
            UpdateShadowProjector();
        }
    }

    /// <summary>
    /// Checks for wall collisions to trigger direction reversal.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private void OnCollisionEnter(Collision collision) {
        if (((1 << collision.gameObject.layer) & wallLayer) != 0 && Time.time - lastDirectionChangeTime >= directionChangeCooldown) {
            ReverseDirection();
        }
    }

    /// <summary>
    /// Reverses the movement direction and moves the object downward slightly.
    /// </summary>
    private void ReverseDirection() {
        direction *= -1;
        transform.position += Vector3.down * stepDownDistance;
        lastDirectionChangeTime = Time.time;
    }

    /// <summary>
    /// Returns the current movement direction (1 = right, -1 = left).
    /// </summary>
    public int GetDirection() {
        return direction;
    }

    private void UpdateShadowProjector() {
        if (shadowProjector == null) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, shadowRayDistance, environmentLayer)) {
            float height = Vector3.Distance(transform.position, hit.point);

            if (!shadowProjector.gameObject.activeSelf)
                shadowProjector.gameObject.SetActive(true);

            float t = Mathf.InverseLerp(0f, projectorDepth, height);
            float size = Mathf.Lerp(5, 10, t);
            shadowProjector.size = new Vector3(size, size, projectorDepth);
        }
        else {
            if (shadowProjector.gameObject.activeSelf)
                shadowProjector.gameObject.SetActive(false);
        }
    }
}