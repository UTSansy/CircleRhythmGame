using UnityEngine;
using System.Collections.Generic;
using static NoteJudge;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance { get; private set; }

    private float currentScore = 0f;
    private float noteValue = 0f;
    private int totalNotes = 0;

    [SerializeField]
    private float goodScoreRatio;

    [Header("Scoring")]
    public float totalScore = 1000000f;

    public float debugScore = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void CalculateNoteValue(TotalNoteCount totalNoteCount) {
        totalNotes = totalNoteCount.shortNote + totalNoteCount.longNote;
        noteValue = (totalNotes > 0) ? totalScore / totalNotes : 0f;

        Debug.Log($"[ScoreManager] Total Notes: {totalNotes}, Each Note Worth: {noteValue}");
    }

    //public void ProcessNoteHit(ShortNote note) {
    //    if (note == null) {
    //        Debug.Log("[ScoreManager] No note hit.");
    //        return;
    //    }

    //    float startAngle = note.GetStartAngle();
    //    float endAngle = note.GetEndAngle();
    //    float angleSize = Mathf.Abs(endAngle - startAngle); // Just a placeholder use

    //    // You can replace this with accuracy logic later
    //    AddHit(false); // false = short note (default for ShortNote)
    //}

    public void AddHit(string noteType, ZoneType zoneType) {
        // Better change to some other comparison!!!!!!!!!
        if (noteType == "long") { Debug.Log("I dont have it for now! work as short note"); }
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        float multiplier;

        switch (zoneType) {
            case ZoneType.Perfect:
                multiplier = 1f;
                break; 
            case ZoneType.Early:
            case ZoneType.Late:
                multiplier = goodScoreRatio;
                break;
            default:
                multiplier = 0f;
                break;
        }

        // float baseValue = isLongNote ? noteValue * 2f : noteValue;
        float scoreToAdd = noteValue * multiplier;
        currentScore += scoreToAdd;

        // DEBUG +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        debugScore += noteValue;
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    }


    public void ResetScore() {
        currentScore = 0f;
    }

    public float GetScore() => currentScore;
    public int GetTotalNotes() => totalNotes;
    public float GetNoteValue() => noteValue;
}