using UnityEngine;
using DG.Tweening;


public class FadeAndDestroySprite : MonoBehaviour {
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private SpriteRenderer sr;

    private void Start() {
        // Start fade
        sr.DOFade(0f, fadeDuration)
          .SetEase(Ease.Linear)
          .OnComplete(() => Destroy(gameObject));
    }
}