using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using RetroHunter2;

public class UpgradePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image upgradeIconImage;
    [SerializeField] private RectTransform visualContainer;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Hover Animation")]
    [SerializeField] private float tweenDuration = 0.15f;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.8f);

    public UpgradeType upgradeType { get; private set; }
    public UpgradeData upgradeData { get; private set; }

    private UpgradeStatusUI statusUI;
    private Action _onSelected;
    private Vector3 _originalScale;
    private Color _originalColor;
    private Tweener _scaleTween;

    /// <summary>
    /// Initializes hidden state on awake to avoid flicker.
    /// </summary>
    private void Awake() {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        visualContainer.localScale = Vector3.one * 0.8f;
    }

    /// <summary>
    /// Initializes the panel with upgrade data and selection logic.
    /// </summary>
    public void Setup(UpgradeData data, Action onSelected, UpgradeStatusUI statusUI) {
        titleText.text = data.upgradeName;
        descriptionText.text = data.description;
        upgradeIconImage.sprite = data.icon;
        this.upgradeData = data;
        this.upgradeType = data.upgradeType;
        this.statusUI = statusUI;
        this._onSelected = onSelected;

        visualContainer.localScale = Vector3.one;
        _originalScale = visualContainer.localScale;
        _originalColor = backgroundImage.color;

        if (canvasGroup != null) {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
        visualContainer.localScale = Vector3.one * 0.8f;
    }


    /// <summary>
    /// Triggers hover animation and preview display.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SoundIndexKey.MouseHover);
        _scaleTween?.Kill();

        visualContainer.localScale = _originalScale;
        _scaleTween = visualContainer.DOPunchScale(Vector3.one * 0.05f, tweenDuration, 4, 0.6f).SetUpdate(true);
        backgroundImage.DOColor(hoverColor, tweenDuration).SetUpdate(true);

        statusUI?.Highlight(upgradeType);
    }

    /// <summary>
    /// Resets visual state on hover exit.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) {
        _scaleTween?.Kill();
        _scaleTween = visualContainer.DOScale(_originalScale, tweenDuration).SetEase(Ease.OutBack).SetUpdate(true);
        backgroundImage.DOColor(_originalColor, tweenDuration).SetUpdate(true);

        statusUI?.ResetHighlighting();
    }

    /// <summary>
    /// Applies upgrade when clicked.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SoundIndexKey.upgradeGet);
        _onSelected?.Invoke();
    }

    /// <summary>
    /// Plays the show animation with optional delay.
    /// </summary>
    public void PlayShowEffect(float delay = 0f) {
        if (canvasGroup != null) {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(1f, 0.2f)
                .SetDelay(delay)
                .SetUpdate(true)
                .OnComplete(() => {
                    canvasGroup.blocksRaycasts = true;
                });
        }

        visualContainer.localScale = Vector3.one * 0.8f;
        visualContainer.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetDelay(delay)
            .SetUpdate(true);
    }

    /// <summary>
    /// Plays the hide animation with optional delay and callback.
    /// </summary>
    public void PlayHideEffect(float delay = 0f, Action onComplete = null) {
        if (canvasGroup != null) {
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0f, 0.2f)
                .SetDelay(delay)
                .SetUpdate(true)
                .OnComplete(() => {
                    onComplete?.Invoke();
                });
        }
        else {
            onComplete?.Invoke();
        }

        visualContainer.DOScale(Vector3.one * 0.8f, 0.2f)
            .SetEase(Ease.InBack)
            .SetDelay(delay)
            .SetUpdate(true);
    }
}