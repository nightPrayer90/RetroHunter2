using UnityEngine;
using System.Collections.Generic;
using System;
using RetroHunter2;
using DG.Tweening;

/// <summary>
/// Modular health system for enemies. Fires a C# event on death, but does not handle death itself.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyHealth : MonoBehaviour {
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    public bool isMarkedByKelvin { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 100;
    [HideInInspector] public GameManager gameManager;

    [Header("Death Effects")]
    [SerializeField] private GameObject diePrefab;
    [SerializeField] private SoundIndexKey dieSound;
    [SerializeField] private GameObject escapePrefab;
    [SerializeField] private SoundIndexKey escapeSound;

    [Header("Escape Settings")]
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float escapeYThreshold = -10f;
    [SerializeField] private float checkEscapeInterval = 0.5f;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private Color originalColor;

    [Header("Optional: Sprite Swapping")]
    [SerializeField] private bool useDamageSprites = false;
    [SerializeField] private List<Sprite> damageSprites;

    private SpawnManager spawnManager;
    public event Action OnDeath;


    private void Awake() {
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;

        InvokeRepeating(nameof(CheckEscapeConditions), 0f, checkEscapeInterval);
    }

    /// <summary>
    /// Handles trigger-based death caused by environment contact.
    /// </summary>
    private void OnTriggerEnter(Collider other) {

        if (((1 << other.gameObject.layer) & environmentLayer) != 0) {
            Escape();
        }
    }

    private void OnTriggerStay(Collider other) {

        if (((1 << other.gameObject.layer) & environmentLayer) != 0) {
            Escape();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        int layer = collision.gameObject.layer;

        if (((1 << layer) & environmentLayer) != 0) {
            Debug.Log("OnCollision");
            Escape();
        }

        if (layer == gameObject.layer) {
            Debug.Log("OnEnemyCollision");
            Escape();
        }
    }

    public void SetSpawnManager(SpawnManager manager) {
        spawnManager = manager;
    }

    /// <summary>
    /// Assigns the GameManager reference.
    /// </summary>
    public void SetGameManager(GameManager gm) {
        gameManager = gm;
    }

    /// <summary>
    /// Returns the score value this enemy gives on death.
    /// </summary>
    public int GetScoreValue() {
        return scoreValue;
    }

    /// <summary>
    /// Reduces health and triggers death event if necessary.
    /// </summary>
    public void TakeDamage(int amount) {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0) {
            Die();
            spawnManager?.NotifyEnemyKilled();
        }
        else {
            // only for enemies with more then 1 HP
            UpdateSpriteByHealth();
        }
    }

    /// <summary>
    /// Checks if the enemy has fallen below the defined Y threshold.
    /// </summary>
    private void CheckEscapeConditions() {
        if (!isDead && transform.position.y <= escapeYThreshold) {
            Escape();
        }
    }

    // <summary>
    /// Handles forced death from environmental hazards or falling off the map.
    /// No score is granted.
    /// </summary>
    public void Escape(float destroyDelay = 0f) {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();

        if (escapePrefab != null) {
            Instantiate(escapePrefab, transform.position, transform.rotation);
        }

        spawnManager?.NotifyEnemyRemoved();
        if (escapeSound != SoundIndexKey.none)
            AudioManager.Instance.PlaySFXWithRandomPitch(escapeSound, 0.5f, 0f);
        Destroy(gameObject, destroyDelay);
    }


    /// <summary>
    /// Publicly callable death behavior (e.g. from SpawnManager or another system).
    /// </summary>
    public void Die(float destroyDelay = 0f) {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();

        Vector3 hitPoint = transform.position + Vector3.up * 5f;

        if (diePrefab != null) {
            Instantiate(diePrefab, transform.position, transform.rotation);
        }

        if (gameManager != null) {
            gameManager.AddScore(scoreValue, hitPoint);
        }

        AudioManager.Instance.PlaySFXWithRandomPitch(dieSound, 0.5f, 0f);
        Destroy(gameObject, destroyDelay);
    }

    public void SetKelvinMarked(bool value) {
        isMarkedByKelvin = value;
    }

    private void UpdateSpriteByHealth() {
        if (!useDamageSprites) return;

        AudioManager.Instance.PlaySFX(SoundIndexKey.enemy01Die);
        Instantiate(diePrefab, transform.position, transform.rotation);

        int index = Mathf.Clamp(maxHealth - currentHealth, 0, damageSprites.Count - 1);
        spriteRenderer.sprite = damageSprites[index];

        // Punch - Effect
        spriteRenderer.transform.DOKill();
        spriteRenderer.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 10, 0.5f); 
    }

    /// <summary>
    /// Flashes the enemy sprite with a highlight color.
    /// </summary>
    public void FlashColor() {
        if (spriteRenderer == null) return;

        spriteRenderer.DOKill();
        spriteRenderer.color = Color.white;
        spriteRenderer.DOColor(flashColor, flashDuration)
            .OnComplete(() => spriteRenderer.DOColor(originalColor, flashDuration));
    }
}