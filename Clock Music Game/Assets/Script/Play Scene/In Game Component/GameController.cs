using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Rendering.STP;

public class GameController : MonoBehaviour {
    public static GameController Instance { get; private set; }

    // Will be list of data in the future for more game board to join
    public float songTime;
    public float deltaTime;

    [Header("Game Boards Management")]
    public List<BoardController> boardControllers = new List<BoardController>();
    public GameObject boardPrefab;


    public float defaultShortNoteDuration;
    private bool isGameRunning = false;
    private float musicDelay;

    private SongData songData;

    public ScoreManager scoreManager;

    [System.Serializable]
    public class InitialCircleConfig {
        public int circleId;
        public float[] position;
        public float scale;
        public string direction;
        public float rotationSpeed;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start() {
        songTime = 0;
        SetSystemSampleRate();
        scoreManager = FindFirstObjectByType<ScoreManager>();

        yield return new WaitUntil(() => SelectedSongDataManager.Instance?.GetSelectedSongData() != null);
        songData = SelectedSongDataManager.Instance.GetSelectedSongData();

        yield return StartCoroutine(InGameAudioManager.Instance.AssignPlayMusic(songData.musicPath));

        scoreManager.CalculateNoteValue(songData.totalNoteCount);
        InitializeStartingBoards();

        LoadingUIController.Instance.HideLoading();

        yield return new WaitForSeconds(5f);

        StartGame();
    }

    private void Update() {
        deltaTime = Time.deltaTime;

        if (!InGameAudioManager.Instance.isMusicReady) { return; }

        // Start song time only when actual music starts
        if (isGameRunning) {
            songTime += Time.deltaTime;
            if (InGameAudioManager.Instance.songEnded) { EndGame(); }
        }
    }

    private void InitializeStartingBoards() {
        if (songData == null || songData.startCircleConfigs == null) {
            Debug.Log("[GameController] No start circle configs found in song data.");
            return;
        }

        foreach (InitialCircleConfig config in songData.startCircleConfigs) { CreateNewBoard(config); }
    }

    public void CreateNewBoard(InitialCircleConfig config) {
        // Instantiate a board prefab from Resources folder (adjust path if needed)
        GameObject boardGO = Instantiate(boardPrefab);

        // Set position and scale
        boardGO.transform.position = new Vector2(config.position[0], config.position[1]);
        boardGO.transform.localScale = Vector3.one * config.scale;

        // Get controller and assign properties
        BoardController board = boardGO.GetComponent<BoardController>();
        board.AssignCircleId(config.circleId);
        board.AssignNoteDuration(defaultShortNoteDuration);
        if (board == null) {
            Debug.Log("[GameController] Instantiated board prefab missing BoardController!");
            return;
        }

        PointerController pc = board.GetPointerController();
        pc.SetRotationDirection(config.direction == "clockwise" || config.direction == "Clockwise");
        pc.StartRotation();

        boardControllers.Add(board);
        if (config.circleId >= 0 && config.circleId < songData.noteData.Count) {
            board.AssignNoteData(songData.noteData[config.circleId]);
        } else {
            Debug.Log($"[GameController] No NoteData found for circleId {config.circleId}, assigning empty list.");
            board.AssignNoteData(new List<NoteData>());
        }
        Debug.Log($"[GameController] Created board {config.circleId} at {config.position[0]}, {config.position[1]}");

    }

    // Starts the game across all boards
    public void StartGame() {

        // Check if the game is currently running
        if (isGameRunning) { return; }
        isGameRunning = true;

        // Start each board currently exist
        foreach (BoardController board in boardControllers) { board.StartBoard(); }

        // Use the first board's pointer controller for timing
        if (boardControllers.Count > 0) {
            PointerController firstPointer = boardControllers[0].GetPointerController();
            musicDelay = firstPointer.GetNoteSpawnerPosition() / firstPointer.GetRotationSpeed();
            StartCoroutine(DelayedMusicStart());
        }
    }

    // Pauses the game across all boards
    public void PauseGame() {

        // Check if the game is already paused
        if (!isGameRunning) { return; }
        isGameRunning = false;

        // Pause all boards
        foreach (BoardController board in boardControllers) { board.StopBoard(); }

        // Pause the music
        InGameAudioManager.Instance.PauseMusic();
    }

    // **Ends the game across all boards**
    public void EndGame() {

        // Check if the game is currently running (not sure if necessary?)
        if (!isGameRunning) return;
        isGameRunning = false;

        // Stop all Board
        foreach (BoardController board in boardControllers) { board.StopBoard(); }

        LoadingUIController.Instance.ShowLoading();
        LoadingUIController.Instance.LoadSceneAsync("Song Selection Scene");
    }

    // Handles delayed music start
    private IEnumerator DelayedMusicStart() {
        yield return new WaitForSeconds(musicDelay);
        InGameAudioManager.Instance.PlayMusic();
        // songStarted = true;
    }

    public List<BoardController> GetAllBoards() => boardControllers;
    public float GetSongTime() => songTime;
    public float GetDelayedSongTime() => songTime - musicDelay;

    // For adjusting outputs
    private void SetSystemSampleRate() {
        int systemSampleRate = AudioSettings.outputSampleRate;
        Debug.Log($"[GameController] System Sample Rate: {systemSampleRate} Hz");

        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.sampleRate = systemSampleRate;
        AudioSettings.Reset(config);

        Debug.Log($"[GameController] Updated Unity Sample Rate to {AudioSettings.outputSampleRate} Hz");
    }
}
