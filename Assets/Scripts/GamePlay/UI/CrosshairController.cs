using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RetroHunter2;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Replaces system cursor with a UI crosshair that follows mouse position.
/// Changes sprite depending on hover state and game state.
/// </summary>
public class CrosshairController : MonoBehaviour {
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameManager gameManger;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private Sprite cursorSprite;
    [SerializeField] private Sprite cursorSpriteHover;
    public bool IsOverUI { get; private set; }
    private Coroutine flashRoutine;
    private Tweener scaleTween;

    private void Awake() {
        // Hide system cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update() {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 anchoredPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.worldCamera,
            out anchoredPos
        );

        crosshairUI.anchoredPosition = anchoredPos;

        // Determine hover state and set appropriate sprite
        IsOverUI = IsPointerOverUILayer();

        if (IsOverUI) {
            crosshairImage.sprite = cursorSpriteHover;
            crosshairImage.SetNativeSize();
        }
        else {
            switch (gameManger.gameState) {
                case GameState.play:
                    crosshairImage.sprite = crosshairSprite;
                    break;
                default:
                    crosshairImage.sprite = cursorSprite;
                    break;
            }

            crosshairImage.SetNativeSize();
        }
    }

    /// <summary>
    /// Returns true if the pointer is currently over a UI element on the "UI" layer.
    /// </summary>
    private bool IsPointerOverUILayer() {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        
        foreach (var result in results) {
           // Debug.Log($"Hit: {result.gameObject.name} (Layer: {LayerMask.LayerToName(result.gameObject.layer)})");

            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                //Debug.Log($"Hovering UI Element (UI Layer): {result.gameObject.name}");
            return true;
        }

        return false;
    }

    public void AnimateHitFeedback(float punchScale = 1.25f, float duration = 0.2f) {
        if (scaleTween != null && scaleTween.IsActive()) {
            scaleTween.Kill();
        }

        crosshairUI.localScale = Vector3.one;
        scaleTween = crosshairUI.DOPunchScale(Vector3.one * (punchScale - 1f), duration, vibrato: 1, elasticity: 0.5f)
            .SetUpdate(true);
    }

    public void FlashCrosshair(Color flashColor, float holdDuration = 0.05f, float fadeDuration = 0.1f) {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(flashColor, holdDuration, fadeDuration));
    }

    private IEnumerator FlashRoutine(Color flashColor, float holdDuration, float fadeDuration) {
        crosshairImage.color = flashColor;

        yield return new WaitForSeconds(holdDuration);

        Color startColor = crosshairImage.color;
        Color targetColor = Color.white;

        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / fadeDuration;
            crosshairImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        crosshairImage.color = targetColor;
        flashRoutine = null;
    }
}