using UnityEngine;
using DG.Tweening;

public class SongSelectorSwitcher : MonoBehaviour {
    public GameObject builtInSelector;
    public GameObject userSelector;

    public float switchAnimDuration = 0.4f;

    private bool isShowingBuiltIn = true;
    private Tween currentTween;
    private GameObject lastFromObj;  // track which one is shrinking

    void Start() {
        builtInSelector.SetActive(true);
        userSelector.SetActive(false);

        builtInSelector.transform.localScale = Vector3.one;
        userSelector.transform.localScale = new Vector3(0f, 1f, 1f);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha2)) {

            CircularSongSelector activeSelector = isShowingBuiltIn
                ? builtInSelector.GetComponent<CircularSongSelector>()
                : userSelector.GetComponent<CircularSongSelector>();

            if (activeSelector.GetSwitchingStatus()) {
                return;
            }

            // ✅ Play sound here, when key is pressed
            if (SelectionAudioManager.Instance != null) {
                SelectionAudioManager.Instance.PlayScrollSFX();  // or PlaySwitchTabSFX() if you have a custom one
            }

            SwitchWithOverlapFix();
        }
    }

    void SwitchWithOverlapFix() {
        if (currentTween != null && currentTween.IsActive()) {
            currentTween.Kill(); // cancel previous in-progress animation
        }

        GameObject fromObj = isShowingBuiltIn ? builtInSelector : userSelector;
        GameObject toObj = isShowingBuiltIn ? userSelector : builtInSelector;

        RectTransform from = fromObj.transform as RectTransform;
        RectTransform to = toObj.transform as RectTransform;

        // Set the new one active first
        toObj.SetActive(true);
        to.localScale = new Vector3(0f, 1f, 1f);

        lastFromObj = fromObj; // record the last "outgoing" object

        // Shrink current (fromObj)
        from.DOScaleX(0f, switchAnimDuration).SetEase(Ease.InBack)
            .OnComplete(() => {
                // only disable if it wasn't switched again during the animation
                if (lastFromObj == fromObj) {
                    fromObj.SetActive(false);
                }
            });

        // Grow new (toObj)
        currentTween = to.DOScaleX(1f, switchAnimDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                CircularSongSelector selector = toObj.GetComponent<CircularSongSelector>();
                selector.PlayAudioAtIndex(selector.GetCurrentAudioIndex(), true);
            });

        isShowingBuiltIn = !isShowingBuiltIn;
    }
}