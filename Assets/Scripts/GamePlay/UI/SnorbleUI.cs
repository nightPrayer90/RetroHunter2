using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RetroHunter2;
using System.Linq;

public class SnorbleUI : MonoBehaviour {
    [Header("UI Elements")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private ToggleGroup islandToggleGroup;
    

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Snorble Images")]
    [SerializeField] private Image kelvinImage;
    [SerializeField] private Sprite kelvinIdleSprite;
    [SerializeField] private Sprite kelvinHoverSprite;

    [SerializeField] private Image albertImage;
    [SerializeField] private Sprite albertIdleSprite;
    [SerializeField] private Sprite albertHoverSprite;

    private const string kelvinDescription = "<b>Kelvins don’t care. About anything.</b>\nThey’ve got three lives, which is great-because they rarely use their brains. Think of them as lazy turrets with legs: sometimes they shoot, sometimes they nap. Perfect for players who like to survive without trying too hard.";

    private const string albertDescription = "<b>Alberts are pure chaos in a fluffy package.</b>\nThey’re small, fast, and constantly jittering like they’ve had too much leaf juice. With only two lives, they’re not exactly built to last - but sometimes they get so excited they toss their cookie in the air. Shoot it, and you’ll get energy and extra wave time.";

    /// <summary>
    /// Called by UI button for Kelvin.
    /// </summary>
    public void SelectKelvin() {
        int areaIndex = GetSelectedIslandIndex(); 
        AudioManager.Instance.PlaySFX(SoundIndexKey.kelvinSelect);
        gameManager.OnSnorbleChosen(SnorbleType.Kelvin, areaIndex);
    }

    /// <summary>
    /// Called by UI button for Albert.
    /// </summary>
    public void SelectAlbert() {
        AudioManager.Instance.PlaySFX(SoundIndexKey.albertSelect);
        int areaIndex = GetSelectedIslandIndex(); 
        gameManager.OnSnorbleChosen(SnorbleType.Albert, areaIndex);
    }

    public void SelectToggle() {
        AudioManager.Instance.PlaySFXWithRandomPitch(SoundIndexKey.islandToggle, 0.7f, 0.2f);
    }

    /// <summary>
    /// Called via EventTrigger on pointer enter.
    /// </summary>
    public void OnHoverKelvin() {
        AudioManager.Instance.PlaySFX(SoundIndexKey.MouseHover);
        descriptionText.text = kelvinDescription;

        if (kelvinImage != null && kelvinHoverSprite != null)
            kelvinImage.sprite = kelvinHoverSprite;
    }

    /// <summary>
    /// Called via EventTrigger on pointer enter.
    /// </summary>
    public void OnHoverAlbert() {
        AudioManager.Instance.PlaySFX(SoundIndexKey.MouseHover);
        descriptionText.text = albertDescription;

        if (albertImage != null && albertHoverSprite != null)
            albertImage.sprite = albertHoverSprite;
    }

    /// <summary>
    /// Called via EventTrigger on pointer exit.
    /// </summary>
    public void OnHoverExit() {
        descriptionText.text = "";

        if (kelvinImage != null && kelvinIdleSprite != null)
            kelvinImage.sprite = kelvinIdleSprite;

        if (albertImage != null && albertIdleSprite != null)
            albertImage.sprite = albertIdleSprite;
    }

    /// <summary>
    /// Determines which island is selected. Returns -1 if "Random" is selected.
    /// </summary>
    private int GetSelectedIslandIndex() {
        Toggle active = islandToggleGroup.ActiveToggles().FirstOrDefault();
        if (active == null) return Random.Range(0, 3);

        string name = active.name.ToLower();
        if (name.Contains("0")) return 0;
        if (name.Contains("1")) return 1;
        if (name.Contains("2")) return 2;

        return Random.Range(0, 3);
    }

}