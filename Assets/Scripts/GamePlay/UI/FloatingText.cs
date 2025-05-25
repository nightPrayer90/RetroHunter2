using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Displays floating score text with a fade/move animation.
/// </summary>
public class FloatingText : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatDistance = 1f;

    /// <summary>
    /// Initializes and plays the floating score text at a world position.
    /// </summary>
    /// <param name="score">The score to display (e.g. 100)</param>
    /// <param name="worldPos">The world position to spawn the text at</param>
    /// <param name="duration">Duration of the animation in seconds</param>
    public void Init(int score, Vector3 worldPos, float duration, Color textColor, float multiplier = 1f, bool isDouble = false) {
        text.text = $"+{score}";
        text.color = textColor;
        transform.position = worldPos;

        // Skaliere den gesamten Text je nach Multiplier
        float scale = 1f + Mathf.Log(multiplier, 2f) * 0.2f;

        if (isDouble) {
            scale *= 2f;
        }

        transform.localScale = Vector3.one * scale;

        transform.DOMoveY(worldPos.y + floatDistance, duration).SetEase(Ease.OutQuad);
        text.DOFade(0f, duration/2).SetDelay(duration / 2).SetEase(Ease.InExpo);

        Destroy(gameObject, duration + 0.1f);
    }
}