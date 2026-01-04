using UnityEngine;
using System.Collections.Generic;

public class DebugLogger : MonoBehaviour {
    private static List<string> logMessages = new List<string>();
    private static object lockObject = new object();
    private Vector2 scrollPosition;
    private int maxLogCount = 200; // 🔥 Stores 200 logs

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    /// ✅ Capture Unity Console logs with LogType
    void HandleLog(string logString, string stackTrace, LogType type) {
        lock (lockObject) {
            if (logMessages.Count > maxLogCount) {
                logMessages.RemoveAt(0); // 🔥 Keep only last 200 logs
            }
            string logEntry = $"[{type}] {logString}";
            logMessages.Add(logEntry);
        }
    }

    void OnGUI() {
        GUIStyle logStyle = new GUIStyle();
        logStyle.fontSize = 20; // 🔥 Larger text for readability
        logStyle.normal.textColor = Color.white;

        GUI.Box(new Rect(10, 10, 1600, 1000), "Debug Log"); // 🔥 MASSIVE SIZE

        scrollPosition = GUI.BeginScrollView(new Rect(10, 40, 1580, 950), scrollPosition, new Rect(0, 0, 1560, logMessages.Count * 30));

        lock (lockObject) {
            for (int i = 0; i < logMessages.Count; i++) {
                GUI.Label(new Rect(10, i * 30, 1560, 30), logMessages[i], logStyle);
            }
        }

        GUI.EndScrollView();

        // ✅ **Auto-scroll to the latest log**
        if (logMessages.Count > 0) {
            scrollPosition.y = Mathf.Infinity;
        }
    }
}