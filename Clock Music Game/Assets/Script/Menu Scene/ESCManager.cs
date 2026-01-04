using UnityEngine;
using DG.Tweening;

public class ESCManager : MonoBehaviour {
    public static ESCManager Instance { get; private set; }

    [Header("ESC Hold Settings")]
    public float holdThreshold = 3f;
    public float minHoldTime = 0.3f;

    [Header("Hold Hint Fade")]
    public CanvasGroup holdHintGroup;
    public float fadeDuration = 0.3f;

    private float holdTime = 0f;
    private bool mightHolding = false;
    private bool triggered = false;
    private bool isHolding = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); // Only allow one instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Survive scene changes
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !isHolding && !mightHolding) {
            mightHolding = true;
            holdTime = 0f;
            triggered = false;
            isHolding = false;

            ShowHoldHint();
        }

        if (mightHolding) {
            holdTime += Time.deltaTime;

            if (holdTime >= minHoldTime && !isHolding) {
                isHolding = true;
                Debug.Log("Hold ESC started");
            }

            if (isHolding && holdTime >= holdThreshold && !triggered) {
                triggered = true;
                Debug.Log("Quitting game...");
                Application.Quit();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape)) {
            mightHolding = false;
            holdTime = 0f;
            triggered = false;
            isHolding = false;

            HideHoldHint();
        }
    }

    private void ShowHoldHint() {
        if (holdHintGroup != null) {
            holdHintGroup.DOKill();
            holdHintGroup.alpha = 0f;
            holdHintGroup.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);
        }
    }

    private void HideHoldHint() {
        if (holdHintGroup != null) {
            holdHintGroup.DOKill();
            holdHintGroup.DOFade(0f, fadeDuration).SetEase(Ease.InQuad);
        }
    }
}