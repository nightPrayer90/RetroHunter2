using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Displays floating text (e.g. speech or reactions) with animation and fade.
/// </summary>
public class FloatingTextSimple : MonoBehaviour {
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatDistance = 1f;
    [SerializeField] private float duration = 1.5f;

    public void Init(string message, Vector3 worldPos, Color color) {
        text.text = message;
        text.color = color;
        transform.position = worldPos;
        transform.localScale = Vector3.one;

        transform.DOMoveY(worldPos.y + floatDistance, duration).SetEase(Ease.OutQuad);
        text.DOFade(0f, duration / 2f).SetDelay(duration / 2f).SetEase(Ease.InExpo);

        Destroy(gameObject, duration + 0.1f);
    }
}