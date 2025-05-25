using LootLocker.Requests;
using UnityEngine;
using System.Collections;

public class LootLockerManager : MonoBehaviour {
    public static LootLockerManager Instance;

    private const string LEADERBOARD_ID = "retro_scoreboard";

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        string playerName = PlayerPrefs.GetString("PlayerName", "Unnamed");

        LootLockerSDKManager.StartGuestSession(playerName, response => {
            if (response.success) {
                Debug.Log("Guest session started");
                StartCoroutine(RefreshScoreboardNextFrame());
            }
            else {
                Debug.LogError("Guest session failed: " + response.errorData.message);
            }
        });
    }

    /*public void SubmitScore(int score, System.Action<bool> onComplete = null) {
        string playerName = PlayerPrefs.GetString("PlayerName", "Unnamed");

        LootLockerSDKManager.SubmitScore(playerName, score, LEADERBOARD_ID, response => {
            if (response.success) {
                Debug.Log("Score submitted to generic leaderboard!");
            }
            else {
                Debug.LogError("Score submission failed: " + response.errorData.message);
            }

            onComplete?.Invoke(response.success);
        });
    }*/

    public void SubmitScore(int score, System.Action<bool, int> onComplete = null) {
        string playerName = PlayerPrefs.GetString("PlayerName", "Unnamed");

        LootLockerSDKManager.SubmitScore(playerName, score, LEADERBOARD_ID, response => {
            if (response.success) {
                Debug.Log("Score submitted to generic leaderboard!");
                int rank = response.rank;
                onComplete?.Invoke(true, rank);
            }
            else {
                Debug.LogError("Score submission failed: " + response.errorData.message);
                onComplete?.Invoke(false, -1);
            }
        });
    }

    private IEnumerator RefreshScoreboardNextFrame() {
        yield return null;

        MainMenuScoreboard scoreboard = Object.FindFirstObjectByType<MainMenuScoreboard>();
        if (scoreboard != null) {
            scoreboard.LoadScores();
            Debug.Log("Scoreboard refreshed after login.");
        }
    }
}

