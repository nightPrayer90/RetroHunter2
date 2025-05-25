using UnityEngine;
using UnityEngine.EventSystems;
using RetroHunter2;

public class SliderUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerUpHandler {

    [Header("Audio")]
    [SerializeField] private SoundIndexKey hoverSound;
    [SerializeField] private SoundIndexKey clickSound;
    [SerializeField] private SoundIndexKey releaseSound;

    public void OnPointerEnter(PointerEventData eventData) {
        if (hoverSound != SoundIndexKey.none)
            AudioManager.Instance.PlaySFX(hoverSound);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (clickSound != SoundIndexKey.none)
            AudioManager.Instance.PlaySFX(clickSound);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (releaseSound != SoundIndexKey.none)
            AudioManager.Instance.PlaySFX(releaseSound);
    }
}