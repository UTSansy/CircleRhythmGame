using UnityEngine;
using System.Collections.Generic;

public class BoardController : MonoBehaviour {
    [Header("Core Components")]
    // [SerializeField] public PointerController pointerController;
    [SerializeField] public NoteSpawner noteSpawner;
    [SerializeField] public RingController ringController;
    [SerializeField] public PointerController pointerController;

    public GameController gameController;
    public NoteJudge noteJudge;

    // ACTUALLY CONTAINS BOTH NOTE, NEED TO CHANGE IF INHERITANCE OF LONG NOTE CHANGES
    public List<ShortNote> noteInBoard;

    [Header("Game Settings")]
    public bool IsBoardRunning { get; private set; } = false;
    public float defaultBoardRotationSpeed;
    public float shortNoteDuration;
    public float deltaTime;
    public int CircleId { get; private set; } = -1;

    private void Awake() {
        FetchComponents();
    }

    public void Update() {
        deltaTime = gameController.deltaTime;
    }

    private void FetchComponents() {
        gameController = FindFirstObjectByType<GameController>();
        pointerController = GetComponentInChildren<PointerController>();
        noteSpawner = GetComponentInChildren<NoteSpawner>();
        ringController = GetComponentInChildren<RingController>();
        noteJudge = GetComponentInChildren<NoteJudge>();
        noteInBoard = new List<ShortNote>();
        NoteHitLogicManager.Instance.RegisterJudge(noteJudge);

        if (!pointerController || !noteSpawner || !ringController) {
            Debug.Log("[BoardController] Missing required components!");
        }
    }

    private void Start() {
        defaultBoardRotationSpeed *= GameSettings.Instance.gameSpeed;
        pointerController.SetRotationSpeed(defaultBoardRotationSpeed);
    }

    public void StartBoard() {
        if (IsBoardRunning) return;
        IsBoardRunning = true;
        pointerController.StartRotation();
        noteSpawner.StartSpawning();
    }

    public void StopBoard() {
        IsBoardRunning = false;
        pointerController.StopRotation();
    }

    public PointerController GetPointerController() => pointerController;
    public NoteSpawner GetNoteSpawner() => noteSpawner;
    public float GetSongTime() => gameController.GetSongTime();
    public float GetDelayedSongTime() => gameController.GetDelayedSongTime();
    public void AssignCircleId(int id) { CircleId = id; }
    public void AssignNoteData(List<NoteData> noteDatas) { noteSpawner.ReceiveNoteData(noteDatas); }
    public void AssignNoteDuration(float noteDuration) { shortNoteDuration = noteDuration; }
}
