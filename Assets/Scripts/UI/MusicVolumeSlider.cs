using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour {
    [SerializeField] private Slider slider;

    private void Start() {
        slider.value = AudioManager.Instance.GetMusicVolume();
        slider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value) {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnDestroy() {
        slider.onValueChanged.RemoveListener(SetVolume);
    }
}