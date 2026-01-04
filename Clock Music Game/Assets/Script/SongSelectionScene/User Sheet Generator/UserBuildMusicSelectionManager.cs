using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using static GameController;

public static class UserBuildMusicSelectionManager {
    public static string selectedShortNoteMidiFile;
    public static string selectedLongNoteMidiFile;
    public static string selectedJsonFile;
    public static string selectedMusicFile;

    public static float defaultShortNoteDuration = 0.08f;

    public static SongData SelectSong(string songName, string songPath, string extension) {
        //selectedMidiFile = Path.Combine(Application.persistentDataPath, "Own_Music/Place_MIDI", songName + ".mid");
        //selectedJsonFile = Path.Combine(Application.persistentDataPath, "Own_Music/Place_JSON", songName + ".json");
        //selectedMusicFile = Path.Combine(Application.persistentDataPath, "Own_Music/Place_Music", songName + ".aif");

        selectedShortNoteMidiFile = Path.Combine(songPath, "MIDI", songName + "Short.mid");
        selectedLongNoteMidiFile = Path.Combine(songPath, "MIDI", songName + "Long.mid");
        selectedJsonFile = Path.Combine(songPath, "JSON", songName + ".json");
        selectedMusicFile = Path.Combine(songPath, "Music", songName + extension);

        Debug.Log("music path: " + selectedMusicFile);

        List<List<NoteData>> convertedNotes = MidiConverter.ConvertMidiToNoteData(selectedShortNoteMidiFile, selectedLongNoteMidiFile, selectedJsonFile, defaultShortNoteDuration);
        List<InitialCircleConfig> startCircleConfigs = LoadStartConfigs(selectedJsonFile);

        return new SongData(selectedMusicFile, convertedNotes, startCircleConfigs, MidiConverter.totalNoteCount);
    }

    private static List<InitialCircleConfig> LoadStartConfigs(string jsonPath) {
        if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath)) return new List<InitialCircleConfig>();

        try {
            var fullJson = File.ReadAllText(jsonPath);
            var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(fullJson);

            if (root.ContainsKey("Start")) {
                var startJson = root["Start"].ToString();
                return JsonConvert.DeserializeObject<List<InitialCircleConfig>>(startJson);
            }
        } catch (System.Exception e) {
            Debug.Log($"[MusicManager] Failed to load StartCircleConfig: {e.Message}");
        }

        return new List<InitialCircleConfig>();
    }
}