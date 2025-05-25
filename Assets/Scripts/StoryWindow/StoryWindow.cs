using System.Collections;
using System.Text.RegularExpressions;
using DG.Tweening;
using RetroHunter2;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryWindow : MonoBehaviour, ISubmitHandler, ICancelHandler {
    [Header("Scene Control")]
    [SerializeField] private SceneKey nextScene;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private Image fadePanel;
    [SerializeField] private string musicName;

    [Header("Text Control")]
    [SerializeField] private TMP_Text displayText;
    private string _textToType;
    [SerializeField] private float typingSpeed = 0.1f;
    private bool _isTyping = true;
    [SerializeField] private TMP_Text inputClickText;
    [SerializeField] private TMP_Text skipTutorialText;
    [SerializeField] private string afterTextString = "<< Next >>";
    private bool _isSkipTutorial = false;
    [SerializeField] private GameObject _firstSelectedBtn;

    private void Start() {
        _textToType = displayText.text;
        displayText.text = "";
        StartCoroutine(TypeText());
        //AudioManager.Instance.PlaySFX(SoundIndexKey.unlockSound);
        //AudioManager.Instance.PlayMusicWithoutRestart(musicName);
        Time.timeScale = 1;
        EventSystem.current.SetSelectedGameObject(_firstSelectedBtn);
    }



    public void OnCancel(BaseEventData eventData) {
        if (_isSkipTutorial == true) return;
        ControlInput();
    }

    public void OnSubmit(BaseEventData eventData) {
        if (_isSkipTutorial == true) return;
        ControlInput();
    }

    public void SkipTutorial() {
        if (skipTutorialText != null) {
            skipTutorialText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f);
            _isSkipTutorial = true;

            //AudioManager.Instance.SceneTransition(SceneKey.ShopScene);
        }
    }

    public void ControlInput() {
        if (_isSkipTutorial == true) return;

        inputClickText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f);

        if (_isTyping) {
            StopAllCoroutines();
            displayText.text = RemoveTags(_textToType);
            _isTyping = false;
            inputClickText.text = afterTextString;
            AudioManager.Instance.PlaySFX(SoundIndexKey.MouseKlick);
        }
        else if (!_isTyping) {
            //AudioManager.Instance.PlaySFX(SoundIndexKey.DimensionSwap);
            StartFade();
        }
    }

    private string RemoveTags(string inputText) {
        var tagRegex = new Regex(@"<wait.*?>|<slower.*?>");
        return tagRegex.Replace(inputText, "");
    }

    private IEnumerator TypeText() {
        displayText.text = "...";
        int charIndex = 0;
        var tagRegex = new Regex(@"<.*?>");

        yield return new WaitForSeconds(1f);
        displayText.text = "";

        float waitTime = 0.15f;
        float typingTime = 0f;

        while (charIndex < _textToType.Length) {
            if (_textToType[charIndex] == '<') {
                var match = tagRegex.Match(_textToType.Substring(charIndex));
                if (match.Success) {
                    if (match.Value == "<wait>") {
                        charIndex += match.Length;

                        yield return new WaitForSeconds(waitTime);
                        continue;
                    }
                    if (match.Value == "<slower>") {
                        charIndex += match.Length;
                        typingTime = 0.02f;
                        continue;
                    }

                    displayText.text += match.Value;
                    charIndex += match.Length;
                    continue;
                }
            }

            if (_textToType[charIndex] == '.' || _textToType[charIndex] == ',' || _textToType[charIndex] == '-' || _textToType[charIndex] == '!') {
                yield return new WaitForSeconds(waitTime);
            }


            if (!char.IsWhiteSpace(_textToType[charIndex])) {
                /*if (Random.Range(0, 5) < 2)
                    //AudioManager.Instance.PlaySFXWithRandomPitch(SoundIndexKey.typingSound, 2, 0.3f);
                else
                    //AudioManager.Instance.PlaySFXWithRandomPitch(SoundIndexKey.typingSound2, 2, 0.3f);*/
            }

            displayText.text += _textToType[charIndex];
            charIndex++;

            yield return new WaitForSeconds((typingSpeed) * Random.Range(0.3f, 1.5f) + typingTime);
        }

        _isTyping = false;
        inputClickText.text = afterTextString;
    }



    private void StartFade() {
        fadePanel.gameObject.SetActive(true); 
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {
        float elapsedTime = 0f;
        Color panelColor = fadePanel.color;

        while (elapsedTime < transitionDuration) {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / transitionDuration;

            panelColor.a = Mathf.Lerp(0f, 1f, normalizedTime);
            fadePanel.color = panelColor;

            yield return null;
        }

        //AudioManager.Instance.SceneTransition(nextScene);
    }
}
