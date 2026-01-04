using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using System.Linq;

public static class MidiConverter {
    private static Dictionary<int, int> noteToCircleMapping;

    public static TotalNoteCount totalNoteCount = new TotalNoteCount();

    [System.Serializable]
    public class JsonSongWrapper {
        public Dictionary<string, int> noteToCircle;
        public List<GameController.InitialCircleConfig> Start;
    }

    // **Converts a MIDI file to `NoteData` list**
    public static List<List<NoteData>> ConvertMidiToNoteData(string shortMidiPath, string longMidiPath, string jsonPath, float shortNoteDuration) {
        if (!File.Exists(shortMidiPath) || !File.Exists(longMidiPath)) {
            Debug.Log($"[MidiConverter] MIDI file missing");
            return null;
        }

        JsonSongWrapper songJson = LoadFullJson(jsonPath);
        if (songJson == null) {
            Debug.Log("[MidiConverter] No JSON config loaded.");
            return null;
        }

        noteToCircleMapping = new Dictionary<int, int>();
        foreach (var kv in songJson.noteToCircle) {
            if (int.TryParse(kv.Key, out int note)) {
                noteToCircleMapping[note] = kv.Value;
            } else {
                Debug.Log($"[MidiConverter] Invalid key in noteToCircle: {kv.Key}");
            }
        }

        List<NoteData> flatList = new List<NoteData>();

        //// read both file and get their total note
        totalNoteCount.shortNote = ReadMidi(shortMidiPath, "Short", shortNoteDuration, flatList);
        totalNoteCount.longNote =  ReadMidi(longMidiPath, "Long", shortNoteDuration, flatList);

        return ProcessFlatList(flatList);
    }

    private static JsonSongWrapper LoadFullJson(string jsonPath) {
        if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath)) return null;

        try {
            string json = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<JsonSongWrapper>(json);
        } catch (System.Exception e) {
            Debug.Log($"[MidiConverter] Failed to load JSON: {e.Message}");
            return null;
        }
    }

    private static int ReadMidi(string path, string type, float shortNoteDuration, List<NoteData> flatList) {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            int totalNoteCount = 0;
            // Debug.Log($"[MidiConverter] Reading {type} Note file: {path}");

            var midiFile = MidiFile.Read(stream);
            var tempoMap = midiFile.GetTempoMap();
            var notes = midiFile.GetNotes();

            foreach (var note in notes) {
                int noteNumber = note.NoteNumber;

                int circleId = GetCircleIdFromNote(noteNumber, noteToCircleMapping);
                // Debug.Log("circle id: " + circleId);
                if (circleId < 0) { continue; }

                double startTime = System.Math.Round(TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds, 4);
                double endTime = System.Math.Round(TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, tempoMap).TotalSeconds, 4);

                float adjustedStartTime = (float)startTime - shortNoteDuration / 2;

                totalNoteCount += (type == "Long") ? 2 : 1;

                flatList.Add(new NoteData(type, adjustedStartTime, (float)endTime, "", (float)startTime, (float)endTime, circleId));
            }

            return totalNoteCount;
        }
    }

    private static List<List<NoteData>> ProcessFlatList(List<NoteData> flatList) {
        // Find how many circles are used
        int maxCircleId = flatList.Max(n => n.circleId);

        // Initialize the outer list
        List<List<NoteData>> noteArray = new List<List<NoteData>>();
        for (int i = 0; i <= maxCircleId; i++) {
            noteArray.Add(new List<NoteData>());
        }

        // Distribute notes into their respective lists
        foreach (var note in flatList) {
            noteArray[note.circleId].Add(note);
        }

        // Sort each circle's list by time
        foreach (var list in noteArray) {
            list.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        }

        return noteArray;
    }

    // Map each circle based on the mapping provide in JSON, if no mapping, only 60 will be used as default circle 0
    private static int GetCircleIdFromNote(int noteNumber, Dictionary<int, int> noteToCircleMapping) {
        if (noteToCircleMapping != null && noteToCircleMapping.TryGetValue(noteNumber, out int circleId)) {
            return circleId;
        }

        // Default fallback: only Middle C (60) is allowed as Circle 0
        return noteNumber == 60 ? 0 : -1;
    }

    private static Dictionary<int, int> LoadCircleMapping(string jsonPath) {
        if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath)) return null;

        try {
            string json = File.ReadAllText(jsonPath);
            var data = JsonConvert.DeserializeObject<JsonSongWrapper>(json);

            // Convert string keys (e.g., "60") to int
            Dictionary<int, int> converted = new Dictionary<int, int>();
            foreach (var kv in data.noteToCircle) {
                if (int.TryParse(kv.Key, out int noteNumber)) {
                    converted[noteNumber] = kv.Value;
                } else {
                    Debug.LogWarning($"[MidiConverter] Invalid note key: {kv.Key}");
                }
            }

            return converted;
        } catch (System.Exception e) {
            Debug.Log($"[MidiConverter] Failed to load noteToCircle mapping: {e.Message}");
            return null;
        }
    }

    private static Dictionary<int, string> LoadInstructionMapping(string jsonPath) {
        if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath)) {
            return new Dictionary<int, string>(); // ✅ No JSON = No instructions
        }

        try {
            return JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText(jsonPath));
        } catch (System.Exception e) {
            Debug.Log($"[MidiConverter] Failed to load JSON: {e.Message}");
            return new Dictionary<int, string>();
        }
    }
}