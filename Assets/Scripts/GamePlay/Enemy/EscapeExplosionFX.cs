using UnityEngine;
using RetroHunter2;

/// <summary>
/// Explosion FX that plays sound, animation, and applies area damage via OverlapSphere.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EscapeExplosionFX : MonoBehaviour {
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private GameObject creakPrefab;

    [Header("Animation & Audio")]
    [SerializeField] private Animator animator;
    [SerializeField] private SoundIndexKey explosionSound;

    private float duration = 0f;



    private void Start() {
        AudioManager.Instance.PlaySFXWithRandomPitch(explosionSound, 0.5f, 0.2f);

        if (animator != null && animator.runtimeAnimatorController != null) {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0) {
                duration = clips[0].length;
            }
        }

        Instantiate(creakPrefab, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, targetLayers);

        foreach (var hit in hits) {
            PanicWalkerHealth health = hit.GetComponent<PanicWalkerHealth>();
            if (health != null) {
                health.TakeDamage(1);
            }
        }
        Destroy(gameObject, duration);
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, explosionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}