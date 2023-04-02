using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ChangeMenuCanvasBtn : MonoBehaviour, IPointerClickHandler
{
    // Создаем тип события которе может передавать 2 объекта,
    // тот что нужно показать, и тот что нужно скрыть
    [System.Serializable]
    private class OnClickEvent : UnityEvent<GameObject, GameObject> { }
    [SerializeField]
    private GameObject showCanvas, hideCanvas;
    [SerializeField]
    private OnClickEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        onClick?.Invoke(showCanvas, hideCanvas);
    }
}
