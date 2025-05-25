using UnityEngine;

public class HuntRankSystem : MonoBehaviour {
    public int CurrentRank { get; private set; } = 1;
    public int CurrentXP { get; private set; } = 0;

    [SerializeField] private int xpToNextRank = 12;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;

    private void Start() {
        uiManager.SetHuntRankDisplay(1, 0);
    }

    public void AddXP() {
        float combo = Mathf.Max(1f, gameManager.comboMultiplier*0.5f);
        float rawXP = combo; 

        if (Random.value < (gameManager.upgradeManager.doubleHuntRank)) {
            rawXP += combo;
            Debug.Log("Double HuntRank XP triggered!");
        }

        int gainedXP = Mathf.CeilToInt(rawXP);
        CurrentXP += gainedXP;

        CheckRankUp();
        uiManager.SetHuntRankDisplay(CurrentRank, GetProgress01(), gainedXP > combo);
    }

    private void CheckRankUp() {
        while (CurrentXP >= xpToNextRank) {
            CurrentXP -= xpToNextRank;
            CurrentRank++;
            xpToNextRank = Mathf.RoundToInt(12 * Mathf.Pow(CurrentRank, 2f));
            AudioManager.Instance.PlaySFX(RetroHunter2.SoundIndexKey.levelUp);
        }
    }

    public float GetProgress01() {
        return (float)CurrentXP / xpToNextRank;
    }

    public int GetUpgradeCount() {
        return CurrentRank;
    }

    public void ResetRankAfterUpgradePhase() {
        CurrentRank = 1;
        CurrentXP = 0;
        xpToNextRank = 12; 
        uiManager.SetHuntRankDisplay(CurrentRank, 0, false);
    }
}