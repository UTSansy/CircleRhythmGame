using DG.Tweening;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingUIController : MonoBehaviour {
    public static LoadingUIController Instance { get; private set; }

    private CanvasGroup canvasGroup;

    [Header("UI References")]
    public TMP_Text loadingText;
    private Coroutine dotCoroutine;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
            canvasGroup = GetComponent<CanvasGroup>();

            // 🔒 Start invisible and non-blocking
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowLoading() {
        Debug.Log("in show load");
        canvasGroup.DOFade(1f, 0.1f);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        // Start dot animation
        if (dotCoroutine != null) StopCoroutine(dotCoroutine);
        dotCoroutine = StartCoroutine(AnimateLoadingDots());
    }

    public void HideLoading(float hideTime = 1.5f) {
        canvasGroup.DOFade(0f, hideTime);
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        if (dotCoroutine != null) StopCoroutine(dotCoroutine);
        // gameObject.SetActive(false);
    }

    private IEnumerator AnimateLoadingDots() {
        string baseText = "Loading";
        int dotCount = 0;

        while (true) {
            dotCount = (dotCount + 1) % 4;  // 0 to 3
            string dots = new string('.', dotCount);
            loadingText.text = baseText + dots;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void LoadSceneAsync(string sceneName) {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Optional: wait until it's mostly done
        while (asyncLoad.progress < 0.9f) {
            yield return null;
        }

        // Wait a tiny bit more if you want, or show 100% progress
        yield return new WaitForSeconds(3f);

        // 🔁 Finally activate scene
        asyncLoad.allowSceneActivation = true;
    }
}
