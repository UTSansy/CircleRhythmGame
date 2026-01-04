//using UnityEngine;
//using System.Collections.Generic;
//using TMPro;
//using DG.Tweening;

//public class OptionMenuManager : MonoBehaviour {
//    public RectTransform container;               // Parent for options
//    public List<RectTransform> optionButtons;     // Option buttons
//    public float arcAngle = 150f;                 // Arc spread in degrees
//    public float radius = 11.9f;                  // Based on board size (7 x 1.7)
//    public float verticalOffset = 1f;            // Offset from board
//    public float appearDuration = 0.5f;           // Fade in time

//    private int selectedIndex = 0;
//    private bool inputEnabled = false;

//    void Start() {
//        ArrangeOptions();
//        HideAllOptions();
//    }

//    public void ActivateMenu() {
//        ShowOptions();
//        inputEnabled = true;
//    }

//    void Update() {
//        if (!inputEnabled) return;

//        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
//            selectedIndex = (selectedIndex - 1 + optionButtons.Count) % optionButtons.Count;
//            ArrangeOptions();
//        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
//            selectedIndex = (selectedIndex + 1) % optionButtons.Count;
//            ArrangeOptions();
//        }

//        if (Input.GetKeyDown(KeyCode.Return)) {
//            MenuOption option = optionButtons[selectedIndex].GetComponent<MenuOption>();
//            option?.TriggerOption();
//        }
//    }

//    private void ArrangeOptions() {
//        float angleStep = arcAngle / (optionButtons.Count - 1);
//        float startAngle = -arcAngle / 2f;

//        for (int i = 0; i < optionButtons.Count; i++) {
//            float angle = startAngle + angleStep * i;
//            float rad = angle * Mathf.Deg2Rad;
//            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
//            pos.y += verticalOffset;

//            optionButtons[i].anchoredPosition = pos;
//            optionButtons[i].DOScale(i == selectedIndex ? 1.2f : 1f, 0.3f);

//            Debug.Log($"[{i}] pos = {pos}, angle = {angle}");
//        }
//    }

//    private void HideAllOptions() {
//        foreach (var btn in optionButtons) {
//            CanvasGroup group = btn.GetComponent<CanvasGroup>();
//            if (!group) group = btn.gameObject.AddComponent<CanvasGroup>();
//            group.alpha = 0f;
//        }
//    }

//    private void ShowOptions() {
//        foreach (var btn in optionButtons) {
//            CanvasGroup group = btn.GetComponent<CanvasGroup>();
//            group.DOFade(1f, appearDuration);
//        }
//        ArrangeOptions();
//    }


//}


using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class OptionMenuManager : MonoBehaviour {
    [Header("Setup")]
    public RectTransform container;               // Usually OptionManager's own RectTransform
    public List<RectTransform> optionButtons;     // Option buttons (child objects)

    [Header("Layout Settings")]
    public float arcAngle = 150f;                 // Arc spread in degrees
    public float radius = 11.9f;                  // Logical radius (matching board)
    public float verticalOffset = 1f;             // How far above the center of the board
    public float uiScaleFactor = 100f;            // Scale UI distance (convert to pixels)

    [Header("Visuals")]
    public float appearDuration = 0.5f;           // Fade in duration
    public float scaleSelected = 1.2f;            // Selected option scale
    public float scaleNormal = 1f;                // Unselected option scale
    public bool rotateToArc = false;              // Optional: rotate to face curve

    private int selectedIndex = 0;
    private bool inputEnabled = false;

    void Start() {
        if (container == null)
            container = GetComponent<RectTransform>();

        ArrangeOptions();
        HideAllOptions();
    }

    public void ActivateMenu() {
        ShowOptions();
        inputEnabled = true;
    }

    void Update() {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            selectedIndex = (selectedIndex - 1 + optionButtons.Count) % optionButtons.Count;
            ArrangeOptions();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            selectedIndex = (selectedIndex + 1) % optionButtons.Count;
            ArrangeOptions();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            MenuOption option = optionButtons[selectedIndex].GetComponent<MenuOption>();
            option?.TriggerOption();
        }
    }

    private void ArrangeOptions() {
        float angleStep = arcAngle / (optionButtons.Count - 1);
        float startAngle = -arcAngle / 2f;

        for (int i = 0; i < optionButtons.Count; i++) {
            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius * uiScaleFactor;
            pos.y += verticalOffset * uiScaleFactor;

            optionButtons[i].anchoredPosition = pos;
            optionButtons[i].DOScale(i == selectedIndex ? scaleSelected : scaleNormal, 0.3f);

            if (rotateToArc) {
                optionButtons[i].localRotation = Quaternion.Euler(0, 0, angle);
            } else {
                optionButtons[i].localRotation = Quaternion.identity;
            }

            Debug.Log($"[{i}] pos = {pos}, angle = {angle}");
        }
    }

    private void HideAllOptions() {
        foreach (var btn in optionButtons) {
            CanvasGroup group = btn.GetComponent<CanvasGroup>();
            if (!group) group = btn.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
        }
    }

    private void ShowOptions() {
        foreach (var btn in optionButtons) {
            CanvasGroup group = btn.GetComponent<CanvasGroup>();
            group.DOFade(1f, appearDuration);
        }
        ArrangeOptions();
    }
}