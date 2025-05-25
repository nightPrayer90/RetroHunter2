using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class PostProcessingManager : MonoBehaviour {
    [SerializeField] private Volume volume;
    private Vignette vignette;
    private Tween intensityTween;
    private Tween colorTween;

    private void Awake() {
        Debug.Log("PostProcessingManager: Awake() called!");

        if (volume == null) {
            Debug.LogError("Volume reference is null!");
            return;
        }

        if (volume.profile == null) {
            Debug.LogError("Volume profile is null!");
            return;
        }

        if (volume.profile.TryGet(out Vignette v)) {
            vignette = v;
            Debug.Log("Vignette found: " + v);
        }
        else {
            Debug.LogError("Vignette not found in profile.");
        }
    }

    /// <summary>
    /// Triggers a red flash and fades to the target intensity based on walker count. Player loose a walker
    /// </summary>
    public void FlashAndSetVignette(int walkerCount) {
        if (vignette == null) return;

        Debug.Log("Trigger Flash Vignette" + walkerCount);

        float targetIntensity = GetTargetIntensity(walkerCount);

        // Kill any running tweens
        intensityTween?.Kill();
        colorTween?.Kill();

        // Flash red immediately
        vignette.color.Override(Color.red);
        vignette.intensity.Override(0.5f);

        // Fade back to normal (black + desired intensity)
        intensityTween = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.Override(x), targetIntensity, 0.25f)
                                .SetEase(Ease.OutQuad);
        colorTween = DOTween.To(() => vignette.color.value, x => vignette.color.Override(x), Color.black, 0.25f)
                            .SetEase(Ease.OutQuad);
    }


    /// <summary>
    /// Triggers a red flash and fades to the target intensity based on walker count. Player get a new walker
    /// </summary>
    public void UpdateVignetteIntensity(int walkerCount) {
        if (vignette == null) return;

        Debug.Log("Trigger UpdateVignette " + walkerCount);

        float targetIntensity = GetTargetIntensity(walkerCount);

        // Kill any running tweens
        intensityTween?.Kill();

        // Fade back to normal (black + desired intensity)
        intensityTween = DOTween.To(() => vignette.intensity.value, x => vignette.intensity.Override(x), targetIntensity, 0.25f)
                                .SetEase(Ease.OutQuad);
       
    }

    /// <summary>
    /// Returns desired vignette intensity based on remaining walkers.
    /// </summary>
    private float GetTargetIntensity(int walkerCount) {
        return walkerCount switch {
            <= 0 => 0.8f,
            1 => 0.6f,
            2 => 0.4f,
            3 => 0.2f,
            _ => 0.1f
        };
    }
}