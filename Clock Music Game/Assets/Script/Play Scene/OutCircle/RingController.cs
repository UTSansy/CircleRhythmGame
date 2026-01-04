using UnityEngine;

public class RingController : MonoBehaviour {
    public Material ringMaterial;

    void Awake() {
        InitializeMaterial();
    }

#if UNITY_EDITOR
    void OnValidate() {
        ringMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
    }
#endif

    private void InitializeMaterial() {
        if (ringMaterial == null) {
            ringMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
        }
    }

    public float GetInnerRadius() {
        return ringMaterial != null ? ringMaterial.GetFloat("_InnerRadius") : 85f;
    }

    public float GetOuterRadius() {
        return ringMaterial != null ? ringMaterial.GetFloat("_OuterCutoff") : 95f;
    }
}