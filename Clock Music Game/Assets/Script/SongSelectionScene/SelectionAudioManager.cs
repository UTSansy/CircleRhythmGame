using DG.Tweening;
using UnityEngine;

public class SelectionAudioManager : MonoBehaviour {
    public static SelectionAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [HideInInspector] public AudioSource sfxSource;
    [HideInInspector] public AudioSource musicSource;

    [Header("UI Sounds")]
    public AudioClip scrollClip;
    public AudioClip confirmClip;

    void Awake() {
        if (Instance == null) {
            Instance = this;

            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        } else {
            Destroy(gameObject);
        }
    }

    public void Start() {
        SetSystemSampleRate();
    }

    private void SetSystemSampleRate() {
        int systemSampleRate = AudioSettings.outputSampleRate;
        Debug.Log($"[GameController] System Sample Rate: {systemSampleRate} Hz");

        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.sampleRate = systemSampleRate;
        AudioSettings.Reset(config);

        Debug.Log($"[GameController] Updated Unity Sample Rate to {AudioSettings.outputSampleRate} Hz");
    }

    public void PlaySFX(AudioClip clip) {
        if (clip != null && sfxSource != null) {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayScrollSFX() {
        PlaySFX(scrollClip);
    }

    public void PlayConfirmSFX() {
        PlaySFX(confirmClip);
    }

    public void PlayMusic(AudioClip musicClip) {
        if (musicClip == null) {
            Debug.Log("⚠️ Tried to play a null musicClip");
            return;
        }

        Debug.Log($"🎵 Playing: {musicClip.name}");

        if (musicSource.clip != musicClip) {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void FadeOutMusic(float duration = 1f) {
        if (musicSource.isPlaying) {
            musicSource.DOFade(0f, duration).OnComplete(() => {
                musicSource.Stop();
                musicSource.volume = 1f;
            });
        }
    }

    public void StopMusic() {
        if (musicSource.isPlaying) { musicSource.Stop(); }
    }
}
