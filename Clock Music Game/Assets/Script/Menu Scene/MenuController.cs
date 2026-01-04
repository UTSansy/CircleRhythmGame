using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MenuController : MonoBehaviour {
    public GameObject menuBoardPrefab;
    public CanvasGroup menuOptions;
    public float targetScale = 1.0f;
    public float moveDownY = -3f;
    public float growDuration = 2f;
    public float moveDuration = 5f;
    public float spinRounds = 2f;

    private GameObject menuBoard;
    public OptionMenuManager menuManager;


    public void Start() {
        StartCoroutine(PlayIntroAnimation());
    }

    public IEnumerator PlayIntroAnimation() {
        menuBoard = Instantiate(menuBoardPrefab);
        menuBoard.transform.localScale = Vector3.zero;
        menuBoard.transform.rotation = Quaternion.identity;

        float totalDuration = growDuration + moveDuration;

        var growTween = menuBoard.transform.DOScale(targetScale, growDuration)
            .SetEase(Ease.OutBack);

        menuBoard.transform
            .DORotate(new Vector3(0, 0, -spinRounds * 360f), totalDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart); // smooth deceleration

        yield return growTween.WaitForCompletion();

        Vector3 endPos = new Vector3(menuBoard.transform.position.x, moveDownY, menuBoard.transform.position.z);
        yield return menuBoard.transform.DOMove(endPos, moveDuration)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        //  menuManager.ActivateMenu();
    }
}
