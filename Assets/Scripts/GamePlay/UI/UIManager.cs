using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using RetroHunter2;
using System.Collections;

/// <summary>
/// Handles all game-related UI updates: time, score, wave, ammo, combo system, rank progression, and screen state transitions.
/// </summary>
public class UIManager : MonoBehaviour {

    /* **************************************************************************** */
    /* TEXT & UI ELEMENTS -------------------------------------------------------- */
    /* **************************************************************************** */

    [Header("Text Displays")]
    [SerializeField] private TMP_Text waveTimeText;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text walkerText;

    [Header("Ammo Display")]
    [SerializeField] private Transform ammoBarContainer;
    [SerializeField] private GameObject bulletIconPrefab;
    private List<Image> filledIcons = new();
    [SerializeField] private AmmoEjector ammoEjector;

    [Header("Controls Display")]
    [SerializeField] private TMP_Text leftBtn;
    [SerializeField] private TMP_Text rightBtn;
    [SerializeField] private Color orginalColor;
    [SerializeField] private Color flashColor;

    [Header("Combo System")]
    [SerializeField] private Gradient comboColorGradient;
    [SerializeField] private Image comboRing;
    [SerializeField] private TMP_Text comboText;
    private Tweener _comboPunchTween;
    private Color _currentComboColor = Color.white;

    [Header("LevelUpSystem")]
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private Image rankProgressBar;
    private Tweener _rankPunchTween;

    [Header("HUD States")]
    [SerializeField] private CanvasGroup playerHudCG;
    [SerializeField] private CanvasGroup upgradeHudCG;
    [SerializeField] private CanvasGroup snorblesHudCG;
    [SerializeField] private CanvasGroup breakHudCG;
    [SerializeField] private CanvasGroup gameOverCG;

    [Header("Walker Death Feed")]
    [SerializeField] private Transform walkerMessageParent;
    [SerializeField] private WalkerDeathMessage walkerDeathMessagePrefab;

    [Header("GameOver")]
    [SerializeField] private TMP_Text hunterNameText;
    [SerializeField] private TMP_Text hunterScoreText;
    [SerializeField] private TMP_Text narrativeText;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [SerializeField] private TMP_Text playerMessageText;
    [SerializeField] private float messageDisplayTime = 3f;

    private void Start() {
        UpdateScoreDisplay();
    }


    /* **************************************************************************** */
    /* TIME / SCORE / WAVE -------------------------------------------------------- */
    /* **************************************************************************** */

    public void UpdateWaveTimeDisplay() {
        float t = gameManager.waveTimeRemaining;
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);
        waveTimeText.text = $"Wave {gameManager.currentWave} – {minutes:00}:{seconds:00}";
    }

    public void UpdateScoreDisplay() {
        totalScoreText.text = $"Total: {gameManager.totalScore}";
    }

    /// <summary>
    /// Plays a punch animation on the wave time text for visual feedback.
    /// </summary>
    public void AnimateWaveTimeText() {
        // Stop previous tween to prevent stacking effects
        waveTimeText.rectTransform.DOKill();

        // Play punch scale animation
        waveTimeText.rectTransform.DOPunchScale(new Vector3(0.3f, 0.3f, 0f), 0.3f, 6, 0.7f);
    }


    /* **************************************************************************** */
    /* AMMO BAR ------------------------------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Updates the visual ammo display by enabling or disabling bullet icons.
    /// </summary>
    public void UpdateAmmoBar(int currentAmmo) {
        for (int i = 0; i < filledIcons.Count; i++) {
            filledIcons[i].enabled = i < currentAmmo;
        }
    }

    /// <summary>
    /// Updates the visual ammo display after shooting.
    /// </summary>
    public void UpdateAmmoBarAfterShooting(int currentAmmo) {
        for (int i = 0; i < filledIcons.Count; i++) {
            bool wasActive = filledIcons[i].enabled;
            bool isActive = i < currentAmmo;

            if (!isActive && wasActive) {
                ammoEjector?.Eject(filledIcons[i].transform.position);
            }

            filledIcons[i].enabled = isActive;
        }
    }

    /// <summary>
    /// Initializes the ammo bar with the correct number of bullet icons.
    /// </summary>
    public void InitializeAmmoBar(int maxAmmo) {
        foreach (Transform child in ammoBarContainer) {
            Destroy(child.gameObject);
        }

        filledIcons.Clear();

        for (int i = 0; i < maxAmmo; i++) {
            GameObject icon = Instantiate(bulletIconPrefab, ammoBarContainer);
            Image filled = icon.transform.Find("AmmoFull")?.GetComponent<Image>();
            filledIcons.Add(filled);
        }
    }

    public void PlayReloadAnimation(float reloadTime, int currentAmmo) {
        ammoEjector.AnimateReloadSequence(filledIcons, reloadTime, currentAmmo);
    }

    public void PlayInstantReloadAnimation(int currentAmmo) {
        ammoEjector.AnimateInstantReload(filledIcons, currentAmmo);
    }

    public void FlashBtn(int btn = 0) {
        leftBtn.DOComplete();
        leftBtn.DOComplete();

        if (btn == 0) {
            leftBtn.color = flashColor;
            leftBtn.DOColor(orginalColor, 0.3f);
        }
        else {
            rightBtn.color = flashColor;
            rightBtn.DOColor(orginalColor, 0.3f);
        }
    }

    /* **************************************************************************** */
    /* COMBO SYSTEM --------------------------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Updates the combo ring fill, color, and multiplier text. Triggers punch effect if desired.
    /// </summary>
    public void SetComboVisual(float fill01, float multiplier, bool updateMultiplier) {
        comboRing.fillAmount = Mathf.Clamp01(fill01);

        float gradientKey = Mathf.Clamp01(Mathf.Log(multiplier, 5f) / 2f);
        Color color = comboColorGradient.Evaluate(gradientKey);

        comboText.color = color;
        comboRing.color = color;
        _currentComboColor = color;

        if (updateMultiplier) {
            if (_comboPunchTween != null && _comboPunchTween.IsActive())
                _comboPunchTween.Kill();

            AudioManager.Instance.PlaySFX(RetroHunter2.SoundIndexKey.multiplierUp);
            comboText.transform.localScale = Vector3.one;
            _comboPunchTween = comboText.rectTransform.DOPunchScale(Vector3.one * Mathf.Max(1, (multiplier * 0.5f)), 0.25f);
        }

        comboText.enabled = multiplier > 1;
        if (multiplier > 1)
            comboText.text = $"x{multiplier:0}";
    }

    /// <summary>
    /// Returns the current color based on the combo multiplier.
    /// </summary>
    public Color GetCurrentComboColor() => _currentComboColor;


    /* **************************************************************************** */
    /* LEVEL / RANK SYSTEM -------------------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Updates the rank display text and progress bar. Optionally plays a punch animation.
    /// </summary>
    public void SetHuntRankDisplay(int currentRank, float progress01, bool punch = false) {
        rankText.text = $"{currentRank}";
        rankProgressBar.fillAmount = Mathf.Clamp01(progress01);

        if (punch) {
            if (_rankPunchTween != null && _rankPunchTween.IsActive())
                _rankPunchTween.Kill();

            rankText.transform.localScale = Vector3.one;
            _rankPunchTween = rankText.transform.DOPunchScale(Vector3.one * 1.3f, 0.25f);

            // Farbflash: z. B. Gold → Weiß
            rankText.DOComplete();
            Color originalColor = rankText.color;
            rankText.color = new Color(1f, 0.85f, 0.3f); // Gold
            rankText.DOColor(originalColor, 0.25f).SetEase(Ease.InOutQuad);
        }
    }


    /* **************************************************************************** */
    /* SCREEN STATES (HUD SWITCHING) --------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Fades out all HUD groups and fades in the specified target group. Uses unscaled time.
    /// </summary>
    private void ShowCanvasGroup(CanvasGroup target) {
        playerHudCG.DOFade(0f, 0.1f).SetUpdate(true).OnComplete(() => playerHudCG.interactable = playerHudCG.blocksRaycasts = false);
        upgradeHudCG.DOFade(0f, 0.1f).SetUpdate(true).OnComplete(() => upgradeHudCG.interactable = upgradeHudCG.blocksRaycasts = false);
        gameOverCG.DOFade(0f, 0.1f).SetUpdate(true).OnComplete(() => gameOverCG.interactable = gameOverCG.blocksRaycasts = false);
        snorblesHudCG.DOFade(0f, 0.1f).SetUpdate(true).OnComplete(() => snorblesHudCG.interactable = snorblesHudCG.blocksRaycasts = false);
        breakHudCG.DOFade(0f, 0.1f).SetUpdate(true).OnComplete(() => breakHudCG.interactable = breakHudCG.blocksRaycasts = false);

        target.DOFade(1f, 0.2f).SetUpdate(true).OnComplete(() => target.interactable = target.blocksRaycasts = true); ;
    }

    public void ShowPlayerHUD() => ShowCanvasGroup(playerHudCG);
    public void ShowUpgradeHUD() => ShowCanvasGroup(upgradeHudCG);
    public void ShowGameOverHUD() => ShowCanvasGroup(gameOverCG);
    public void ShowSnorbleHUD() => ShowCanvasGroup(snorblesHudCG);
    public void ShowBreakHUD() => ShowCanvasGroup(breakHudCG);

    /* **************************************************************************** */
    /* Panic Walker System -------------------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Updates the total and new-per-wave display for PanicWalkers.
    /// </summary>
    /// <param name="total">Total number of walkers</param>
    /// <param name="waveAdd">New walkers spawned this wave</param>
    public void UpdateWalkerCountUI(int total) {
        walkerText.text = $"Snorbles: {total}";
    }

    /// <summary>
    /// Spawns a temporary death message in the UI.
    /// </summary>
    public void ShowWalkerDeathMessage(string message) {
        WalkerDeathMessage instance = Instantiate(walkerDeathMessagePrefab, walkerMessageParent);
        instance.Init(message);
    }

    /* **************************************************************************** */
    /* GAME OVER ------------------------------------------------------------------ */
    /* **************************************************************************** */
    public void ShowGameOverNarrative(int score, int rank, string narrative) {
        ShowGameOverHUD();

        string hunterName = PlayerPrefs.GetString("PlayerName", "Unnamed Hunter");
        hunterNameText.text = hunterName;

        if (rank > 0)
            hunterScoreText.text = $"Score: {score}   |   Rank: {rank}";
        else
            hunterScoreText.text = $"Score: {score}   |   Rank: Unranked";

        narrativeText.text = $"\"{narrative}\"";
    }

    /* **************************************************************************** */
    /* GENERAL -------------------------------------------------------------------- */
    /* **************************************************************************** */
    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    public void RetryGame() {
        DOTween.KillAll();
        AudioManager.Instance.PlaySFX(SoundIndexKey.openSnorblesUI);
        AudioManager.Instance.SwitchScene(SceneKey.Game);
    }

    public void ReturnToMainMenu() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DOTween.KillAll();

        AudioManager.Instance.PlaySFX(SoundIndexKey.openSnorblesUI);
        AudioManager.Instance.SwitchScene(SceneKey.MainMenu);     
    }

    public void OpenBreakHudFromBtn() {
        gameManager.OpenBreakHUD();
    }

    public void ResumeGameFromBreakHUD() {
        StartCoroutine(DelayedResumeRequest());
    }

    private IEnumerator DelayedResumeRequest() {
        //AudioManager.Instance.PlaySFX(SoundIndexKey.openSnorblesUI);
        ShowPlayerHUD();

        yield return new WaitForSecondsRealtime(1.5f);
        gameManager.ResumeFromBreakHUD(); 
    }

    /// <summary>
    /// Displays a temporary player message with fade in/out.
    /// </summary>
    public void ShowPlayerMessage(string message) {
        playerMessageText.text = message;
        playerMessageText.alpha = 1f;

        // fade in
        playerMessageText.DOFade(1f, 0.5f).SetDelay(0.5f).OnComplete(() => {

            // fade out
            playerMessageText.DOFade(0f, 0.3f).SetDelay(messageDisplayTime - 0.8f).SetEase(Ease.InOutQuad);
        });
    }


}
