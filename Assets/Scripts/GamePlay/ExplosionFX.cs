using UnityEngine;
using RetroHunter2;

/// <summary>
/// Explosion FX that plays sound and animates itself before being destroyed.
/// </summary>
public class ExplosionFX : MonoBehaviour {
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
        Destroy(gameObject, duration);
    }

    /// <summary>
    /// Immediately destroys this FX object.
    /// </summary>
    public void DestroySelf() {
        Destroy(gameObject);
    }
}