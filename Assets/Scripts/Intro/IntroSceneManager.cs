using UnityEngine;
using RetroHunter2;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class IntroSceneManager : MonoBehaviour {
    [SerializeField] private float duration = 3.0f;
    [SerializeField] private SceneKey sceneKey = SceneKey.MainMenu;

    [SerializeField] private PlayableDirector director;

    private float timer;
    private bool skipped = false;

    private void Start() {
        AudioManager.Instance.FadeOutCurrentMusic(0.2f);
        timer = 0f;
        director.Play();
    }

    private void Update() {
        timer += Time.deltaTime;

        if (Keyboard.current.anyKey.wasPressedThisFrame) {
            skipped = true;
            LoadMenu();
        }

        if (timer >= duration && !skipped) {
            skipped = true;
            LoadMenu();
        }
    }

    private void LoadMenu() {

        AudioManager.Instance.SwitchScene(sceneKey);
    }
}