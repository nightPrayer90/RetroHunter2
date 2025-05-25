using UnityEngine;
using TMPro;

public class TypewriterText : MonoBehaviour {
    [SerializeField] private TMP_Text textComponent;

    [Header("Timing (one of both is required)")]
    [SerializeField] private float totalDuration = 5f; 
    [SerializeField] private bool useUnscaledTime = true;

    private string fullText;
    private float charDelay;
    private float timer;
    private int currentIndex;

    private void OnEnable() {
        if (textComponent == null) return;

        fullText = textComponent.text;
        textComponent.text = "";
        timer = 0f;
        currentIndex = 0;

        int totalChars = fullText.Length;
        charDelay = (totalChars > 0) ? totalDuration / totalChars : 0.05f;
    }

    private void Update() {
        if (currentIndex >= fullText.Length) return;

        timer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        while (timer >= charDelay && currentIndex < fullText.Length) {
            timer -= charDelay;
            currentIndex++;
            textComponent.text = fullText.Substring(0, currentIndex);
        }
    }
}