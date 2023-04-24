using UnityEngine;
using UnityEngine.Events;

public class ColorToggleHandler : MonoBehaviour
{
    [SerializeField]
    private Material color;
    [System.Serializable]
    public class NewColorEvent : UnityEvent<Material> { } // Создаем тип события которе может передавать 1 параметр
    public NewColorEvent colorToggleOn; // Цветовой переключатель включен

    public void ColorToggleClick(bool toggleState)
    {
        if (toggleState)
        {
            colorToggleOn?.Invoke(color);
        }
    }
}
