using UnityEngine;

public class OptionsCarrier : MonoBehaviour {
    public void StartGame() {
        Debug.Log("start game");
        OptionSelector.Instance.onTransfer = true;
        LoadingUIController.Instance.ShowLoading();
        LoadingUIController.Instance.LoadSceneAsync("Song Selection Scene");
    }

    public void Settings() {
        Debug.Log("🛠️ Open Settings");
    }

    public void Exit() {
        Debug.Log("application quit");
        Application.Quit();
    }
}