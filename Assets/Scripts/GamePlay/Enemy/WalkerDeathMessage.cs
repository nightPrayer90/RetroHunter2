using TMPro;
using UnityEngine;

public class WalkerDeathMessage : MonoBehaviour {
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float lifetime = 2.5f;

    public void Init(string message) {
        messageText.text = message;
        Destroy(gameObject, lifetime);
    }
}