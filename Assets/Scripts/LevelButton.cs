using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable] // Создаем тип события которе может передавать имя нужного уровня
    private class OnClickEvent : UnityEvent<string> { }
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private OnClickEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        onClick?.Invoke(sceneName);
    }
}
