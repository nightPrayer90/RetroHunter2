using UnityEngine;
using DG.Tweening;

/// <summary>
/// Handles camera effects (e.g. shake) and smooth offset movement based on crosshair position.
/// </summary>
public class CameraController : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform cameraStartTransform;
    [SerializeField] private Transform cameraRig;         // Rig that moves with crosshair
    [SerializeField] private Transform cameraTransform;   // Actual camera for shake only
    [SerializeField] private RectTransform crosshairUI;
    [SerializeField] private Canvas canvas;

    [Header("Shake Settings")]
    [SerializeField] private int shakeVibrato = 10;

    [Header("Crosshair Panning")]
    [SerializeField] private float maxOffset = 3f;
    [SerializeField] private float panSmoothing = 5f;

    [Header("Intro")]

    private Vector3 basePosition;
    private Vector3 cameraRootBasePosition;
    private Quaternion cameraRootBaseRotation;

    private void Awake() {
        if (cameraRig == null)
            cameraRig = transform;

        basePosition = cameraRig.position;

        cameraRootBasePosition = cameraRoot.position;
        cameraRootBaseRotation = cameraRoot.rotation;
    }

    private void Update() {
        HandleCrosshairPanning();
    }

    /// <summary>
    /// Moves the camera rig smoothly based on the crosshair's distance from canvas center.
    /// </summary>
    private void HandleCrosshairPanning() {
        if (crosshairUI == null || canvas == null) return;

        Vector2 canvasSize = canvas.GetComponent<RectTransform>().rect.size;
        Vector2 canvasCenter = canvasSize / 2f;
        Vector2 delta = crosshairUI.anchoredPosition - canvasCenter;

        Vector2 normalized = new Vector2(
            delta.x / canvasCenter.x,
            delta.y / canvasCenter.y
        );

        Vector3 offset = new Vector3(
            normalized.x * maxOffset,
            normalized.y * maxOffset,
            0f
        );

        Vector3 targetPos = Vector3.Lerp(cameraRig.position, basePosition + offset, Time.deltaTime * panSmoothing);
        cameraRig.position = targetPos;
    }

    /// <summary>
    /// Applies a shake effect directly to the cameraTransform.
    /// </summary>
    public void Shake(float duration, float intensity) {
        cameraTransform.DOKill();
        cameraTransform.localPosition = Vector3.zero;

        cameraTransform.DOShakePosition(duration, intensity, shakeVibrato)
            .OnComplete(() => cameraTransform.localPosition = Vector3.zero);
    }

    // <summary>
    /// Moves the camera root from intro start position to gameplay position.
    /// </summary>
    public void PlayWaveIntro() {
        cameraRoot.position = cameraStartTransform.position;
        cameraRoot.rotation = cameraStartTransform.rotation;

        cameraRoot.DOMove(cameraRootBasePosition, 1.5f).SetDelay(1.2f).SetEase(Ease.InOutSine);
        cameraRoot.DORotateQuaternion(cameraRootBaseRotation, 1.5f).SetDelay(1.2f);
    }
}
