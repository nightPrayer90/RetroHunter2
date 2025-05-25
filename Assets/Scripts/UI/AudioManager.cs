using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using RetroHunter2;


[Serializable]
public class BGM {
    public string name;
    public AudioClip clip;
}

[Serializable]
public class Sound {
    public SoundIndexKey soundIndexKey;
    public AudioClip clip;
}


public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("MixerGroups")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectGroup;
    private const string PREF_KEY_MUSIC = "volume_music";
    private const string PREF_KEY_SFX = "volume_sfx";

    [Header("Music")]
    [SerializeField] private BGM[] musicSounds;

    [Header("Sounds")]
    [SerializeField] private Sound[] sfxSounds;

    [Header("Components")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private TransitionManager transitionManager;
    private Tweener _musicFadeTweener;

    public Dictionary<SoundIndexKey, AudioClip> sfxDictionary;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        sfxDictionary = sfxSounds.ToDictionary(s => s.soundIndexKey, s => s.clip);

        sfxSource.outputAudioMixerGroup = soundEffectGroup;
        musicSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private IEnumerator Start() {
        yield return null;

        float musicVol = PlayerPrefs.GetFloat(PREF_KEY_MUSIC, 1f);
        float sfxVol = PlayerPrefs.GetFloat(PREF_KEY_SFX, 1f);

        SetMusicVolume(musicVol);
        SetSfxVolume(sfxVol);
    }


    public void PlayMusicWithoutRestart(string name) {
        if (_musicFadeTweener != null && _musicFadeTweener.IsActive()) {
            _musicFadeTweener.Kill();
        }

        BGM s = Array.Find(musicSounds, x => x.name == name);

        if (s == null) {
            Debug.Log("Sound Not Found");
        }
        else {
            if (musicSource.clip != s.clip) {
                musicSource.Stop();
                musicSource.volume = 1f;
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }
    }

    public void FadeOutCurrentMusic(float duration = 1f) {
        if (_musicFadeTweener != null && _musicFadeTweener.IsActive()) {
            _musicFadeTweener.Kill();
        }

        if (musicSource.isPlaying) {
            _musicFadeTweener = musicSource.DOFade(0f, duration)
                .OnComplete(() => {
                    musicSource.Stop();
                    musicSource.volume = 1f;
                });
        }
    }

    public void PlaySFX(SoundIndexKey key) {
        if (sfxDictionary.TryGetValue(key, out AudioClip clip)) {
            sfxSource.PlayOneShot(clip);
        }
        else {
            Debug.Log("Sound Not Found: " + key);
        }
    }

    public void PlaySFXWithRandomPitch(SoundIndexKey key, float pitchRange, float volumeRange) {
        if (sfxDictionary.TryGetValue(key, out AudioClip clip)) {
            if (!sfxSource.isPlaying || sfxSource.clip != clip) {
                float originalPitch = sfxSource.pitch;
                float originalVolume = sfxSource.volume;
                sfxSource.pitch = originalPitch + UnityEngine.Random.Range(-pitchRange, pitchRange);
                sfxSource.volume = originalVolume + UnityEngine.Random.Range(-volumeRange, volumeRange);
                sfxSource.PlayOneShot(clip);


                sfxSource.pitch = originalPitch;
                sfxSource.volume = originalVolume;
            }
        }
        else {
            Debug.Log("Sound Not Found: " + key);
        }
    }

    public void SetMusicVolume(float volume) {
        musicMixerGroup.audioMixer.SetFloat("MusicGroupVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat(PREF_KEY_MUSIC, volume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume) {
        soundEffectGroup.audioMixer.SetFloat("SfxGroupVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat(PREF_KEY_SFX, volume);
        PlayerPrefs.Save();
    }

    public void AssignSfxOutput(AudioSource source) {
        source.outputAudioMixerGroup = soundEffectGroup;
    }

    public float GetMusicVolume() {
        return PlayerPrefs.GetFloat(PREF_KEY_MUSIC, 1f);
    }

    public float GetSfxVolume() {
        return PlayerPrefs.GetFloat(PREF_KEY_SFX, 1f);
    }


    // --------------------------------------------
    // Transition Stuff ---------------------------
    public void SwitchScene(SceneKey key) {
        transitionManager?.StartSceneTransition(key);
    }
}


