using UnityEngine;

public class SelectedSongDataManager : MonoBehaviour {
    public static SelectedSongDataManager Instance { get; private set; }

    private SongData selectedSongData;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SetSelectedSongData(SongData data) {
        selectedSongData = data;
        Debug.Log("Song data " + selectedSongData);
    }

    public SongData GetSelectedSongData() {
        return selectedSongData;
    }
}