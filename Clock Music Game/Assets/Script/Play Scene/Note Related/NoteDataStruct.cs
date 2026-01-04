using System.Collections.Generic;
using UnityEngine;
using static GameController;

[System.Serializable]
public class NoteData {
    public string type;
    public float startTime;
    public float endTime;
    public int circleId;
    public string instruction;
    public float originalStartTime;

    // Long only
    public float originalEndTime;


    public NoteData(string type, float startTime, float endTime, string instruction, float _originalStartTime, float _originalEndTime, int circleId) {
        this.type = type;
        this.startTime = startTime;
        this.endTime = endTime;
        this.instruction = instruction;
        this.originalStartTime = _originalStartTime;
        this.originalEndTime = _originalEndTime;
        this.circleId = circleId;
    }
}

public class SongData {
    public string musicPath;
    public List<List<NoteData>> noteData;
    public List<InitialCircleConfig> startCircleConfigs;
    // short note for 1, long note for 2
    public TotalNoteCount totalNoteCount;

    public SongData(string musicPath, List<List<NoteData>> noteData, List<InitialCircleConfig> startCircleConfigs, TotalNoteCount totalNoteCount) {
        this.musicPath = musicPath;
        this.noteData = noteData;
        this.startCircleConfigs = startCircleConfigs;
        this.totalNoteCount = totalNoteCount;
    }
}

public class TotalNoteCount {
    public int shortNote;
    public int longNote;

    public TotalNoteCount() {
        this.shortNote = 0;
        this.longNote = 0;
    }
}
