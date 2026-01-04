////using UnityEngine;

////public class PointerZoneCollider : MonoBehaviour {
////    public enum ZoneType { Early, Perfect, Late, MissEarly, MissLate, Center, None }
////    public ZoneType zoneType;

////    private PointerController pointerController;

////    private ShortNote currentNote;
////    public ShortNote GetNote() => currentNote;

////    public void Awake() {
////        pointerController = GetComponentInParent<PointerController>();
////    }

////    private void OnTriggerEnter2D(Collider2D collision) {
////        ShortNote note = collision.GetComponent<ShortNote>();
////        if (note != null) {
////            if (zoneType == ZoneType.MissLate) {
////                Debug.Log("leave note");
////                note.TriggerMissEffect();
////                pointerController.ClearAllZone(note);
////            } else {
////                Debug.Log("enter note");
////                currentNote = note;
////                note.NoteEntered(zoneType);
////            }
////        }
////    }

////    private void OnTriggerExit2D(Collider2D other) {
////        if (other.TryGetComponent<ShortNote>(out var note)) {
////            if (note == currentNote) {
////                ClearNote();
////            }
////        }
////    }

////    public void ClearNote() {
////        currentNote = null;
////    }
////}

//using UnityEngine;

//public class PointerZoneCollider : MonoBehaviour {
//    public enum ZoneType { Early, Perfect, Late, MissEarly, MissLate, Center, None }
//    public ZoneType zoneType;

//    private ShortNote currentNote;
//    public ShortNote GetNote() => currentNote;

//    private void OnTriggerEnter2D(Collider2D collision) {
//        if (collision.TryGetComponent(out ShortNote note)) {
//            currentNote = note;
//            note.NoteEntered(zoneType);  // purely visual
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other) {
//        if (other.TryGetComponent(out ShortNote note)) {
//            if (note == currentNote) {
//                note.NoteExited(zoneType); // purely visual
//                ClearNote();
//            }
//        }
//    }

//    public void ClearNote() => currentNote = null;
//}
