/* **************************************************************************** */
/* GAME MANAGER: CONTROLS FLOW, SCORE, COMBO, WALKERS                          */
/* **************************************************************************** */

using RetroHunter2;
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the overall game flow: timer, waves, score, combo system and PanicWalkers.
/// </summary>
public class GameManager : MonoBehaviour {

    /* **************************************************************************** */
    /* VARIABLES ------------------------------------------------------------------ */
    /* **************************************************************************** */

    [Header("Wave")]
    public int currentWave = 1;
    public GameState gameState;
    [SerializeField] private SpawnManager spawnManager;

    [Header("Score")]
    public int totalScore = 0;

    [Header("Timer")]
    public float waveTimeRemaining = 60f;
    [SerializeField] private float waveDuration = 60f;

    [Header("GameObjects")]
    public CameraController cameraController;
    public UIManager uiManager;
    public PlayerController playerController;
    [SerializeField] private PostProcessingManager ppManager;

    [Header("Floating Text")]
    [SerializeField] private FloatingText floatingTextPrefab;
    [SerializeField] private GameObject floatingTextSimplePrefab;

    [Header("Combo")]
    public float comboMultiplier = 1f;
    private float comboFill = 0f;
    [SerializeField] private float comboFillPerHit = 0.25f;
    [SerializeField] private float comboDrainRate = 0.1f;

    [Header("HuntRankSystem")]
    [SerializeField] private HuntRankSystem huntRankSystem;
    public UpgradeManager upgradeManager;

    [Header("PanicWalkerSystem")]
    public PanicWalkerMessagePool messagePool;
    [SerializeField] private PanicWalkerSpawner walkerSpawner;
    [SerializeField] private int initialWalkerCount = 20;
    private int totalPanicWalkers = 0;


    /* **************************************************************************** */
    /* UNITY METHODS -------------------------------------------------------------- */
    /* **************************************************************************** */

    private void Start() {
        uiManager.ShowPlayerHUD();
        
        // Spawn first panic walkers
        walkerSpawner.SpawnWalkers(initialWalkerCount);
        totalPanicWalkers = initialWalkerCount;
        uiManager.UpdateWalkerCountUI(totalPanicWalkers);
        ppManager.UpdateVignetteIntensity(totalPanicWalkers);

        // Spawn first enemies
        spawnManager.StartNextWave();
        StartCoroutine(BeginWave());

        AudioManager.Instance.PlayMusicWithoutRestart("HunterParadies");
    }

    private void Update() {
        if (gameState != GameState.play) return;


        waveTimeRemaining -= Time.deltaTime;
        uiManager.UpdateWaveTimeDisplay();

        if (waveTimeRemaining <= 0f) {
            waveTimeRemaining = 0f;
            OpenUpgradeMenu();

            ResetCombo();
            uiManager.SetComboVisual(0f, 1f, false);
        }
        else {
            UpdateComboState();
        }

        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (gameState == GameState.play) {
                OpenBreakHUD();
            }
        }

    }


    /* **************************************************************************** */
    /* BREAK HUD ------------------------------------------------------------------ */
    /* **************************************************************************** */

    public void OpenBreakHUD() {
        Time.timeScale = 0f;
        gameState = GameState.breakHud;
        AudioManager.Instance.PlaySFX(SoundIndexKey.openSnorblesUI);
        uiManager.ShowBreakHUD();
    }

    public void ResumeFromBreakHUD() {
        AudioManager.Instance.PlaySFX(SoundIndexKey.getScoreGoal);
        Time.timeScale = 1f;
        gameState = GameState.play;
    }

    /* **************************************************************************** */
    /* COMBO SYSTEM --------------------------------------------------------------- */
    /* **************************************************************************** */

    /// <summary>
    /// Updates combo drain, thresholds and UI.
    /// </summary>
    private void UpdateComboState() {
        if (comboMultiplier > 1f || comboFill > 0f) {
            bool updateMultiplier = false;
            float drain = comboDrainRate * comboMultiplier * upgradeManager.comboDrainMult * Time.deltaTime;
            comboFill -= drain;

            if (comboFill <= 0f) {
                comboFill = 0f;
                ResetCombo();
            }
            else if (comboFill >= 1f) {
                comboMultiplier += 1f;
                updateMultiplier = true;
                comboFill = 0.5f;
                cameraController.Shake(0.25f, 0.3f + comboMultiplier * 0.1f);
            }

            uiManager.SetComboVisual(comboFill, comboMultiplier, updateMultiplier);
        }
    }

    /// <summary>
    /// Called when a successful hit occurs. Fills combo bar.
    /// </summary>
    public void ComboHit() {
        float bonus = upgradeManager.comboFillBonus;
        comboFill += comboFillPerHit + bonus;
    }

    /// <summary>
    /// Resets combo to base value and optionally triggers fail sound.
    /// </summary>
    public void ResetCombo() {
        if (comboMultiplier > 1f) {
            AudioManager.Instance.PlaySFX(SoundIndexKey.multiplierFail);
        }

        comboMultiplier = 1f;
    }

    /// <summary>
    /// Increases combo multiplier by given value.
    /// </summary>
    public void IncreaseCombo(float amount) {
        comboMultiplier += amount;
    }

    /* **************************************************************************** */
    /* SCORE SYSTEM ---------------------------------------------------------------- */
    /* **************************************************************************** */

    /// <summary>
    /// Adds score with combo and upgrade multipliers.
    /// </summary>
    public void AddScore(int basePoints, Vector3 worldPos) {
        float mult = comboMultiplier * upgradeManager.scoreBonusMult;
        int finalScore = Mathf.RoundToInt(basePoints * mult);

        bool isDouble = Random.value < (upgradeManager.doubleScore);

        if (isDouble) {
            finalScore *= 2;
            AudioManager.Instance.PlaySFX(SoundIndexKey.doublePointsGet);
            Debug.Log("Double score triggered!");
        }
        totalScore += finalScore;

        uiManager.UpdateScoreDisplay();
        ShowFloatingText(finalScore, worldPos, 1.2f, isDouble);
    }

    /// <summary>
    /// Spawns a floating score text in the world.
    /// </summary>
    public void ShowFloatingText(int displayScore, Vector3 worldPos, float duration, bool isDouble = false) {
        FloatingText textInstance = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity);
        Color color = uiManager.GetCurrentComboColor();
        textInstance.Init(displayScore, worldPos, duration, color, comboMultiplier, isDouble);
    }

    /// <summary>
    /// Spawns a simple floating message text in the world.
    /// </summary>
    public void ShowSimpleFloatingText(string message, Vector3 worldPos, Color color) {
        if (floatingTextSimplePrefab == null) return;

        GameObject obj = Instantiate(floatingTextSimplePrefab, worldPos, Quaternion.identity);
        FloatingTextSimple floating = obj.GetComponent<FloatingTextSimple>();
        floating.Init(message, worldPos, color);
    }

    /* **************************************************************************** */
    /* WAVE SYSTEM ----------------------------------------------------------------- */
    /* **************************************************************************** */

    /// <summary>
    /// Starts a new wave and resets all relevant values.
    /// </summary>
    public void StartNextWave() {
        spawnManager.StartNextWave();
        playerController.InstantReload();
        currentWave++;
        waveTimeRemaining = waveDuration + upgradeManager.waveTimeBonus;
        uiManager.UpdateWaveTimeDisplay();
        uiManager.ShowPlayerHUD();
        StartCoroutine(BeginWave());
    }

    /// <summary>
    /// Starts the active wave by setting gameState to play and unpausing time.
    /// </summary>
    private IEnumerator BeginWave() {
        Time.timeScale = 1;
        gameState = GameState.cinematic;
        cameraController.PlayWaveIntro();


        string introMessage = messagePool.GetRandomIntroMessage();
        uiManager.ShowPlayerMessage(introMessage);

        yield return new WaitForSecondsRealtime(3.5f);

        gameState = GameState.play;
    }

    private void OpenUpgradeMenu() {
        Time.timeScale = 0;
        uiManager.ShowUpgradeHUD();
        upgradeManager.StartUpgradeSequence();
        gameState = GameState.upgradeScreen;
        AudioManager.Instance.PlaySFX(SoundIndexKey.openShopSound);
    }

    /* **************************************************************************** */
    /* GAME OVER ------------------------------------------------------------------ */
    /* **************************************************************************** */

    private void TriggerGameOver() {
        gameState = GameState.gameOver;
        ResetCombo();
        Time.timeScale = 0;
        uiManager.ShowGameOverHUD();
        LootLockerManager.Instance.SubmitScore(totalScore, OnScoreSubmitted);
    }

    private void OnScoreSubmitted(bool success, int rankPosition) {
        string message;

        if (!success || rankPosition <= 0) {
            message = messagePool.GetNoRankMessage();
            rankPosition = -1;
        }
        else if (rankPosition <= 10) {
            message = messagePool.GetTop10Message();
        }
        else if (rankPosition <= 50) {
            message = messagePool.GetRank11to50Message();
        }
        else {
            message = messagePool.GetRank51PlusMessage();
        }

        uiManager.ShowGameOverNarrative(totalScore, rankPosition, message);
    }

    /* **************************************************************************** */
    /* PANIC WALKERS -------------------------------------------------------------- */
    /* **************************************************************************** */

    /// <summary>
    /// Adjusts the number of living PanicWalkers. Triggers Game Over at 0. -> only if a walker die and at beginning
    /// </summary>
    public void AdjustWalkerCount(int delta) {
        totalPanicWalkers += delta;
        totalPanicWalkers = Mathf.Max(0, totalPanicWalkers);
        ppManager.FlashAndSetVignette(totalPanicWalkers);


        if (totalPanicWalkers == 0) {
            TriggerGameOver();
        }
        else {
            uiManager.UpdateWalkerCountUI(totalPanicWalkers);
        }
    }

    public void OpenSnorbleSelection() {
        gameState = GameState.snorbleScreen;
        uiManager.ShowSnorbleHUD();
        AudioManager.Instance.PlaySFX(SoundIndexKey.openSnorblesUI);
    }

    public void OnSnorbleChosen(SnorbleType chosenType, int spawnAreaIndex) {
        gameState = GameState.play;
        Time.timeScale = 1f;

        walkerSpawner.SpawnSnorble(chosenType, spawnAreaIndex);

        totalPanicWalkers++;

        ppManager.UpdateVignetteIntensity(totalPanicWalkers);
        uiManager.UpdateWalkerCountUI(totalPanicWalkers);
        StartNextWave();
    }

    /* **************************************************************************** */
    /* AMMO ------------------------------------------------------------------------ */
    /* **************************************************************************** */

    /// <summary>
    /// Initializes the player's ammo bar.
    /// </summary>
    public void InitAmmoBar(int maxAmmo) {
        uiManager.InitializeAmmoBar(maxAmmo);
    }

    /// <summary>
    /// Updates the player's ammo bar display.
    /// </summary>
    public void UpdateAmmoBar(int currentAmmo) {
        uiManager.UpdateAmmoBar(currentAmmo);
    }

    public void UpdateAmmoBarAfterShooting(int currentAmmo) {
        uiManager.UpdateAmmoBarAfterShooting(currentAmmo);
    }
}