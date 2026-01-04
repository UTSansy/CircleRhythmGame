//using UnityEngine;
//using System.IO;
//using static NoteJudge;

//public class LongNote : ShortNote {
//    protected float maxColliderAngle = 360f;

//    private bool isHolding = false;
//    private bool isReleased = false;
//    private bool isCompleted = false;

//    float deltaAngle = 0f;

//    public override string noteType => "long";

//    /// <summary>
//    /// it does not contains the pre-enter time for long note, need to adjust
//    /// </summary>
//    protected override void Update() {
//        base.Update();

//        if (boardController.GetDelayedSongTime() > startTime) {
//            duration -= boardController.deltaTime;
//            deltaAngle += boardController.defaultBoardRotationSpeed * Time.deltaTime;
//        }

//        if (isHolding) {
//            float newStart = GetStartAngle() + deltaAngle;
//            float end = GetEndAngle();

//            if (duration <= boardController.shortNoteDuration) {
//                isCompleted = true;

//                // automatically stop holding
//                StopHolding();
//            } else {
//                ChangeAngle(newStart, end);
//            }

//            deltaAngle = 0f;

//        } else if (isReleased) {
//            if (!isCompleted) {
//                TriggerMissEffect();
//            } else {
//                // DEBUG FOR NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//                base.TriggerHitEffect(ZoneType.Perfect);
//                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//            }
//        }
//    }

//    // DEBUG FOR NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//    public override void TriggerHitEffect(ZoneType zoneType) {
//        ApplyMaterial(hitPerfectMaterial);
//        InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNotePerfectHitSound);
//    }

//    public override void OnHold() {
//        Debug.Log("On here");
//        isHolding = true;
//    }

//    public override void StopHolding() {
//        isHolding = false;
//        isReleased = true;
//    }

//    [Header("Predefined Collider (360°)")]
//    public ColliderData colliderData;
//    private string savePath;

//    protected override void Awake() {
//        base.Awake();
//        savePath = Path.Combine(Application.dataPath, "LongNoteData", "colliderData.json");
//        LoadColliderData();
//    }

//    protected override void UpdateCollider(Vector3[] vertices) {
//        if (angleSize >= maxColliderAngle && colliderData != null && colliderData.maxColliderPath.Length > 0) {
//            ApplyPredefinedCollider();
//        } else {
//            base.UpdateCollider(vertices);
//        }
//    }

//    private void ApplyPredefinedCollider() {
//        if (colliderData != null && colliderData.maxColliderPath.Length > 0) {
//            polygonCollider.pathCount = 1;
//            polygonCollider.SetPath(0, colliderData.maxColliderPath);
//        }
//    }

//    private void LoadColliderData() {
//        if (File.Exists(savePath)) {
//            string json = File.ReadAllText(savePath);

//            // Deserialize JSON into a temporary class
//            ColliderSaveData data = JsonUtility.FromJson<ColliderSaveData>(json);

//            if (colliderData == null) {
//                colliderData = ScriptableObject.CreateInstance<ColliderData>(); // ✅ Create new instance
//            }

//            colliderData.maxColliderPath = data.points;
//            Debug.Log($"[LongNote] Loaded predefined collider with {colliderData.maxColliderPath.Length} points.");
//        }
//    }

//    //public override void NoteEntered(ZoneType zoneType) {
//    //    // Debug.Log("Start Duration: " + duration);
//    //    if (zoneType == ZoneType.Center) {
//    //        inPointerController = true;
//    //    }
//    //}

//    /// ✅ Data structure for loading collider points
//    [System.Serializable]
//    public class ColliderSaveData {
//        public Vector2[] points;
//    }
//}

using UnityEngine;
using System.IO;
using static NoteJudge;

public class LongNote : ShortNote {
    protected float maxColliderAngle = 360f;

    private bool isHolding = false;
    private bool isReleased = false;
    private bool isCompleted = false;

    float deltaAngle = 0f;

    public override string noteType => "long";

    /// <summary>
    /// it does not contains the pre-enter time for long note, need to adjust
    /// </summary>
    protected override void Update() {
        base.Update();

        if (boardController.GetDelayedSongTime() > startTime) {
            duration -= boardController.deltaTime;
            deltaAngle += boardController.defaultBoardRotationSpeed * Time.deltaTime;
        }

        if (isHolding) {
            float newStart = GetStartAngle() + deltaAngle;
            float end = GetEndAngle();

            if (originalEndTime <= boardController.GetDelayedSongTime()) {
                isCompleted = true;

                // automatically stop holding
                StopHolding();
            } else {
                ChangeAngle(newStart, end);
            }

            deltaAngle = 0f;

        } else if (isReleased) {
            if (!isCompleted) {
                TriggerMissEffect();
            } else {
                // DEBUG FOR NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                base.TriggerHitEffect(ZoneType.Perfect);
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
        }
    }

    // DEBUG FOR NOW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public override void TriggerHitEffect(ZoneType zoneType) {
        ApplyMaterial(hitPerfectMaterial);
        InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNotePerfectHitSound);
    }

    public override void OnHold() {
        isHolding = true;
    }

    public override void StopHolding() {
        isHolding = false;
        isReleased = true;
    }

    //[Header("Predefined Collider (360°)")]
    //public ColliderData colliderData;
    //private string savePath;

    protected override void Awake() {
        base.Awake();
        // savePath = Path.Combine(Application.dataPath, "LongNoteData", "colliderData.json");
        // LoadColliderData();
    }

    //protected override void UpdateCollider(Vector3[] vertices) {
    //    if (angleSize >= maxColliderAngle && colliderData != null && colliderData.maxColliderPath.Length > 0) {
    //        ApplyPredefinedCollider();
    //    } else {
    //        base.UpdateCollider(vertices);
    //    }
    //}

    //private void ApplyPredefinedCollider() {
    //    if (colliderData != null && colliderData.maxColliderPath.Length > 0) {
    //        polygonCollider.pathCount = 1;
    //        polygonCollider.SetPath(0, colliderData.maxColliderPath);
    //    }
    //}

    //private void LoadColliderData() {
    //    if (File.Exists(savePath)) {
    //        string json = File.ReadAllText(savePath);

    //        // Deserialize JSON into a temporary class
    //        ColliderSaveData data = JsonUtility.FromJson<ColliderSaveData>(json);

    //        if (colliderData == null) {
    //            colliderData = ScriptableObject.CreateInstance<ColliderData>(); // ✅ Create new instance
    //        }

    //        colliderData.maxColliderPath = data.points;
    //        Debug.Log($"[LongNote] Loaded predefined collider with {colliderData.maxColliderPath.Length} points.");
    //    }
    //}

    //public override void NoteEntered(ZoneType zoneType) {
    //    // Debug.Log("Start Duration: " + duration);
    //    if (zoneType == ZoneType.Center) {
    //        inPointerController = true;
    //    }
    //}

    ///// ✅ Data structure for loading collider points
    //[System.Serializable]
    //public class ColliderSaveData {
    //    public Vector2[] points;
    //}
}
