using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentBtn : MonoBehaviour, IPointerClickHandler
{
    public UpgradeType upgradeType;
    [SerializeField]
    private UpgradeLvlIndicator upgradeLvlIndicator;
    private PlayerData playerData;

    [System.Serializable]
    private class OnClickEvent : UnityEvent<UpgradeType, Button> { }
    [SerializeField]
    private OnClickEvent onClick;

    private void Awake()
    {
        playerData = GetComponentInParent<BuyingEquipments>().playerData;
    }

    private void OnEnable()
    {
        upgradeLvlIndicator.SetLvlIndicator(playerData.GetUpgradeLvl(upgradeType));
    }

    public void SetLvlIndicator(UpgradeType updatedUpgradeType)
    {
        if (updatedUpgradeType == upgradeType)
        {
            upgradeLvlIndicator.SetLvlIndicator(playerData.GetUpgradeLvl(upgradeType));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(upgradeType, GetComponent<Button>());
    }
}
