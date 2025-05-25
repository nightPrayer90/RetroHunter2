using UnityEngine;
using UnityEngine.UI;

public class SfxVolumeSlider : MonoBehaviour {
    [SerializeField] private Slider slider;

    private void Start() {
        slider.value = AudioManager.Instance.GetSfxVolume();
        slider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float value) {
        AudioManager.Instance.SetSfxVolume(value);
    }

    private void OnDestroy() {
        slider.onValueChanged.RemoveListener(SetVolume);
    }
}