using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColliderDataSaver : MonoBehaviour {
    public ColliderData colliderData;  // Assigned in Inspector

    private string filePath;

    void Start() {
        filePath = Path.Combine(Application.persistentDataPath, "ColliderData.json");

        if (File.Exists(filePath)) {
            Debug.Log("[ColliderDataSaver] Collider data file already exists. Overwriting...");
        }

        SaveColliderData();
    }

    public void SaveColliderData() {
        if (colliderData == null || colliderData.maxColliderPath == null || colliderData.maxColliderPath.Length == 0) {
            Debug.LogError("[ColliderDataSaver] No valid collider data to save!");
            return;
        }

        string json = JsonUtility.ToJson(new ColliderDataWrapper(colliderData.maxColliderPath), true);
        File.WriteAllText(filePath, json);
        Debug.Log($"[ColliderDataSaver] Data saved to {filePath}");
    }

    public void LoadColliderData() {
        if (!File.Exists(filePath)) {
            Debug.LogError("[ColliderDataSaver] No saved collider data found!");
            return;
        }

        string json = File.ReadAllText(filePath);
        ColliderDataWrapper loadedData = JsonUtility.FromJson<ColliderDataWrapper>(json);
        colliderData.maxColliderPath = loadedData.data;
        Debug.Log($"[ColliderDataSaver] Loaded collider data with {colliderData.maxColliderPath.Length} points.");
    }

    [System.Serializable]
    private class ColliderDataWrapper {
        public Vector2[] data;
        public ColliderDataWrapper(Vector2[] d) { data = d; }
    }
}