using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Flips the sprite of a PanicWalker based on NavMeshAgent movement direction and adds wobble.
/// </summary>
public class PanicWalkerSpriteFlipper : MonoBehaviour {
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Wobble Animation")]
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float scaleAmplitude = 0.05f;
    [SerializeField] private Axis scaleAxis = Axis.X;

    private Vector3 baseScale;

    private void Start() {
        baseScale = transform.localScale;
        if (agent == null) agent = GetComponentInParent<NavMeshAgent>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() {
        if (agent != null && spriteRenderer != null) {
            float vx = agent.velocity.x;
            if (Mathf.Abs(vx) > 0.01f) {
                spriteRenderer.flipX = vx < 0;
            }

            // Wobble
            float scaleOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * scaleAmplitude;
            Vector3 newScale = baseScale;

            switch (scaleAxis) {
                case Axis.X: newScale.x += scaleOffset; break;
                case Axis.Y: newScale.y += scaleOffset; break;
                case Axis.Z: newScale.z += scaleOffset; break;
            }

            transform.localScale = newScale;
        }
    }

    public enum Axis { X, Y, Z }
}