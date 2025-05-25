using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

namespace RetroHunter2 {
    public class TransitionManager : MonoBehaviour {
        [SerializeField] private CanvasGroup transitionCanvasGroup;

        private bool isTransitioning = false;

        private void Awake() {
            transitionCanvasGroup.alpha = 0f;
            transitionCanvasGroup.gameObject.SetActive(false);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void StartSceneTransition(SceneKey sceneKey) {
            if (!isTransitioning) {
                StartCoroutine(FadeAndSwitch(sceneKey));
            }
        }

        private IEnumerator FadeAndSwitch(SceneKey key) {
            isTransitioning = true;
            transitionCanvasGroup.gameObject.SetActive(true);

            yield return transitionCanvasGroup.DOFade(1f, 0.5f).SetUpdate(true).WaitForCompletion();

            string sceneName = key switch {
                SceneKey.MainMenu => "MainMenu",
                SceneKey.Game => "GameScene",
                SceneKey.StoryWindow => "StoryScene",
                _ => throw new System.Exception("SceneKey not handled: " + key)
            };
            SceneManager.LoadScene(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            StartCoroutine(FadeOutAfterLoad());
        }

        private IEnumerator FadeOutAfterLoad() {
            yield return null;

            yield return new WaitForSecondsRealtime(0.05f);

            yield return transitionCanvasGroup.DOFade(0f, 0.5f).SetUpdate(true).WaitForCompletion();

            transitionCanvasGroup.gameObject.SetActive(false);
            isTransitioning = false;
        }

        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}