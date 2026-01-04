using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.Networking;

public class CircularSongSelector : MonoBehaviour {
    public enum SelectSongType { BuiltIn, User }
    public SelectSongType selectSongType;
    public SelectionAudioManager audioManager;
    private List<AudioClip> songPreviews = new List<AudioClip>();

    private List<string> songNames = new List<string>();
    private List<string> visualList = new List<string>();
    private List<string> extensions = new List<string>();
    private List<RectTransform> entryObjects = new List<RectTransform>();
    private int songNameIndex = 0;

    public RectTransform container;
    public GameObject songEntryPrefab;

    public int visibleCount = 5;
    public int bufferCount = 2;
    public int firstVisibleIndex;
    public int lastVisibleIndex;

    public float entrySpacing;
    public float animationDuration;

    private int holdDirection = 0;
    private int pendingScroll = 0;

    private int lastPreviewIndex = -1;

    private bool isSwitching = false;

    private string musicPath;
    private string folderPath;

    private bool onTransfer = false;

    public void Start() {
        LoadSongsByType();
    }

    public void Update() {
        if (!onTransfer) {
            HandleInput();
        } 
    }

    private void LoadSongsByType() {
        songNames.Clear();

        folderPath = selectSongType == SelectSongType.BuiltIn
            ? Application.streamingAssetsPath
            : Path.Combine(Application.persistentDataPath, "Own_Music") ;

        musicPath = Path.Combine(folderPath, "Music");

        StartCoroutine(LoadAllAudio(musicPath));
    }

    private IEnumerator LoadAllAudio(string folderPath) {
        string[] supportedExtensions = { ".aif", ".wav", ".mp3", ".ogg" };

        var files = Directory.GetFiles(folderPath)
            .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToList();

        List<AudioClip> loadedClips = new List<AudioClip>();
        List<string> loadedNames = new List<string>();

        foreach (var path in files) {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string url = "file://" + path;
            string extension = Path.GetExtension(path);

            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN)) {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success) {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    clip.name = fileName;
                    loadedClips.Add(clip);
                    loadedNames.Add(fileName);
                    extensions.Add(extension);
                    Debug.Log($"✅ Loaded: {fileName}");
                } else {
                    Debug.Log($"❌ Failed to load {fileName}: {uwr.error}");
                }
            }
        }

        songPreviews = loadedClips;
        songNames = loadedNames;

        if (songNames.Count == 0) {
            songNames.Add("No songs found");
            songPreviews = new List<AudioClip>();
        }

        InitializeVisualList();
        InitializeContainer();
        UpdateEntryVisuals();
        LoadingUIController.Instance.HideLoading(1f);
        PlayAudioAtIndex(GetCurrentAudioIndex());
    }

    private void InitializeVisualList() {
        int totalCount = visibleCount + bufferCount * 2;
        int centerIndex = totalCount / 2;

        visualList.Clear();

        for (int i = 0; i < totalCount; i++) {
            int visualIndex = songNameIndex - centerIndex + i;
            while (visualIndex < 0) { visualIndex += songNames.Count; }
            while (visualIndex >= songNames.Count) { visualIndex -= songNames.Count;}
            visualList.Add(songNames[visualIndex]);

            if (i == 0) { firstVisibleIndex = visualIndex; }
            if (i == totalCount - 1) { lastVisibleIndex = visualIndex; }
        }
    }

    private void InitializeContainer(){
        int totalCount = visibleCount + bufferCount * 2;

        for (int i = 0; i < totalCount; i++) {
            GameObject entry = Instantiate(songEntryPrefab, container);
            RectTransform rect = entry.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -entrySpacing * i);
            entryObjects.Add(rect);
        }
    }

    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            holdDirection = -1;
            pendingScroll += holdDirection;
            Scroll(holdDirection);
            audioManager?.PlayScrollSFX();
            isSwitching = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            holdDirection = 1;
            pendingScroll += holdDirection;
            Scroll(holdDirection);
            audioManager?.PlayScrollSFX();
            isSwitching = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && holdDirection == -1) EndHold();
        if (Input.GetKeyUp(KeyCode.DownArrow) && holdDirection == 1) EndHold();

        // CONFRIM USE 1 FIRST
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (!isSwitching) { StartCoroutine(ConfirmAndTransfer()); }
        }
    }

    private IEnumerator ConfirmAndTransfer() {
        onTransfer = true;
        audioManager?.PlaySFX(audioManager.confirmClip);
        LoadingUIController.Instance.ShowLoading();
        yield return new WaitForSeconds(5f); // allow fade-in to visually start

        SongData data = UserBuildMusicSelectionManager.SelectSong(songNames[songNameIndex], folderPath, extensions[songNameIndex]);

        SelectedSongDataManager.Instance.SetSelectedSongData(data);
        TransferToPlayScene();
    }

    public void EndHold() {
        int newIndex = songNameIndex + pendingScroll;
        if (newIndex >= songNames.Count) newIndex -= songNames.Count;
        if (newIndex < 0) newIndex += songNames.Count;

        pendingScroll = 0;

        if (newIndex != songNameIndex) {
            songNameIndex = newIndex;
            InitializeVisualList();
            UpdateEntryVisuals();
            PlayAudioAtIndex(GetCurrentAudioIndex());
            isSwitching = false;
        } else {
            InitializeVisualList();
            UpdateEntryVisuals();
            isSwitching = false;
        }
    }

    private void Scroll(int direction) {
        foreach (var entry in entryObjects) entry.DOKill();
        RotateList(direction);

        for (int i = 0; i < entryObjects.Count; i++) {
            RectTransform entry = entryObjects[i];
            float duration = animationDuration;
            Vector2 target = new Vector2(0, -entrySpacing * i);
            entry.DOAnchorPosY(target.y, duration).SetEase(Ease.OutExpo);
        }
    }

    private void RotateList(int direction) {
        if (direction == -1) {
            var last = entryObjects[entryObjects.Count - 1];
            entryObjects.RemoveAt(entryObjects.Count - 1);
            entryObjects.Insert(0, last);
        } else {
            var first = entryObjects[0];
            entryObjects.RemoveAt(0);
            entryObjects.Add(first);
        }
    }

    private void UpdateEntryVisuals() {
        for (int i = 0; i < entryObjects.Count; i++) {
            RectTransform entry = entryObjects[i];
            int songIndex = (firstVisibleIndex + i) % songNames.Count;

            entry.anchoredPosition = new Vector2(0, -entrySpacing * i);
            TMP_Text text = entry.GetComponent<TMP_Text>();
            text.text = songNames[songIndex];

            bool isCenter = (i == (visibleCount + bufferCount * 2) / 2);
            float alpha = Mathf.Lerp(0.3f, 1f, 1f - Mathf.Abs(i - (visibleCount / 2)) / (visibleCount / 2f));
            float scale = isCenter ? 1.3f : 1f;

            text.fontSize = isCenter ? 50 : 36;
            text.color = Color.yellow;
            text.transform.localScale = Vector3.one * scale;
        }
    }

    public int GetCurrentAudioIndex() {
        if (songPreviews == null || songPreviews.Count == 0) {
            Debug.Log("songPreviews is empty");
            return -1;
        }

        int audioIndex = songNameIndex % songPreviews.Count;
        Debug.Log($"🔍 songPreviews.Length: {songPreviews.Count}, logicalIndex: {songNameIndex}, audioIndex: {audioIndex}");
        return audioIndex;
    }

    public void PlayAudioAtIndex(int index, bool isSwitchedBack = false) {
        if (index < 0 || index >= songPreviews.Count) {
            Debug.Log("out of range");
            return;
        }

        if (isSwitchedBack || index != lastPreviewIndex) {
            AudioClip clip = songPreviews[index];
            if (clip != null) {
                audioManager.PlayMusic(clip);
                lastPreviewIndex = index;
            }
        }
    }

    public void TransferToPlayScene() {
        // Debug.Log("In transfer Play Scene");
        SelectionUIManager.Instance.HideSelectionUI();
        audioManager.FadeOutMusic(3f);
        LoadingUIController.Instance.LoadSceneAsync("Play Scene");
    }

    public string GetSelectedSong() => songNames[songNameIndex];
    public bool GetSwitchingStatus() => isSwitching;
}
