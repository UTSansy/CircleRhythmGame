using UnityEngine;
using DG.Tweening;

public class SelectionUIManager : MonoBehaviour {
    public static SelectionUIManager Instance { get; private set; }

    private GameObject selectionUIRoot;
    private CanvasGroup canvasGroup;

    private void Awake() {
        if (Instance == null) {
            Instance = this;

            // Use self if not assigned
            if (selectionUIRoot == null) {
                selectionUIRoot = this.gameObject;
            }

            canvasGroup = selectionUIRoot.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = selectionUIRoot.AddComponent<CanvasGroup>();
            }
        } else {
            Destroy(gameObject);
        }
    }

    public void HideSelectionUI(float duration = 0.5f) {
        if (canvasGroup != null) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, duration).OnComplete(() => {
                selectionUIRoot.SetActive(false);
            });
        }
    }

    public void ShowSelectionUI(float duration = 0.5f) {
        if (canvasGroup != null) {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, duration).OnComplete(() => {
                selectionUIRoot.SetActive(true);
            });
        }
    }
}