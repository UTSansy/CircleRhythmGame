using UnityEngine;

public class OptionCurving : MonoBehaviour {
    public float radius = 500f;
    public float arcAngle = 180f; // in degrees
    public bool autoUpdate = true;

    void LateUpdate() {
        if (autoUpdate) ApplyCurving();
    }

    public void ApplyCurving() {
        int count = transform.childCount;
        if (count == 0) return;

        float angleStep = arcAngle / (count - 1);
        float startAngle = -arcAngle / 2f;

        for (int i = 0; i < count; i++) {
            float angle = startAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius;
            Transform child = transform.GetChild(i);
            child.localPosition = pos;
            child.localRotation = Quaternion.Euler(0, 0, -angle); // optional: make text face the curve
        }
    }
}