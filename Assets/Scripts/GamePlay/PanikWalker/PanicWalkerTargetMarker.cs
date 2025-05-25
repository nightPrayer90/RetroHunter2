using UnityEngine;
using DG.Tweening;

public class PanicWalkerTargetMarker : MonoBehaviour {
    private Transform followTarget;
    private EnemyHealth health;
    private bool targetAlive = true;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float idlePunchScale = 0.05f;
    [SerializeField] private float idlePunchTime = 1.2f;

    private Tween idleLoopTween;

    public void Init(EnemyHealth target) {
        health = target;
        followTarget = target.transform;

        health.OnDeath += HandleTargetDeath;
        health.SetKelvinMarked(true);

        // Punch-Spawn Animation
        transform.localScale = Vector3.one * 0.5f;
        transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 8, 0.6f).OnComplete(StartIdleLoop);
    }

    private void Update() {
        if (targetAlive && followTarget != null) {
            transform.position = followTarget.position + Vector3.up * 1.5f;
        }
    }

    private void StartIdleLoop() {
        idleLoopTween = transform
            .DOPunchScale(Vector3.one * idlePunchScale, idlePunchTime, 4, 0.4f)
            .SetLoops(-1)
            .SetEase(Ease.InOutQuad);
    }

    private void HandleTargetDeath() {
        targetAlive = false;
        followTarget = null;

        health.OnDeath -= HandleTargetDeath;

        // Stop loop + fade out
        if (idleLoopTween != null) idleLoopTween.Kill();

        spriteRenderer.DOFade(0f, fadeOutDuration).OnComplete(UnlinkAndDestroy);
    }

    private void OnDestroy() {
        UnlinkAndDestroy(); // falls durch andere Quelle zerstört
    }

    public void UnlinkAndDestroy() {
        if (health != null)
            health.OnDeath -= HandleTargetDeath;

        spriteRenderer.DOKill();
        health?.SetKelvinMarked(false);
        Destroy(gameObject);
    }
}