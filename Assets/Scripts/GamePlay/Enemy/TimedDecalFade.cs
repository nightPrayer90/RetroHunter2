using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Fades out a DecalProjector's opacity over time and destroys the object.
/// </summary>
public class TimedDecalFade : MonoBehaviour {
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float fadeStartTime = 5f;

    [SerializeField] private DecalProjector projector;
    private float timer;
    private float fadeDuration;
    private float decalSize = 0;

    private void Awake() {
        fadeDuration = lifeTime - fadeStartTime;
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    private void Update() {
        timer += Time.deltaTime;

        if (decalSize < 0.9f) {
            decalSize += 0.05f;
            projector.fadeFactor = decalSize;
        }

        if (timer >= fadeStartTime) {
            float t = Mathf.Clamp01((timer - fadeStartTime) / fadeDuration);
            projector.fadeFactor = Mathf.Lerp(1f, 0f, t);
        }

        if (timer >= lifeTime) {
            Destroy(gameObject);
        }
    }
}