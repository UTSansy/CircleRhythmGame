using UnityEngine;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;

public class NoteSpawner : MonoBehaviour {
    private BoardController boardController;

    [Header("Prefabs")]
    public GameObject shortNotePrefab;
    public GameObject longNotePrefab;

    private bool generateStart;
    private ShortNote currentNote;

    private int NoteIndex;
    public List<NoteData> noteDatas;

    private bool generateANote;
    private float duration;

    private void Start() {
        generateStart = false;
        generateANote = false;
        boardController = GetComponentInParent<BoardController>();
    }

    public void SpawnNote(NoteData noteData) {
        if (noteData.type == "Short") {
            duration = boardController.shortNoteDuration;
            GenerateNote(shortNotePrefab, noteData.originalStartTime, noteData.originalEndTime, noteData.startTime);
        } else if (noteData.type == "Long") {
            duration = noteData.endTime - noteData.startTime;
            GenerateNote(longNotePrefab, noteData.originalStartTime, noteData.originalEndTime, noteData.startTime);
        }
    }

    private void GenerateNote(GameObject prefab, float originalStartTime, float originalEndTime, float startTime) {
        if (generateANote) return;
        generateANote = true;

        float startAngle = GetCurrentAngle();

        GameObject noteInstance = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        ShortNote note = noteInstance.GetComponent<ShortNote>();

        currentNote = note;
        note.duration = duration;
        note.startTime = startTime;
        note.originalStartTime = originalStartTime;
        note.originalEndTime = originalEndTime;
        note.ChangeAngle(startAngle, startAngle);
        boardController.noteInBoard.Add(currentNote);

        generateANote = false;
    }

    public void StartSpawning() {
        generateStart = true;
    }

    public void ReceiveNoteData(List<NoteData> _noteDatas) {
        noteDatas = _noteDatas;
    }

    private void Update() {
        if (!generateStart) return;

        if (NoteIndex < noteDatas.Count && boardController.GetSongTime() >= noteDatas[NoteIndex].startTime) {
            SpawnNote(noteDatas[NoteIndex]);
            NoteIndex++;
        }

        if (currentNote != null) { GrowCurrentNote(); }
    }

    private float GetCurrentAngle() {
        float pointerZ = boardController.pointerController.transform.eulerAngles.z;

        if (pointerZ < 0) { pointerZ += 360f; }
        float calculatedAngle = 90f - pointerZ;
        if (calculatedAngle < 0) { calculatedAngle += 360f; }
        if (!boardController.pointerController.GetRotationDirection()) { calculatedAngle = 360f - calculatedAngle; }

        return calculatedAngle;
    }

    private void GrowCurrentNote() {
        float rotationSpeed = boardController.defaultBoardRotationSpeed;

        if (rotationSpeed <= 0 || duration <= 0) { return; }

        float elapsedTime = boardController.GetSongTime() - noteDatas[NoteIndex - 1].startTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);

        float targetEndAngle = currentNote.GetStartAngle() + (rotationSpeed * duration);
        float newEndAngle = Mathf.Lerp(currentNote.GetStartAngle(), targetEndAngle, progress);

        currentNote.ChangeAngle(currentNote.GetStartAngle(), newEndAngle);

        if (Mathf.Approximately(newEndAngle, targetEndAngle)) {
            currentNote = null;
            generateANote = false;
        }
    }
}
