using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class InGameAudioManager : MonoBehaviour {
    public static InGameAudioManager Instance { get; private set; }

    // Background Music
    public AudioSource musicSource;

    // Sound Effects (pass by param -- see PlaySound())
    private AudioSource soundEffect;

    [Header("Audio Clips")]
    public AudioClip shortNotePerfectHitSound;
    public AudioClip shortNoteGoodHitSound;
    // public AudioClip strongShortNoteHitSound;  ????

    public AudioClip gameMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume;
    [Range(0f, 1f)] public float sfxVolume;

    // to track if the music is ready
    public bool isMusicReady = false;
    public bool songEnded { get; private set; } = false;

    void Awake() {

        // make sure there is only one Audio manager
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        // create separate AudioSources for both OST and OSE
        musicSource = gameObject.AddComponent<AudioSource>();
        soundEffect = gameObject.AddComponent<AudioSource>();

        // only start to play when game start, only play once
        musicSource.loop = false;
        musicSource.playOnAwake = false;
        soundEffect.playOnAwake = false;
    }

    // load the music based on the filePath provided
    public IEnumerator AssignPlayMusic(string filePath) {
        //// if file not found, exit
        //// probably wont need it afterwards ++++++++++++++++++++++++++++
        //if (!File.Exists(filePath)) {
        //    Debug.Log($"[AudioManager] File not found");
        //    yield break;
        //}
        //// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Debug.Log($"[AudioManager] Loading {filePath}...");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.UNKNOWN)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                gameMusic = clip;
                musicSource.clip = gameMusic;
                isMusicReady = true;
                Debug.Log("[AudioManager] Music loaded successfully.");
            } else {
                Debug.Log($"[AudioManager] Failed to load music: {www.error}");
            }
        }
    }

    public float GetMusicAmplitude(int frequencyBand) {
        if (!isMusicReady || musicSource == null) return 0f;

        float[] spectrumData = new float[256];
        musicSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Make sure the frequency band is within bounds
        int index = Mathf.Clamp(frequencyBand, 0, spectrumData.Length - 1);

        return spectrumData[index];
    }

    // play music if the music is ready and not already playing
    public void PlayMusic() {
        if (isMusicReady && !musicSource.isPlaying) {
            songEnded = false;
            musicSource.Play();
            StartCoroutine(WaitForSongEnd());
        }
    }

    // stop the music (will reset)
    public void StopMusic() {
        if (musicSource.isPlaying) {
            musicSource.Stop();
            songEnded = true;
        }
    }

    // play the audio effect
    public void PlaySound(AudioClip clip) {
        if (clip != null) {
            soundEffect.volume = sfxVolume;
            soundEffect.PlayOneShot(clip);
        }
    }

    private IEnumerator WaitForSongEnd() {
        yield return new WaitUntil(() => !musicSource.isPlaying);
        yield return new WaitForSeconds(3f);
        songEnded = true;
    }

    // use for UI +++++++++++++++++++++++++++++++++++

    // pause the music (will not reset)
    public void PauseMusic() {
        musicSource.Pause();
    }

    // UnPause the music (will not reset)
    public void UnPauseMusic() {
        musicSource.UnPause();
    }
    // +++++++++++++++++++++++++++++++++++++++++++++++
}
