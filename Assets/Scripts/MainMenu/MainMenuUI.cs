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
}