using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeMenuCanvasBtn : MonoBehaviour, IPointerClickHandler
{
    // Создаем тип соыбтия которе может передавать 2 параметра
    [System.Serializable]
    public class OnClickEvent : UnityEvent<GameObject, GameObject> { }
    public GameObject showCanvas;
    public GameObject hideCanvas;
    public OnClickEvent onClick;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Press();
    }

    private void Press()
    {
        onClick?.Invoke(showCanvas, hideCanvas);
    }
}
