using UnityEngine;
using UnityEngine.EventSystems;
using RetroHunter2;

public class ButtonUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler {

    [Header("Text Handling")]
    [SerializeField] private RectTransform textTransform;
    [SerializeField] private Vector2 hoverOffset = new Vector2(0, -4);
    [SerializeField] private Vector2 pressedOffset = new Vector2(0, -4);

    [Header("Audio")]
    [SerializeField] private SoundIndexKey hoverSound = SoundIndexKey.MouseHover;
    [SerializeField] private SoundIndexKey exitSound = SoundIndexKey.none;
    [SerializeField] private SoundIndexKey clickSound = SoundIndexKey.none;

    [SerializeField] bool isPointerEnter = false;
    [SerializeField] bool isPointerDown = false;
    private Vector2 originalTextPos;

    private void Awake() {
        if (textTransform != null)
            originalTextPos = textTransform.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (textTransform != null) {
            if (isPointerDown == false) {
                textTransform.anchoredPosition = originalTextPos + hoverOffset;
                AudioManager.Instance.PlaySFX(hoverSound);
            }
        }
        isPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (textTransform != null) {
            if (isPointerDown == true) {
                textTransform.anchoredPosition = originalTextPos + pressedOffset;
            }
            else {
                if (exitSound != SoundIndexKey.none)
                    AudioManager.Instance.PlaySFX(exitSound);
                textTransform.anchoredPosition = originalTextPos;
            }
        }
            
        isPointerEnter = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (textTransform != null) {
            textTransform.anchoredPosition = originalTextPos + pressedOffset;
            if (clickSound != SoundIndexKey.none)
                AudioManager.Instance.PlaySFX(clickSound);
        }
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (textTransform != null) {
            if (isPointerEnter == true) {
                textTransform.anchoredPosition = originalTextPos + hoverOffset;
            }
            else {
                textTransform.anchoredPosition = originalTextPos;
            }
        }

        EventSystem.current.SetSelectedGameObject(null);
        isPointerDown = false;
    }
}