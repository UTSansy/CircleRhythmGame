//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PointerController : MonoBehaviour {
//    private float rotationSpeed;
//    private bool isRotating = false;
//    private bool clockwise = true;
//    private float noteSpawnerPosition = 180f;

//    [SerializeField] private Transform pointerColliders;

//    [Header("Zone Colliders")]
//    public PointerZoneCollider missEarlyZone;
//    public PointerZoneCollider earlyZone;
//    public PointerZoneCollider perfectZone;
//    public PointerZoneCollider lateZone;

//    private ShortNote currentNote;

//    private void Start() {
//        //boardController = GetComponentInParent<BoardController>();
//        //rotationSpeed = boardController.defaultBoardRotationSpeed;

//        if (!clockwise) {
//            pointerColliders.localScale = new Vector3(1, 1, -1);
//        }
//    }

//    private void Update() {
//        if (isRotating) RotatePointer();
//    }

//    private void RotatePointer() {
//        int direction = clockwise ? -1 : 1;
//        transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
//    }

//    private void CheckForHit() {
//        if (Input.GetKeyDown(KeyCode.Space)) {
//            TryHitFromZones();
//        }

//        if (Input.GetKeyUp(KeyCode.Space)) {
//            // Optional: propagate release to all zones (in case long note missed mid-hold)
//            earlyZone.GetNote()?.StopHolding();
//            perfectZone.GetNote()?.StopHolding();
//            lateZone.GetNote()?.StopHolding();
//        }
//    }

//    private void TryHitFromZones() {
//        // Priority: Late > Perfect > Early

//        if (lateZone.GetNote() != null) {
//            Debug.Log("Find in late");
//            lateZone.GetNote().OnHold();
//            lateZone.GetNote().TriggerHitEffect(PointerZoneCollider.ZoneType.Late);
//            lateZone.ClearNote();
//            return;
//        }

//        if (perfectZone.GetNote() != null) {
//            Debug.Log("Find in Perfect");
//            perfectZone.GetNote().OnHold();
//            perfectZone.GetNote().TriggerHitEffect(PointerZoneCollider.ZoneType.Perfect);
//            perfectZone.ClearNote();
//            return;
//        }

//        if (earlyZone.GetNote() != null) {
//            Debug.Log("Find in Early");
//            earlyZone.GetNote().OnHold();
//            earlyZone.GetNote().TriggerHitEffect(PointerZoneCollider.ZoneType.Early);
//            earlyZone.ClearNote();
//            return;
//        }

//        if (missEarlyZone.GetNote() != null) {
//            Debug.Log("too early my friend");
//            missEarlyZone.GetNote().TriggerMissEffect();
//            ClearAllZone(missEarlyZone.GetNote());
//            return;
//        }
//    }

//    public void ClearAllZone(ShortNote note) {
//        if (earlyZone.GetNote() == note) earlyZone.ClearNote();
//        if (perfectZone.GetNote() == note) perfectZone.ClearNote();
//        if (lateZone.GetNote() == note) lateZone.ClearNote();
//    }

//    public void StartRotation() => isRotating = true;
//    public void StopRotation() => isRotating = false;
//    public void SetRotationSpeed(float speed) => rotationSpeed = speed;

//    public void SetRotationDirection(bool isClockwise) {
//        clockwise = isClockwise;
//        pointerColliders.localScale = new Vector3(1, 1, clockwise ? 1 : -1);
//    }

//    public bool GetRotationDirection() => clockwise;

//    public void NoteEntered(ShortNote note) {
//        Debug.Log("some note in");
//        currentNote = note;
//    }

//    public void NoteExited(ShortNote note) {
//        if (note == currentNote && !currentNote.IsHit()) {
//            currentNote.TriggerMissEffect();
//        }
//        currentNote = null;
//    }

//    public float GetRotationSpeed() => rotationSpeed;

//    public float GetNoteSpawnerPosition() => noteSpawnerPosition;
//}

using UnityEngine;

public class PointerController : MonoBehaviour {
    private float rotationSpeed;
    private bool isRotating = false;
    private bool clockwise = true;

    [SerializeField] private float noteSpawnerPosition = 180f;

    private void Start() {
        if (!clockwise) {
            transform.localScale = new Vector3(1, 1, -1);
        }
    }

    private void Update() {
        if (isRotating) RotatePointer();
    }

    private void RotatePointer() {
        int direction = clockwise ? -1 : 1;
        transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
    }

    public void StartRotation() => isRotating = true;
    public void StopRotation() => isRotating = false;
    public void SetRotationSpeed(float speed) => rotationSpeed = speed;

    public void SetRotationDirection(bool isClockwise) {
        clockwise = isClockwise;
        transform.localScale = new Vector3(1, 1, clockwise ? 1 : -1);
    }

    public float GetRotationSpeed() => rotationSpeed;
    public bool GetRotationDirection() => clockwise;
    public float GetNoteSpawnerPosition() => noteSpawnerPosition;
}