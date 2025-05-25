using UnityEngine;

public class StoryWindowSpriteFlipper : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Wobble Animation")]
    [SerializeField] private float frequency = 2f;
    [SerializeField] private float scaleAmplitude = 0.05f;
    [SerializeField] private Axis scaleAxis = Axis.X;

    [Header("Talking Mode")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite talkSprite;
    [SerializeField] private float talkSwitchInterval = 0.2f;

    [Header("Sprite Index Mode")]
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private int spriteIndex = 0;

    private Vector3 baseScale;
    private float talkTimer;
    private bool showTalkSprite;
    [SerializeField] private bool talkingMode = false;

    private int lastSpriteIndex = -1;

    private void Start() {
        baseScale = transform.localScale;
    }

    private void Update() {
        // Wobble
        float scaleOffset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * scaleAmplitude;
        Vector3 newScale = baseScale;

        switch (scaleAxis) {
            case Axis.X: newScale.x += scaleOffset; break;
            case Axis.Y: newScale.y += scaleOffset; break;
            case Axis.Z: newScale.z += scaleOffset; break;
        }

        transform.localScale = newScale;

        // Sprite Handling
        if (talkingMode && spriteRenderer != null && talkSprite != null && idleSprite != null) {
            talkTimer += Time.deltaTime;
            if (talkTimer >= talkSwitchInterval) {
                talkTimer = 0f;
                showTalkSprite = !showTalkSprite;
                spriteRenderer.sprite = showTalkSprite ? talkSprite : idleSprite;
            }
        }
        else if (spriteRenderer != null && spriteList != null && spriteList.Length > 0) {

            if (spriteIndex != lastSpriteIndex && spriteIndex >= 0 && spriteIndex < spriteList.Length) {
                spriteRenderer.sprite = spriteList[spriteIndex];
                lastSpriteIndex = spriteIndex;
            }
        }
    }

    public void SetTalkingMode(bool isTalking) {
        talkingMode = isTalking;
        talkTimer = 0f;
        showTalkSprite = false;
        lastSpriteIndex = -1; 
    }

    public enum Axis { X, Y, Z }
}