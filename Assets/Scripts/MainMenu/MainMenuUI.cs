using UnityEngine;
using TMPro;
using RetroHunter2;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private RectTransform missionLogPanel;
    [SerializeField] private RectTransform scoreboardPanel;
    [SerializeField] private float tweenDuration = 1.0f;

    [SerializeField] private CanvasGroup creditsGroup;
    [SerializeField] private float creditsFadeDuration = 0.1f;

    [SerializeField] private UnityEngine.UI.Toggle fullscreenToggle;
    [SerializeField] private UnityEngine.UI.Toggle vSyncToggle;

    private const string PREF_FULLSCREEN = "FullscreenEnabled";
    private const string PREF_VSYNC = "VSyncEnabled";

    private MainMenuState currentState = MainMenuState.Menu;

    private void Start() {
        Time.timeScale = 1;

        creditsGroup.alpha = 0;
        creditsGroup.blocksRaycasts = false;
        creditsGroup.interactable = false;
        currentState = MainMenuState.Menu;

        string storedName = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrWhiteSpace(storedName)) {
            storedName = GenerateFallbackName();
        }
        nameInputField.text = storedName;

        // Load and apply preferences
        bool fullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;
        bool vSync = PlayerPrefs.GetInt(PREF_VSYNC, 1) == 1;

        Screen.fullScreen = fullscreen;
        QualitySettings.vSyncCount = vSync ? 1 : 0;

        // Update toggles
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = fullscreen;

        if (vSyncToggle != null)
            vSyncToggle.isOn = vSync;


        AnimatePanels();
    }

    private void AnimatePanels() {
        Vector2 leftOffscreen = new Vector2(-1000, missionLogPanel.anchoredPosition.y);
        Vector2 rightOffscreen = new Vector2(600, scoreboardPanel.anchoredPosition.y);

        missionLogPanel.anchoredPosition = leftOffscreen;
        scoreboardPanel.anchoredPosition = rightOffscreen;

        missionLogPanel.DOAnchorPosX(0, tweenDuration).SetEase(Ease.OutBack).SetDelay(0.2f)
            .OnStart(() => {
                AudioManager.Instance.PlaySFX(SoundIndexKey.paperSound);
                AudioManager.Instance.PlayMusicWithoutRestart("MainMenu");
            });
        scoreboardPanel.DOAnchorPosX(0, tweenDuration).SetEase(Ease.OutBack).SetDelay(0.5f)
            .OnStart(() => AudioManager.Instance.PlaySFX(SoundIndexKey.paperSound));
    }

    public void StartGame() {
        string playerName = nameInputField.text;

        AudioManager.Instance.FadeOutCurrentMusic(0.5f);

        if (string.IsNullOrWhiteSpace(playerName)) {
            playerName = GenerateFallbackName();
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        //LootLockerManager.Instance.SetPlayerNameFromPrefs();
        Debug.Log("StartGame");
        AudioManager.Instance.SwitchScene(SceneKey.StoryWindow);
    }

    private string GenerateFallbackName() {
        string[] baseNames = new[] {
            "Hunter", "Sharpshot", "Slugbrain", "Botbuster",
            "RogueAI", "Huntress", "PixelRanger", "Laserhawk",
            "GlitchScout", "ByteStorm", "ZapFang", "DroidSniper"
        };

        string baseName = baseNames[Random.Range(0, baseNames.Length)];
        int number = Random.Range(1000, 9999);

        return $"{baseName}{number}";
    }

    public void OpenCredits() {
        if (currentState == MainMenuState.Credits) return;

        currentState = MainMenuState.Credits;
        creditsGroup.blocksRaycasts = true;
        creditsGroup.interactable = true;
        creditsGroup.DOFade(1f, creditsFadeDuration);
    }

    public void CloseCredits() {
        if (currentState != MainMenuState.Credits) return;
        AudioManager.Instance.PlaySFX(SoundIndexKey.pointerKlick);

        currentState = MainMenuState.Menu;
        creditsGroup.blocksRaycasts = false;
        creditsGroup.interactable = false;
        creditsGroup.DOFade(0f, creditsFadeDuration);
    }

    // <summary>
    /// Toggles fullscreen mode on or off.
    /// </summary>
    public void ToggleFullscreen(bool isFullscreen) {
        AudioManager.Instance.PlaySFX(SoundIndexKey.KelvinShooting);
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(PREF_FULLSCREEN, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Toggles V-Sync on or off.
    /// </summary>

    public void ToggleVSync(bool isEnabled) {
        AudioManager.Instance.PlaySFX(SoundIndexKey.KelvinShooting);
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt(PREF_VSYNC, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Quits the game. Works in builds, ignored in editor.
    /// </summary>
    public void QuitGame() {
        Debug.Log("Quit requested.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}