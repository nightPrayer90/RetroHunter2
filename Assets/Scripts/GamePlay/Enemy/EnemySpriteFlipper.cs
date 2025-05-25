using UnityEngine;
using DG.Tweening;


/// <summary>
/// Flips the sprite optionally based on movement direction or time-based flipping.
/// Also applies a squash/stretch animation.
/// </summary>
public class EnemySpriteFlipper : MonoBehaviour {
    [Header("Flip Settings")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipWithDirection = true;
    [SerializeField] private SpaceInvaderMovement movement; // optional
    [SerializeField] private float flipInterval = 0.5f;

    [Header("Scale Animation")]
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float scaleAmplitude = 0.05f;
    [SerializeField] private Axis scaleAxis = Axis.X;

    private Vector3 baseScale;
    private float flipTimer;

    private void Start() {
        baseScale = transform.localScale;

        spriteRenderer.gameObject.transform.localScale = Vector3.zero;

        // Scale to base with punch
        spriteRenderer.gameObject.transform.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                spriteRenderer.gameObject.transform.DOPunchScale(Vector3.one * 0.2f, 0.1f, 10, 1f);
    
            });
    }

    private void Update() {
        if (spriteRenderer != null) {
            if (flipWithDirection && movement != null)  {
                spriteRenderer.flipX = movement.GetDirection() > 0;
            }
            else {
                flipTimer += Time.deltaTime;
                if (flipTimer >= flipInterval) {
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                    flipTimer = 0f;
                }
            }
        }

        float scaleOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * scaleAmplitude;
        Vector3 newScale = baseScale;

        switch (scaleAxis) {
            case Axis.X: newScale.x += scaleOffset; break;
            case Axis.Y: newScale.y += scaleOffset; break;
            case Axis.Z: newScale.z += scaleOffset; break;
        }

        transform.localScale = newScale;
    }

    public enum Axis { X, Y, Z }
}