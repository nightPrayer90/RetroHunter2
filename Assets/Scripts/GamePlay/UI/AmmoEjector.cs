using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class AmmoEjector : MonoBehaviour {
    [SerializeField] private GameObject ejectPrefab; // Sprite der Hülse (UI)
    [SerializeField] private RectTransform flyOutRoot; // Canvas-Root für fliegende Objekte

    /// <summary>
    /// Spawns a flying ammo sprite effect at a given world position.
    /// </summary>
    public void Eject(Vector3 worldPosition) {
        if (ejectPrefab == null || flyOutRoot == null) return;

        GameObject instance = Instantiate(ejectPrefab, flyOutRoot);
        instance.transform.position = worldPosition;

        RectTransform rect = instance.GetComponent<RectTransform>();

        float arcHeight = Random.Range(60f, 100f);  // wie stark sie "aufsteigt"
        float distanceX = Random.Range(100f, 180f); // wie weit sie nach rechts fliegt
        float spin = Random.Range(360f, 720f);

        Vector2 upRight = new Vector2(distanceX * 0.5f, arcHeight);
        Vector2 downRight = new Vector2(distanceX * 0.5f, -arcHeight * 1.5f);

        Sequence seq = DOTween.Sequence();

        // Phase 1: nach oben rechts (Bogenbeginn)
        seq.Append(rect.DOAnchorPos(upRight, 0.3f)
            .SetRelative(true)
            .SetEase(Ease.InQuad))
            .AppendCallback(() => {
            AudioManager.Instance.PlaySFX(RetroHunter2.SoundIndexKey.bulletEjectSound);
        });


        // Phase 2: nach unten rechts (Bogenende)
        seq.Append(rect.DOAnchorPos(downRight, 0.4f)
           .SetRelative(true)
           .SetEase(Ease.InQuad));

        // gleichzeitig: Spin & Skalierung
        seq.Join(rect.DORotate(new Vector3(0f, 0f, spin), 0.7f, RotateMode.FastBeyond360));
        seq.Join(rect.DOScale(0.3f, 0.7f));

        // optional: Fade out
        CanvasGroup cg = instance.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        seq.Join(cg.DOFade(0f, 0.4f).SetDelay(0.3f));

        seq.OnComplete(() => Destroy(instance));
    }

    public void AnimateReloadSequence(List<Image> filledIcons, float reloadTime, int currentAmmo) {
        int total = filledIcons.Count;
        int bulletsToReload = total - currentAmmo;
        if (bulletsToReload <= 0) return;

        float delayStep = reloadTime / bulletsToReload;

        for (int i = currentAmmo; i < total; i++) {
            Image icon = filledIcons[i];
            icon.enabled = false;

            int localIndex = i; // für Closure
            DOVirtual.DelayedCall(delayStep * (i - currentAmmo), () => {
                icon.enabled = true;

                Transform t = icon.transform;
                t.DOKill();
                t.localScale = Vector3.zero;
                t.DOScale(0.5f, 0.2f).SetEase(Ease.OutBack);

                icon.color = Color.white;
                icon.DOColor(new Color(1f, 1f, 0.7f), 0.1f)
                    .OnComplete(() => icon.DOColor(Color.white, 0.1f));
            });
        }
    }

    public void AnimateInstantReload(List<Image> filledIcons, int currentAmmo) {
        int total = filledIcons.Count;
        int bulletsToReload = total - currentAmmo;
        if (bulletsToReload <= 0) return;

        

        for (int i = currentAmmo; i < total; i++) {
            Image icon = filledIcons[i];

            icon.enabled = true;

            Transform t = icon.transform;
            t.DOKill();
            t.localScale = Vector3.zero;
            t.DOScale(0.5f, 0.2f).SetEase(Ease.OutBack);

            icon.color = Color.white;
            icon.DOColor(new Color(1f, 1f, 0.7f), 0.1f)
                .OnComplete(() => icon.DOColor(Color.white, 0.1f));
        }
    }
}