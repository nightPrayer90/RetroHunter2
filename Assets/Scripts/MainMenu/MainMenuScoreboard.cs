using UnityEngine;
using LootLocker.Requests;
using System.Text;
using TMPro;

public class MainMenuScoreboard : MonoBehaviour {
    [SerializeField] private Transform scoreboardContainer;
    [SerializeField] private GameObject scoreEntryPrefab;
    [SerializeField] private int maxEntries = 15;
    [SerializeField] private string leaderboardID = "30948";
    [SerializeField] private Color playerColor;

    private void Start() {
        LoadScores();
    }

    public void LoadScores() {
        LootLockerSDKManager.GetScoreList(leaderboardID, maxEntries, 0, response => {
            if (!response.success) {
                Debug.LogError("Failed to load leaderboard.");
                return;
            }

            string currentPlayer = PlayerPrefs.GetString("PlayerName", "Unnamed");

            foreach (Transform child in scoreboardContainer) {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < response.items.Length; i++) {
                var entry = response.items[i];

                string member = entry.member_id;
                int score = entry.score;
                bool isPlayer = member == currentPlayer;

                GameObject entryGO = Instantiate(scoreEntryPrefab, scoreboardContainer);

                TMP_Text rankText = entryGO.transform.Find("RankText").GetComponent<TMP_Text>();
                TMP_Text nameText = entryGO.transform.Find("NameText").GetComponent<TMP_Text>();
                TMP_Text scoreText = entryGO.transform.Find("ScoreText").GetComponent<TMP_Text>();

                rankText.text = (i + 1).ToString();
                nameText.text = member;
                scoreText.text = score.ToString();

                if (isPlayer) {
                    rankText.color = playerColor;
                    nameText.color = playerColor;
                    scoreText.color = playerColor;
                }
            }
        });
    }
}