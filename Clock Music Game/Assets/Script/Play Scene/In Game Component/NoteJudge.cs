using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;

public class NoteJudge : MonoBehaviour {
    [Header("Timing Windows")]
    public float tooEarly;
    public float early;
    public float perfect;
    public float late;
    public float tooLate;

    public enum ZoneType {
        None = 0,
        MissLate = 1,
        MissEarly = 2,
        Late = 3,
        Early = 4,
        Perfect = 5,
    }

    private BoardController boardController;
    private int nextNoteIndex;

    private ShortNote currHoldingNote = null;

    private void Awake() {
        boardController = GetComponentInParent<BoardController>();
        nextNoteIndex = 0;
    }

    private void Update() {
        if (!boardController.IsBoardRunning) { return; }

        MissCheck();
        if (Input.GetKeyUp(KeyCode.Space)) { TryRelease(); }
    }

    private void MissCheck() {
        while (nextNoteIndex < boardController.noteInBoard.Count) {
            var note = boardController.noteInBoard[nextNoteIndex];
            if (note.originalStartTime + tooLate <= boardController.GetDelayedSongTime()) {
                note.TriggerMissEffect();
                nextNoteIndex++;
            } else {
                break;
            }
        }
    }

    public (ZoneType, float delta) TryHit() {
        if (nextNoteIndex >= boardController.noteInBoard.Count) return (ZoneType.None, 0);

        var note = boardController.noteInBoard[nextNoteIndex];
        float currentTime = boardController.GetDelayedSongTime();
        float delta = note.originalStartTime - currentTime;

        // Debug.Log("delta = " + delta + ", original = " + note.originalStartTime + ", curr = " + currentTime);

        ZoneType zone = GetZone(delta);

        return (zone, delta);
    }

    public void HitNote(ZoneType zone) {
        if (zone == ZoneType.None) return;

        var note = boardController.noteInBoard[nextNoteIndex];

        if (zone == ZoneType.MissLate || zone == ZoneType.MissEarly) {
            note.TriggerMissEffect();
        } else if (note.noteType == "short") {
            note.TriggerHitEffect(zone);
        } else if (note.noteType == "long") {
            note.TriggerHitEffect(zone);
            note.OnHold();
            currHoldingNote = note;
        }
        nextNoteIndex++;
    }

    private void TryRelease() {
        if (currHoldingNote == null) return;
        if (currHoldingNote.isHit == true) return;

        currHoldingNote.StopHolding();
        currHoldingNote = null;
    }

    private ZoneType GetZone(float delta) {
        if (delta > tooEarly) return ZoneType.None;

        float offset = Math.Abs(delta);

        Debug.Log("offset = " + offset + ", perfect = " + perfect + ", condition = " + (offset <= perfect));

        if (offset <= perfect) {
            return ZoneType.Perfect;
        } else if (delta < 0 && offset <= early) {
            return ZoneType.Early;
        } else if (delta > 0 && offset <= late) {
            return ZoneType.Late;
        } else if (delta < 0) {
            return ZoneType.MissEarly;
        }

        return ZoneType.MissLate;
    }
}