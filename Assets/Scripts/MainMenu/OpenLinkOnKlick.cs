using UnityEngine;

public class OpenLinkOnClick : MonoBehaviour {
    [SerializeField] private string url = "https://example.com";

    /// <summary>
    /// Opens the specified URL in the default browser.
    /// </summary>
    public void OpenURL() {
        Application.OpenURL(url);
    }
}