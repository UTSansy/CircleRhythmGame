using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class OptionSelector : MonoBehaviour {
    public static OptionSelector Instance { get; private set; }

    public RectTransform container;                 // Moves all option entries
    public GameObject optionPrefab;                 // Prefab to instantiate
    public List<MenuOption> actualMenu;             // Labels for options

    public int visibleCount = 3;                    // Number of options visibly centered
    public int bufferCount = 2;                     // Offscreen buffer
    public float entrySpacing = 300f;               // Horizontal distance between options
    public float animationDuration = 0.3f;

    private List<RectTransform> entryObjects = new List<RectTransform>();
    private List<string> visualList = new List<string>();

    private int selectedIndex = 0;
    private int pendingScroll = 0;

    private int firstVisibleIndex;

    private int holdDirection;

    public bool onTransfer;
    private bool isSwitching;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        InitializeEntries();
        InitializeVisualList();
        UpdateEntryVisuals();
    }

    public void Update() {
        if (!onTransfer) {
            HandleInput();
        }
    }

    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            holdDirection = -1;
            pendingScroll += holdDirection;
            Scroll(holdDirection);
            MenuAudioManager.Instance?.PlayScrollSFX();
            isSwitching = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            holdDirection = 1;
            pendingScroll += holdDirection;
            Scroll(holdDirection);
            MenuAudioManager.Instance?.PlayScrollSFX();
            isSwitching = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && holdDirection == -1) EndHold();
        if (Input.GetKeyUp(KeyCode.RightArrow) && holdDirection == 1) EndHold();

        // CONFRIM USE 1 FIRST
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (!isSwitching) {
                MenuAudioManager.Instance.PlayConfirmSFX();
                actualMenu[selectedIndex].TriggerOption();
            }
        }
    }

    public void EndHold() {
        int newIndex = selectedIndex + pendingScroll;
        if (newIndex >= actualMenu.Count) newIndex -= actualMenu.Count;
        if (newIndex < 0) newIndex += actualMenu.Count;

        selectedIndex = newIndex;
        pendingScroll = 0;
        InitializeVisualList();
        UpdateEntryVisuals();
        isSwitching = false;
    }

    private void InitializeVisualList() {
        int totalCount = visibleCount + bufferCount * 2;
        int centerIndex = totalCount / 2;

        visualList.Clear();

        for (int i = 0; i < totalCount; i++) {
            int visualIndex = selectedIndex - centerIndex + i;
            while (visualIndex < 0) { visualIndex += actualMenu.Count; }
            while (visualIndex >= actualMenu.Count) { visualIndex -= actualMenu.Count; }
            visualList.Add(actualMenu[visualIndex].optionName);

            if (i == 0) { firstVisibleIndex = visualIndex; }
        }
    }

    void InitializeEntries() {
        int totalCount = visibleCount + bufferCount * 2;
        for (int i = 0; i < totalCount; i++) {
            GameObject entry = Instantiate(optionPrefab, container);
            RectTransform rect = entry.GetComponent<RectTransform>();
            entryObjects.Add(rect);
        }
    }

    private void Scroll(int direction) {
        foreach (var entry in entryObjects) entry.DOKill();
        RotateList(direction);

        for (int i = 0; i < entryObjects.Count; i++) {
            RectTransform entry = entryObjects[i];
            float duration = animationDuration;
            Vector2 target = new Vector2((i - bufferCount) * entrySpacing, 0);
            entry.DOAnchorPosX(target.x, duration).SetEase(Ease.OutExpo);
        }
    }

    private void RotateList(int direction) {
        if (direction == -1) {
            var last = entryObjects[entryObjects.Count - 1];
            entryObjects.RemoveAt(entryObjects.Count - 1);
            entryObjects.Insert(0, last);
        } else {
            var first = entryObjects[0];
            entryObjects.RemoveAt(0);
            entryObjects.Add(first);
        }
    }

    private void UpdateEntryVisuals() {
        for (int i = 0; i < entryObjects.Count; i++) {
            RectTransform entry = entryObjects[i];
            int menuIndex = (firstVisibleIndex + i) % actualMenu.Count;

            entry.anchoredPosition = new Vector2((i - bufferCount) * entrySpacing, 0);
            TMP_Text text = entry.GetComponent<TMP_Text>();
            text.text = actualMenu[menuIndex].optionName;

            bool isCenter = (i == (visibleCount + bufferCount * 2) / 2);
            float alpha = Mathf.Lerp(0.3f, 1f, 1f - Mathf.Abs(i - (visibleCount / 2)) / (visibleCount / 2f));
            float scale = isCenter ? 1.3f : 1f;

            text.fontSize = isCenter ? 50 : 36;
            text.color = Color.yellow;
            text.transform.localScale = Vector3.one * scale;
        }
    }
}