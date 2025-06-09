using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyingEquipments : MonoBehaviour, IPointerClickHandler
{
    public PlayerData playerData;
    [SerializeField]
    private Text equipText, equipPrice, buyBtnTxt;
    [SerializeField]
    private Button buyBtn;

    [Header("Upgrade Prices")]
    [SerializeField, Min(0)]
    private int nitrousUpPrice = 10000;
    [SerializeField, Min(0)]
    private int engineUpPrice = 10000;
    [SerializeField, Min(0)]
    private int armorUpPrice = 10000;
    [SerializeField, Min(0)]
    private int ammoUpPrice = 10000;
    [SerializeField, Min(0)]
    private int minesUpPrice = 10000;
    private int currentUpPrice;

    private Button currentBtn; // Текущая выделенная кнопка
    private UpgradeType currentUpgradeType; // Текущий выделленный тип апгрейда
    private bool allowedToBuy = true; // Если денег на покупку не хватает, меняем на false
    private Image buyBtnImg;

    [System.Serializable]
    private class OnClickEvent : UnityEvent<UpgradeType> { }
    [SerializeField]
    private OnClickEvent newUpgradeLvl; // Событие срабатывает покупке нового апгрейда
    [SerializeField]
    private UnityEvent moneyHasChanged; // Событие срабатывает при изменении количества денег

    private void Awake()
    {
        buyBtnImg = buyBtn.GetComponent<Image>();
    }

    private void OnEnable()
    {
        NothingPressed();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        NothingPressed();
    }

    private void NothingPressed()
    {
        equipText.text = "Choose the required upgrade";
        equipPrice.gameObject.SetActive(false);
        buyBtn.gameObject.SetActive(false);
    }

    // Метод добавляется через редактор в EquipmentBtn в качестве слушателя на событие клика по иконке эквипмента
    public void ShowEquipDescription(UpgradeType upgradeType, Button selectedBtn)
    {
        buyBtn.gameObject.SetActive(true);
        equipPrice.gameObject.SetActive(true);
        currentBtn = selectedBtn;
        currentUpgradeType = upgradeType;

        switch (upgradeType)
        {
            case UpgradeType.NOS:
                SetTextAndPrice("Nitrous", nitrousUpPrice);
                break;
            case UpgradeType.Engine:
                SetTextAndPrice("Engine", engineUpPrice);
                break;
            case UpgradeType.Armor:
                SetTextAndPrice("Armor", armorUpPrice);
                break;
            case UpgradeType.Ammo:
                SetTextAndPrice("Ammo", ammoUpPrice);
                break;
            case UpgradeType.Mines:
                SetTextAndPrice("Mines", minesUpPrice);
                break;
            default:
                Debug.Log("No handler found for " + upgradeType);
                break;
        }

        void SetTextAndPrice(string txt, int upPrice)
        {
            equipText.text = txt;
            equipPrice.text = upPrice.ToString("N0") + " $";
            currentUpPrice = upPrice;
            PurchasePossibilityCheck(upPrice);
        }
    }

    private void PurchasePossibilityCheck(int price)
    {
        if (playerData.Money >= price && playerData.GetUpgradeLvl(currentUpgradeType) < 3)
        {
            allowedToBuy = true;
            buyBtnImg.color = Color.white;
            buyBtnTxt.color = Color.white;
        }
        else
        {
            allowedToBuy = false;
            buyBtnImg.color = Color.grey;
            buyBtnTxt.color = Color.grey;
        }
    }

    public void BuyEquipment()
    {
        if (allowedToBuy)
        {
            int newLvl = playerData.GetUpgradeLvl(currentUpgradeType) + 1;

            playerData.SetUpgradeLvl(currentUpgradeType, newLvl);
            newUpgradeLvl?.Invoke(currentUpgradeType);
            playerData.Money -= currentUpPrice;
            moneyHasChanged?.Invoke();
            PurchasePossibilityCheck(currentUpPrice);
        }
        
        currentBtn.Select();
    }
}

public enum UpgradeType
{
    NOS,
    Engine,
    Armor,
    Ammo,
    Mines
}
