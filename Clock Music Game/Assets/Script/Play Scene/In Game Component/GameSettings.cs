using UnityEngine;

public class GameSettings : MonoBehaviour {
    public static GameSettings Instance { get; private set; }

    [Range(0.5f, 3f)] public float gameSpeed = 1; // Default speed

    // public float debugTimeScale = 1f;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        //// DEBUG +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //// DEBUG!!!!!!!!!! Slow game speed
        //Time.timeScale = debugTimeScale;
        //InGameAudioManager.Instance.musicSource.pitch = debugTimeScale;
        //// +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }

    void Start() {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;

        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
        gameSpeed = 1f;
    }

    public float GetGameSpeed() => gameSpeed;
    public void SetGameSpeed(float speed) => gameSpeed = speed;

    void OnGUI() {
        GUI.Label(new Rect(10, 10, 300, 30), "FPS: " + (1f / Time.unscaledDeltaTime).ToString("F2"));
    }
}
