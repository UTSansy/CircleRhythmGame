using UnityEngine;

public class MenuAudioManager : MonoBehaviour {
    public static MenuAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [HideInInspector] public AudioSource sfxSource;

    [Header("UI Sounds")]
    public AudioClip scrollClip;
    public AudioClip confirmClip;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        } else {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip) {
        if (clip != null && sfxSource != null) {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayScrollSFX() {
        Debug.Log("play scroll");
        PlaySFX(scrollClip);
    }

    public void PlayConfirmSFX() {
        Debug.Log("play confirm");
        PlaySFX(confirmClip);
    }
}
