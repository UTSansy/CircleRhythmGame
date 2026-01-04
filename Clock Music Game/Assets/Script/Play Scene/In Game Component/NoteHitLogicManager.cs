using System.Collections.Generic;
using UnityEngine;

public class NoteHitLogicManager : MonoBehaviour {
    public static NoteHitLogicManager Instance { get; private set; }

    NoteJudge.ZoneType bestZone;
    private float bestDelta;
    private List<NoteJudge> registeredJudges = new();
    private List<(NoteJudge.ZoneType zone, float delta, NoteJudge)> sortByZone = new();
    private List<(NoteJudge.ZoneType zone, float delta, NoteJudge)> sortByDelta = new();


    void Awake() {
        Instance = this;
    }

    public void RegisterJudge(NoteJudge judge) {
        registeredJudges.Add(judge);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GetBestPosition();
        }
    }

    private void GetBestPosition() {
        sortByZone.Clear();
        sortByDelta.Clear();


        bestZone = NoteJudge.ZoneType.None;
        bestDelta = 1000;

        foreach (NoteJudge judge in registeredJudges) {
            if (judge == null) continue;

            (NoteJudge.ZoneType zone, float delta) data = judge.TryHit();
            if (data.zone == NoteJudge.ZoneType.None) continue;
            if (data.zone > bestZone) bestZone = data.zone;

            sortByZone.Add((data.zone, data.delta, judge));
        }

        foreach (var (zone, delta, judge) in sortByZone) {
            if (zone == bestZone) {
                if (delta < bestDelta) bestDelta = delta;
                sortByDelta.Add((zone, delta, judge));
            }
        }

        foreach (var (zone, delta, judge) in sortByDelta) {
            if (delta <= bestDelta) judge.HitNote(zone);
        }
    }
}