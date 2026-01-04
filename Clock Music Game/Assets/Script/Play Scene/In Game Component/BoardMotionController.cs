using UnityEngine;

public class BoardMotionController : MonoBehaviour {

    public enum ScaleMode { Auto, Input, Sound }
    [Header("Scaling Mode")]
    public ScaleMode currentMode = ScaleMode.Auto;

    [Header("Auto")]
    public float scaleSpeed = 1f;
    public float maxScale = 1.2f;
    public float minScale = 0.8f;

    [Header("Manual Input")]
    public float manualScaleSpeed = 0.05f;
    public float moveSpeed = 5f;

    [Header("Sound Mode")]
    public float soundSensitivity = 5f;
    public int frequencyBand = 0;

    private Vector3 originalScale;
    private float currentScaleFactor = 1f;

    void Start() {
        originalScale = transform.localScale;
    }

    void Update() {
        // Handle movement (only in Input Mode)
        if (currentMode == ScaleMode.Input) { HandleManualMovement(); }

        // Handle Scaling based on the selected mode
        switch (currentMode) {
            case ScaleMode.Auto:
                AutoBehavior();
                break;
            case ScaleMode.Input:
                ManualBehavior();
                break;
            case ScaleMode.Sound:
                SoundBehavior();
                break;
        }

        // Apply scaling
        transform.localScale = originalScale * currentScaleFactor;
    }

    // ✅ Toggles between different scale modes
    public void SetScaleMode(ScaleMode mode) { currentMode = mode; }

    // ✅ Handles Auto Scaling (sinusoidal pulsing effect)
    private void AutoBehavior() {
        currentScaleFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * scaleSpeed) + 1) / 2);
    }

    // ✅ Handles Manual Scaling (Player Input)
    private void ManualBehavior() {
        if (Input.GetKey(KeyCode.UpArrow))
            currentScaleFactor += manualScaleSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            currentScaleFactor -= manualScaleSpeed * Time.deltaTime;
    }

    // ✅ Handles Scaling Based on Sound (Dynamic Scaling)
    private void SoundBehavior() {
        if (InGameAudioManager.Instance == null || !InGameAudioManager.Instance.isMusicReady) return;

        // Get real-time amplitude from AudioManager
        float amplitude = InGameAudioManager.Instance.GetMusicAmplitude(frequencyBand) * soundSensitivity;

        // Scale the value within minScale & maxScale
        currentScaleFactor = Mathf.Lerp(minScale, maxScale, amplitude);
    }

    // ✅ Handles Manual Movement (Only for Input Mode)
    private void HandleManualMovement() {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(moveX, moveY, 0);
    }

    // ✅ Directly Set Position
    public void SetPosition(Vector2 vec) {
        transform.position = vec;
    }
}