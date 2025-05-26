using UnityEngine;
using System.Collections.Generic;
using RetroHunter2;
using DG.Tweening;

/// <summary>
/// Stores and applies all active upgrades. Other systems read values from here.
/// </summary>
public class UpgradeManager : MonoBehaviour {
    [Header("Upgrade Pool")]
    [SerializeField] private List<UpgradeData> upgradePool;
    private List<UpgradePanel> activePanels = new();
    private List<UpgradeData> lastRoundUpgrades = new();
    private List<UpgradeData> secondLastRoundUpgrades = new();

    [Header("UI")]
    [SerializeField] private GameObject upgradePanelPrefab;
    [SerializeField] private Transform panelContainer;
    [SerializeField] private UpgradeStatusUI statusUI;

    [Header("References")]
    public PlayerController player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HuntRankSystem huntRankSystem;

    private int upgradesPending = 0;

    [Header("Combat Upgrades")]
    public float reloadSpeedMult = 1.0f;
    public int maxAmmoBonus = 0;
    public float noAmmoUseChance = 0.0f;
    public float ricochetChance = 0.0f;

    [Header("Combo Upgrades")]
    public float comboFillBonus = 0.0f;
    public float comboDrainMult = 1.0f;

    [Header("Wave / Score Upgrades")]
    public float waveTimeBonus = 0.0f;
    public float enemySpawnRateMult = 1.0f;
    public float scoreBonusMult = 1.0f;
    public float doubleScore = 0.0f;
    public float doubleHuntRank = 0.0f;


    public void StartUpgradeSequence() {
        upgradesPending = huntRankSystem.GetUpgradeCount();
        ShowNextUpgrade();
    }

    /// <summary>
    /// Shows N random upgrades and spawns UI panels.
    /// </summary>
    private void ShowNextUpgrade() {
        ClearOldPanels();
        statusUI.RefreshDisplay();
        statusUI.ResetHighlighting();
        secondLastRoundUpgrades = new(lastRoundUpgrades);

        if (upgradesPending <= 0) {
            huntRankSystem.ResetRankAfterUpgradePhase();
            gameManager.OpenSnorbleSelection();
            
            return;
        }
        else {
            AudioManager.Instance.PlaySFX(SoundIndexKey.openShopSound);
            statusUI.SetUpgradeInfo(upgradesPending);
        }

        List<UpgradeData> choices = GetRandomUpgrades(3);
        foreach (var upgrade in choices) {
            GameObject panelGO = Instantiate(upgradePanelPrefab, panelContainer);
            UpgradePanel panel = panelGO.GetComponent<UpgradePanel>();

            panel.Setup(upgrade, () => ApplyUpgradeAndContinue(upgrade), statusUI);
            panel.PlayShowEffect(0.05f * activePanels.Count);
            activePanels.Add(panel);
        }

        lastRoundUpgrades.Clear();
        foreach (var panel in activePanels) {
            lastRoundUpgrades.Add(panel.upgradeData);
        }
    }

    private void ApplyUpgradeAndContinue(UpgradeData upgrade) {
        ApplyUpgrade(upgrade);
        statusUI.RefreshDisplay();

        UpgradePanel selectedPanel = activePanels.Find(p => p.upgradeType == upgrade.upgradeType);
        selectedPanel?.PlayHideEffect(0f, () => {

            float delayStep = 0.05f;
            int index = 0;

            foreach (var panel in activePanels) {
                if (panel != selectedPanel) {
                    panel.PlayHideEffect(delayStep * index, () => {
                        Destroy(panel.gameObject);
                    });
                    index++;
                }
            }

            DOVirtual.DelayedCall(0.3f + delayStep * 2f, () => {
                upgradesPending--;
                activePanels.Clear();
                ShowNextUpgrade();
            }).SetUpdate(true);
        });
    }

    /// <summary>
    /// Returns a random selection of upgrades from the pool.
    /// </summary>
    private List<UpgradeData> GetRandomUpgrades(int count) {
        List<UpgradeData> temp = new(upgradePool);

        foreach (var upgrade in lastRoundUpgrades) {
            temp.Remove(upgrade);
        }

        List<UpgradeData> result = new();

        for (int i = 0; i < count && temp.Count > 0; i++) {
            int index = Random.Range(0, temp.Count);
            result.Add(temp[index]);
            temp.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// Applies the effect of a selected upgrade.
    /// </summary>
    public void ApplyUpgrade(UpgradeData upgrade) {
        switch (upgrade.upgradeType) {
            case UpgradeType.ReloadSpeed:
                reloadSpeedMult *= 0.9f;
                break;
            case UpgradeType.MaxAmmo:
                maxAmmoBonus += 1;
                player.IncreaseMaxAmmo(1);
                break;
            case UpgradeType.ComboFill:
                comboFillBonus += 0.05f;
                break;
            case UpgradeType.NoAmmoChance:
                noAmmoUseChance = noAmmoUseChance == 0f ? 0.1f : noAmmoUseChance * 1.1f;
                noAmmoUseChance = Mathf.Min(noAmmoUseChance, 0.95f);
                break;
            case UpgradeType.ComboDrain:
                comboDrainMult *= 0.9f;
                break;
            case UpgradeType.WaveTime:
                waveTimeBonus += 2f;
                break;
            case UpgradeType.EnemySpawnRate:
                enemySpawnRateMult *= 1.05f;
                break;
            case UpgradeType.ScoreBonus:
                scoreBonusMult *= 1.05f;
                break;
            case UpgradeType.DoubleScoreChance:
                doubleScore += 0.05f;
                doubleScore = Mathf.Min(doubleScore, 0.95f);
                break;
            case UpgradeType.DoubleExpChance:
                doubleHuntRank = doubleHuntRank == 0f ? 0.25f : doubleHuntRank * 1.25f;
                doubleHuntRank = Mathf.Min(doubleHuntRank, 0.95f);
                break;
            case UpgradeType.RicochetChance:
                ricochetChance += 0.03f;
                break;
        }
    }

    public string GetPreviewValue(UpgradeType type) {
        switch (type) {
            case UpgradeType.ReloadSpeed:
                float current = player.GetReloadTime();
                float final = current * 0.9f;
                return $"{final:0.00}s";

            case UpgradeType.MaxAmmo:
                int newMaxAmmo = maxAmmoBonus + 1;
                return $"+{newMaxAmmo}";

            case UpgradeType.NoAmmoChance:
                return statusUI.AsPercent(Mathf.Min(noAmmoUseChance == 0f ? 0.1f : noAmmoUseChance * 1.1f, 0.95f));

            case UpgradeType.ComboFill:
                return statusUI.AsPercentBonus(comboFillBonus + 0.05f);

            case UpgradeType.ComboDrain:
                float previewDrainMult = comboDrainMult * 0.9f;
                float reduction = 1f - previewDrainMult;
                return $"-{reduction * 100f:0}%";

            case UpgradeType.WaveTime:
                return $"+{waveTimeBonus + 2f:0.#}s";

            case UpgradeType.EnemySpawnRate:
                return statusUI.AsPercent(enemySpawnRateMult + 0.05f);

            case UpgradeType.ScoreBonus:
                return statusUI.AsPercentBonus(scoreBonusMult + 0.05f);

            case UpgradeType.DoubleScoreChance:
                float nextScore = Mathf.Min(doubleScore + 0.05f, 0.95f);
                return statusUI.AsPercent(nextScore);

            case UpgradeType.DoubleExpChance:
                float nextExp = doubleHuntRank == 0f ? 0.25f : doubleHuntRank * 1.25f;
                return statusUI.AsPercent(Mathf.Min(nextExp, 0.95f));

            case UpgradeType.RicochetChance:
                return statusUI.AsPercent(ricochetChance + 0.03f);

            default:
                return string.Empty;
        }
    }


    /// <summary>
    /// Clears all previously spawned upgrade panels.
    /// </summary>
    private void ClearOldPanels() {
        foreach (Transform child in panelContainer) {
            Destroy(child.gameObject);
        }
    }


}