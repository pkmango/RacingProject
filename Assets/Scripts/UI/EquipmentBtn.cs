using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentBtn : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UpgradeType upgradeType;

    [System.Serializable]
    private class OnClickEvent : UnityEvent<UpgradeType, Button> { }
    [SerializeField]
    private OnClickEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(upgradeType, GetComponent<Button>());
    }
}
