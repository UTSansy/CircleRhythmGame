//using System.Collections.Generic;
//using UnityEngine;
//using static NoteJudge;

//public class ShortNote : MonoBehaviour {

//    // Not even sure if this will be useful anymore ++++++++++++++++++++++++++++
//    public enum NoteGrowthDirection { Center, Left, Right }
//    public NoteGrowthDirection growthDirection = NoteGrowthDirection.Center;
//    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//    [Header("Note Settings")]
//    [SerializeField] protected float startAngle;
//    [SerializeField] protected float endAngle;
//    [SerializeField] protected int segments = 50;
//    public float duration;

//    public float startTime;
//    public float originalStartTime;
//    public float originalEndTime;

//    protected Rigidbody2D rb;
//    public virtual string noteType => "short";

//    // if the note hitted or missed
//    public bool isHit = false;

//    // materials that correspond to different note status
//    [Header("Materials")]
//    [SerializeField] protected Material defaultMaterial;
//    [SerializeField] protected Material hitPerfectMaterial;
//    [SerializeField] protected Material hitGoodMaterial;
//    [SerializeField] protected Material missMaterial;

//    protected MeshFilter meshFilter;
//    protected MeshRenderer meshRenderer;
//    protected PolygonCollider2D polygonCollider;
//    protected RingController ring;
//    protected BoardController boardController;

//    // these will be determined at run time
//    protected float innerRadius;
//    protected float outerRadius;
//    protected float angleSize;

//    protected float previousStartAngle;
//    protected float previousEndAngle;

//    protected virtual void Awake() {
//        // get all required components
//        rb = GetComponent<Rigidbody2D>();
//        rb.bodyType = RigidbodyType2D.Kinematic;
//        ring = GetComponentInParent<RingController>();
//        meshRenderer = GetComponent<MeshRenderer>();
//        meshFilter = GetComponent<MeshFilter>();
//        polygonCollider = GetComponent<PolygonCollider2D>();
//        boardController = GetComponentInParent<BoardController>();
//    }

//    protected virtual void OnEnable() {
//        CalculateRadius();
//        AdjustAngleBasedOnClockwise();
//        GenerateSectorMesh();
//        ApplyMaterial(defaultMaterial);
//    }

//    public void InitializeNote(float _startAngle, float _endAngle) {
//        startAngle = _startAngle;
//        endAngle = _endAngle;

//        AdjustEndAngles();
//        AdjustAngleBasedOnClockwise();
//        GenerateSectorMesh();
//        UpdateCollider(meshFilter.mesh.vertices);
//        ApplyMaterial(defaultMaterial);
//    }

//    protected virtual void Update() {
//        if (!Mathf.Approximately(startAngle, previousStartAngle) || !Mathf.Approximately(endAngle, previousEndAngle)) {
//            previousStartAngle = startAngle;
//            previousEndAngle = endAngle;

//            AdjustAngleBasedOnClockwise();
//            AdjustEndAngles();
//            GenerateSectorMesh();
//            UpdateCollider(meshFilter.mesh.vertices);
//            ApplyMaterial(defaultMaterial);
//        }
//    }

//    private void CalculateRadius() {
//        innerRadius = ring.GetInnerRadius() / 100f * 0.5f;
//        outerRadius = ring.GetOuterRadius() / 100f * 0.5f;
//    }

//    // ensure that the end angle is always larger for equal to start angle
//    protected void AdjustEndAngles() {
//        if (endAngle < startAngle) { endAngle = startAngle; }
//        angleSize = Mathf.Abs(endAngle - startAngle);
//    }

//    // mesh generator, make sure it matches the circle
//    protected virtual void GenerateSectorMesh() {
//        Mesh mesh = new Mesh();
//        meshFilter.mesh = mesh;
//        mesh.name = "ShortNoteMesh";

//        int vertexCount = (segments + 1) * 2;
//        Vector3[] vertices = new Vector3[vertexCount];
//        int[] triangles = new int[segments * 6];

//        float angleStep = angleSize / segments;

//        for (int i = 0; i <= segments; i++) {
//            float angle = (angleStep * i) * Mathf.Deg2Rad;
//            vertices[i] = new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);
//            vertices[i + segments + 1] = new Vector3(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius, 0);
//        }

//        for (int i = 0; i < segments; i++) {
//            int innerIndex1 = i;
//            int innerIndex2 = i + 1;
//            int outerIndex1 = i + segments + 1;
//            int outerIndex2 = i + segments + 2;

//            int baseIndex = i * 6;

//            triangles[baseIndex] = innerIndex1;
//            triangles[baseIndex + 1] = outerIndex1;
//            triangles[baseIndex + 2] = outerIndex2;

//            triangles[baseIndex + 3] = innerIndex1;
//            triangles[baseIndex + 4] = outerIndex2;
//            triangles[baseIndex + 5] = innerIndex2;
//        }

//        mesh.vertices = vertices;
//        mesh.triangles = triangles;
//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
//        UpdateCollider(vertices);
//    }

//    // Update collider to match the new note size
//    protected virtual void UpdateCollider(Vector3[] vertices) {
//        List<Vector2> points = new List<Vector2>();
//        int halfCount = vertices.Length / 2;
//        float minSpacing = 0.005f;

//        polygonCollider.pathCount = 1;

//        for (int i = halfCount; i < vertices.Length; i++) {
//            Vector2 newPoint = new Vector2(vertices[i].x, vertices[i].y);
//            if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > minSpacing) {
//                points.Add(newPoint);
//            }
//        }

//        for (int i = halfCount - 1; i >= 0; i--) {
//            Vector2 newPoint = new Vector2(vertices[i].x, vertices[i].y);
//            if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > minSpacing) {
//                points.Add(newPoint);
//            }
//        }

//        points.Add(points[0]);
//        polygonCollider.SetPath(0, points.ToArray());
//    }

//    protected virtual void ApplyMaterial(Material material) {
//        meshRenderer.sharedMaterial = material;
//    }

//    // effect when player hits the note
//    // WILL NEED TO ADD SCORE SYSTEM LATER +++++++++++++++++
//    public virtual void TriggerHitEffect(ZoneType zoneType) {
//        // Prevent duplicate triggers
//        if (isHit) return;
//        isHit = true;

//        if (zoneType == ZoneType.Perfect) {
//            ApplyMaterial(hitPerfectMaterial);
//            InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNotePerfectHitSound);
//        } else {
//            ApplyMaterial(hitGoodMaterial);
//            InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNoteGoodHitSound);
//        }

//        ScoreManager.Instance.AddHit(this.noteType, zoneType);

//        if (TryGetComponent<Collider2D>(out Collider2D col)) {
//            col.enabled = false;
//        }

//        Destroy(gameObject, 0.3f);
//    }

//    // effect when player missed the note
//    // WILL NEED TO ADD HP SYSTEM LATER +++++++++++++++++
//    public virtual void TriggerMissEffect() {
//        // Prevent duplicate triggers
//        if (isHit) return;
//        isHit = true;

//        // falling effect
//        rb.bodyType = RigidbodyType2D.Dynamic;
//        rb.gravityScale = 1f;
//        rb.linearVelocity = new Vector2(0, -3f);

//        ApplyMaterial(missMaterial);

//        // Disable collider after hit
//        if (TryGetComponent<Collider2D>(out Collider2D col)) {
//            col.enabled = false;
//        }

//        Destroy(gameObject, 1.5f);
//    }

//    // Start angle behavior (will change to end angle if conterclockwise)
//    protected virtual void AdjustAngleBasedOnClockwise() {
//        transform.rotation = boardController.pointerController.GetRotationDirection() ?
//            Quaternion.Euler(0, 0, -endAngle) : Quaternion.Euler(0, 0, startAngle);
//    }

//    // updates angle to the expected new angle
//    public void ChangeAngle(float newStartAngle, float newEndAngle) {
//        startAngle = newStartAngle;
//        endAngle = newEndAngle;
//        AdjustEndAngles();
//        AdjustAngleBasedOnClockwise();
//        GenerateSectorMesh();
//        UpdateCollider(meshFilter.mesh.vertices);
//        ApplyMaterial(defaultMaterial);
//    }

//    // accessor of start angle
//    public float GetStartAngle() {
//        return startAngle;
//    }

//    // accessor of end angle
//    public float GetEndAngle() {
//        return endAngle;
//    }

//    public virtual bool IsHit() {
//        return isHit;
//    }

//    public virtual void StopHolding() { }
//    public virtual void OnHold() { Debug.Log("I got called FUCK"); }
//    //public virtual void NoteEntered(ZoneType zoneType) { }
//}

using System.Collections.Generic;
using UnityEngine;
using static NoteJudge;

public class ShortNote : MonoBehaviour {

    // Not even sure if this will be useful anymore ++++++++++++++++++++++++++++
    public enum NoteGrowthDirection { Center, Left, Right }
    public NoteGrowthDirection growthDirection = NoteGrowthDirection.Center;
    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    [Header("Note Settings")]
    [SerializeField] protected float startAngle;
    [SerializeField] protected float endAngle;
    [SerializeField] protected int segments = 50;
    public float duration;

    public float startTime;
    public float originalStartTime;
    public float originalEndTime;

    protected Rigidbody2D rb;
    public virtual string noteType => "short";

    // if the note hitted or missed
    public bool isHit = false;

    // materials that correspond to different note status
    [Header("Materials")]
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material hitPerfectMaterial;
    [SerializeField] protected Material hitGoodMaterial;
    [SerializeField] protected Material missMaterial;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected PolygonCollider2D polygonCollider;
    protected RingController ring;
    protected BoardController boardController;

    // these will be determined at run time
    protected float innerRadius;
    protected float outerRadius;
    protected float angleSize;

    protected float previousStartAngle;
    protected float previousEndAngle;

    protected virtual void Awake() {
        // get all required components
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        ring = GetComponentInParent<RingController>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        // polygonCollider = GetComponent<PolygonCollider2D>();
        boardController = GetComponentInParent<BoardController>();
    }

    protected virtual void OnEnable() {
        CalculateRadius();
        AdjustAngleBasedOnClockwise();
        GenerateSectorMesh();
        ApplyMaterial(defaultMaterial);
    }

    public void InitializeNote(float _startAngle, float _endAngle) {
        startAngle = _startAngle;
        endAngle = _endAngle;

        AdjustEndAngles();
        AdjustAngleBasedOnClockwise();
        GenerateSectorMesh();
        // UpdateCollider(meshFilter.mesh.vertices);
        ApplyMaterial(defaultMaterial);
    }

    protected virtual void Update() {
        if (!Mathf.Approximately(startAngle, previousStartAngle) || !Mathf.Approximately(endAngle, previousEndAngle)) {
            previousStartAngle = startAngle;
            previousEndAngle = endAngle;

            AdjustAngleBasedOnClockwise();
            AdjustEndAngles();
            GenerateSectorMesh();
            // UpdateCollider(meshFilter.mesh.vertices);
            ApplyMaterial(defaultMaterial);
        }
    }

    private void CalculateRadius() {
        innerRadius = ring.GetInnerRadius() / 100f * 0.5f;
        outerRadius = ring.GetOuterRadius() / 100f * 0.5f;
    }

    // ensure that the end angle is always larger for equal to start angle
    protected void AdjustEndAngles() {
        if (endAngle < startAngle) { endAngle = startAngle; }
        angleSize = Mathf.Abs(endAngle - startAngle);
    }

    // mesh generator, make sure it matches the circle
    protected virtual void GenerateSectorMesh() {
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.name = "ShortNoteMesh";

        int vertexCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleStep = angleSize / segments;

        for (int i = 0; i <= segments; i++) {
            float angle = (angleStep * i) * Mathf.Deg2Rad;
            vertices[i] = new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);
            vertices[i + segments + 1] = new Vector3(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius, 0);
        }

        for (int i = 0; i < segments; i++) {
            int innerIndex1 = i;
            int innerIndex2 = i + 1;
            int outerIndex1 = i + segments + 1;
            int outerIndex2 = i + segments + 2;

            int baseIndex = i * 6;

            triangles[baseIndex] = innerIndex1;
            triangles[baseIndex + 1] = outerIndex1;
            triangles[baseIndex + 2] = outerIndex2;

            triangles[baseIndex + 3] = innerIndex1;
            triangles[baseIndex + 4] = outerIndex2;
            triangles[baseIndex + 5] = innerIndex2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        // UpdateCollider(vertices);
    }

    // Update collider to match the new note size
    //protected virtual void UpdateCollider(Vector3[] vertices) {
    //    List<Vector2> points = new List<Vector2>();
    //    int halfCount = vertices.Length / 2;
    //    float minSpacing = 0.005f;

    //    polygonCollider.pathCount = 1;

    //    for (int i = halfCount; i < vertices.Length; i++) {
    //        Vector2 newPoint = new Vector2(vertices[i].x, vertices[i].y);
    //        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > minSpacing) {
    //            points.Add(newPoint);
    //        }
    //    }

    //    for (int i = halfCount - 1; i >= 0; i--) {
    //        Vector2 newPoint = new Vector2(vertices[i].x, vertices[i].y);
    //        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], newPoint) > minSpacing) {
    //            points.Add(newPoint);
    //        }
    //    }

    //    points.Add(points[0]);
    //    polygonCollider.SetPath(0, points.ToArray());
    //}

    protected virtual void ApplyMaterial(Material material) {
        meshRenderer.sharedMaterial = material;
    }

    // effect when player hits the note
    // WILL NEED TO ADD SCORE SYSTEM LATER +++++++++++++++++
    public virtual void TriggerHitEffect(ZoneType zoneType) {
        // Prevent duplicate triggers
        if (isHit) return;
        isHit = true;

        if (zoneType == ZoneType.Perfect) {
            ApplyMaterial(hitPerfectMaterial);
            InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNotePerfectHitSound);
        } else {
            ApplyMaterial(hitGoodMaterial);
            InGameAudioManager.Instance.PlaySound(InGameAudioManager.Instance.shortNoteGoodHitSound);
        }

        ScoreManager.Instance.AddHit(this.noteType, zoneType);

        if (TryGetComponent<Collider2D>(out Collider2D col)) {
            col.enabled = false;
        }

        Destroy(gameObject, 0.3f);
    }

    // effect when player missed the note
    // WILL NEED TO ADD HP SYSTEM LATER +++++++++++++++++
    public virtual void TriggerMissEffect() {
        // Prevent duplicate triggers
        if (isHit) return;
        isHit = true;

        // falling effect
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(0, -3f);

        ApplyMaterial(missMaterial);

        // Disable collider after hit
        if (TryGetComponent<Collider2D>(out Collider2D col)) {
            col.enabled = false;
        }

        Destroy(gameObject, 1.5f);
    }

    // Start angle behavior (will change to end angle if conterclockwise)
    protected virtual void AdjustAngleBasedOnClockwise() {
        transform.rotation = boardController.pointerController.GetRotationDirection() ?
            Quaternion.Euler(0, 0, -endAngle) : Quaternion.Euler(0, 0, startAngle);
    }

    // updates angle to the expected new angle
    public void ChangeAngle(float newStartAngle, float newEndAngle) {
        startAngle = newStartAngle;
        endAngle = newEndAngle;
        AdjustEndAngles();
        AdjustAngleBasedOnClockwise();
        GenerateSectorMesh();
        // UpdateCollider(meshFilter.mesh.vertices);
        ApplyMaterial(defaultMaterial);
    }

    // accessor of start angle
    public float GetStartAngle() {
        return startAngle;
    }

    // accessor of end angle
    public float GetEndAngle() {
        return endAngle;
    }

    public virtual bool IsHit() {
        return isHit;
    }

    public virtual void StopHolding() { }
    public virtual void OnHold() { Debug.Log("I got called FUCK"); }
    //public virtual void NoteEntered(ZoneType zoneType) { }
}
