using UnityEngine;
using RetroHunter2;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles health, damage, sprite change, and death logic for a PanicWalker.
/// </summary>
public class PanicWalkerHealth : MonoBehaviour {
    [SerializeField] private int currentHP;
    [SerializeField] private PanicWalkerNavMesh walkerNav;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] damageSprites;
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.1f;
    [SerializeField] private ParticleSystem bloodParticle;
    private Color originalColor;

    [Header("Death")]
    [SerializeField] private GameObject diePrefab;
    [SerializeField] private GameObject splatPrefab;
    [SerializeField] private SoundIndexKey hitSound;
    [SerializeField] private SoundIndexKey dieSound;

    [SerializeField] private string walkerName;

    [HideInInspector] public GameManager gameManager;


    [Header("HappyTime")]
    [SerializeField] private Sprite happySprite;
    [SerializeField] private float happyDuration = 1f;
    private float nextHappySpriteTime;
    private bool isShowingHappy = false;
    private Sprite defaultSprite;

    private void Awake() {
        currentHP = damageSprites.Length;
        originalColor = spriteRenderer.color;

        if (spriteRenderer == null) {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (damageSprites.Length > 0) {
            spriteRenderer.sprite = damageSprites[0];
        }

        nextHappySpriteTime = Time.time + Random.Range(2f, 5f);
        defaultSprite = spriteRenderer.sprite;
    }

    private void Update() {
        if(currentHP == damageSprites.Length) {
            if (Time.time >= nextHappySpriteTime) {
                if (isShowingHappy) {
                    spriteRenderer.sprite = defaultSprite;
                    isShowingHappy = false;
                    nextHappySpriteTime = Time.time + Random.Range(2f, 5f);
                }
                else {
                    spriteRenderer.sprite = happySprite;
                    isShowingHappy = true;
                    nextHappySpriteTime = Time.time + happyDuration;
                }
            }
        }
    }


    public void TakeDamage(int amount = 1) {
        // Early exit if already dead
        if (currentHP <= 0) return;

        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, damageSprites.Length);

        // If still alive after damage
        if (currentHP > 0) {
            // Visual + Audio feedback
            gameManager.ShowSimpleFloatingText(
                gameManager.messagePool.GetRandomHitMessage(),
                transform.position + Vector3.up * 25f,
                Color.orange
            );

            bloodParticle.Emit(35);
            FlashColor(hitFlashColor);
            //walkerNav?.PauseAndRestartMovement(1f);

            AudioManager.Instance.PlaySFX(hitSound);

            spriteRenderer.sprite = damageSprites[damageSprites.Length - currentHP];
        }
        // If killed by this damage
        else {
            gameManager.ShowSimpleFloatingText(
                gameManager.messagePool.GetRandomDeathMessage(),
                transform.position + Vector3.up * 25f,
                Color.red
            );

            bloodParticle.Emit(50);
            AudioManager.Instance.PlaySFX(dieSound);
            Die();
        }
    }

    private void Die() {
        if (diePrefab != null) {
            Instantiate(diePrefab, transform.position, transform.rotation);
            Instantiate(splatPrefab, transform.position, transform.rotation);
        }

        string message = gameManager.messagePool.GetRandomNarrativeDeathMessage(walkerName);
        gameManager.uiManager.ShowWalkerDeathMessage(message);

        walkerNav?.WalkerDie();
        gameManager.AdjustWalkerCount(-1);
        Destroy(gameObject);
    }

    public void SetGameManager(GameManager gm) {
        gameManager = gm;
    }

    /// <summary>
    /// Assigns the walker's unique name during spawn.
    /// </summary>
    public void SetWalkerName(string name) {
        walkerName = name;
    }

    public void FlashColor(Color color) {
        spriteRenderer.DOKill();
        spriteRenderer.color = Color.white;
        spriteRenderer.DOColor(color, hitFlashDuration)
            .OnComplete(() => spriteRenderer.DOColor(originalColor, hitFlashDuration));
    }
}