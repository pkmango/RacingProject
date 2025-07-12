using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
public class ColorToggleHandler : MonoBehaviour
{
    [SerializeField]
    private Material color;
    [System.Serializable]
    public class NewColorEvent : UnityEvent<Material> { } // Создаем тип события которе может передавать 1 параметр
    public NewColorEvent toggleOn; // Цветовой переключатель включен

    private Toggle thisToggle;
    private bool defaultState;

    // При использовании Toggle Group будут повторные срабатывания OnValueChanged без реальной смены значения thisToggle.isOn, чтобы это исправить добавляем currentState
    private bool currentState;

    private void Awake()
    {
        thisToggle = GetComponent<Toggle>();
        defaultState = thisToggle.isOn;
    }

    private void OnEnable()
    {
        // При каждой активации canvas сбрасываем состояние переключателя к начальному
        thisToggle.isOn = defaultState;
        currentState = defaultState;
    }

    public void ColorToggleClick(bool toggleState)
    {

        if (toggleState && !currentState)
        {
            toggleOn?.Invoke(color);
        }

        currentState = toggleState;
    }
}
