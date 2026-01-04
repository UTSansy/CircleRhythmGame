using UnityEngine;
using TMPro;

public class ScoreDisplayUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText;

    void Update() {
        if (ScoreManager.Instance != null) {
            // scoreText.text = $"Score: {Mathf.RoundToInt(ScoreManager.Instance.GetScore())}, DEBUG: {ScoreManager.Instance.debugScore}";
             scoreText.text = $"Score: {Mathf.RoundToInt(ScoreManager.Instance.GetScore())}";
        }
    }
}