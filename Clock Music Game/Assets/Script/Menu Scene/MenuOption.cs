using UnityEngine;
using UnityEngine.Events;

public class MenuOption : MonoBehaviour {
    public string optionName;
    public UnityEvent onSelect;

    public void TriggerOption() {
        onSelect?.Invoke();
    }
}