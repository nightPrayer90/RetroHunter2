using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RetroHunter2;

public class UpgradeStatusUI : MonoBehaviour {
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Value Texts")]
    [SerializeField] private TMP_Text reloadSpeedText;
    [SerializeField] private TMP_Text maxAmmoText;
    [SerializeField] private TMP_Text noAmmoChanceText;
    [SerializeField] private TMP_Text comboFillText;
    [SerializeField] private TMP_Text comboDrainText;
    [SerializeField] private TMP_Text waveTimeText;
    [SerializeField] private TMP_Text spawnRateText;
    [SerializeField] private TMP_Text scoreBonusText;
    [SerializeField] private TMP_Text doubleScoreText;
    [SerializeField] private TMP_Text doubleRankText;

    [Header("Upgrade Info Text")]
    [SerializeField] private TMP_Text upgradeInfoText;

    private Dictionary<UpgradeType, TMP_Text> typeToText;

    private Dictionary<UpgradeType, string> labelMap = new() {
    { UpgradeType.ReloadSpeed, "Reload Speed" },
    { UpgradeType.MaxAmmo, "Max Ammo" },
    { UpgradeType.NoAmmoChance, "Free Ammo" },
    { UpgradeType.ComboFill, "Combo Fill Bonus" },
    { UpgradeType.ComboDrain, "Combo Drain" },
    { UpgradeType.WaveTime, "Wave Time" },
    { UpgradeType.EnemySpawnRate, "Spawn Rate" },
    { UpgradeType.ScoreBonus, "Score Bonus" },
    { UpgradeType.DoubleScoreChance, "Score x2" },
    { UpgradeType.DoubleExpChance, "Rank exp x2" },
    };

    public string AsPercent(float value) => $"{value * 100f:0}%";
    public string AsPercentBonus(float bonus) => $"+{bonus * 100f:0}%";

    private void Awake() {
        typeToText = new() {
            { UpgradeType.ReloadSpeed, reloadSpeedText },
            { UpgradeType.MaxAmmo, maxAmmoText },
            { UpgradeType.NoAmmoChance, noAmmoChanceText },
            { UpgradeType.ComboFill, comboFillText },
            { UpgradeType.ComboDrain, comboDrainText },
            { UpgradeType.WaveTime, waveTimeText },
            { UpgradeType.EnemySpawnRate, spawnRateText },
            { UpgradeType.ScoreBonus, scoreBonusText },
            { UpgradeType.DoubleScoreChance, doubleScoreText },
            { UpgradeType.DoubleExpChance, doubleRankText },
        };
    }

    public void RefreshDisplay() {
        float reloadTime = upgradeManager.player.GetReloadTime();
        reloadSpeedText.text = $"{labelMap[UpgradeType.ReloadSpeed]}: {reloadTime:0.00}s";

        maxAmmoText.text = $"{labelMap[UpgradeType.MaxAmmo]}: +{upgradeManager.maxAmmoBonus}";

        noAmmoChanceText.text = $"{labelMap[UpgradeType.NoAmmoChance]}: {AsPercent(upgradeManager.noAmmoUseChance)}";

        comboFillText.text = $"{labelMap[UpgradeType.ComboFill]}: {AsPercentBonus(upgradeManager.comboFillBonus)}";

        float drainReduction = 1f - upgradeManager.comboDrainMult;
        comboDrainText.text = $"{labelMap[UpgradeType.ComboDrain]}: -{drainReduction * 100f:0}%";

        waveTimeText.text = $"{labelMap[UpgradeType.WaveTime]}: +{upgradeManager.waveTimeBonus:0.#}s";

        spawnRateText.text = $"{labelMap[UpgradeType.EnemySpawnRate]}: {AsPercent(upgradeManager.enemySpawnRateMult)}";
        scoreBonusText.text = $"{labelMap[UpgradeType.ScoreBonus]}: {AsPercentBonus(upgradeManager.scoreBonusMult)}";

        doubleScoreText.text = $"{labelMap[UpgradeType.DoubleScoreChance]}: {AsPercent(upgradeManager.doubleScore)}";
        doubleRankText.text = $"{labelMap[UpgradeType.DoubleExpChance]}: {AsPercent(upgradeManager.doubleHuntRank)}";
    }

    public void Highlight(UpgradeType type) {
        ResetHighlighting();

        if (typeToText.TryGetValue(type, out TMP_Text t) && labelMap.TryGetValue(type, out string label)) {
            t.text = $"{label}: {upgradeManager.GetPreviewValue(type)}";
            ColorUtility.TryParseHtmlString("#118C06", out var hexColor);
            t.color = hexColor;
        }
    }

    public void ResetHighlighting() {
        RefreshDisplay();
        ColorUtility.TryParseHtmlString("#FFFBD6", out var readableColor);

        foreach (var t in typeToText.Values) {
            t.color = readableColor;
        }
    }

    public void SetUpgradeInfo(int upgradesRemaining) {
        string coloredPoints = $"<color=#226622>{upgradesRemaining}</color>";
        string plural = upgradesRemaining == 1 ? "point" : "points";

        upgradeInfoText.text =
            $"Your hunt was successful! You earned {coloredPoints} Hunter Rank {plural}.\n" +
            $"Exchange them for upgrades.";
    }
}