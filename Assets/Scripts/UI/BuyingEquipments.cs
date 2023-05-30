using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyingEquipments : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private PlayerData playerData;
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

    private Button currentBtn; // Текущая выделенная кнопка
    private bool allowedToBuy = true; // Если денег на покупку не хватает, меняем на false
    private Image buyBtnImg;

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

    public void ShowEquipDescription(UpgradeType upgradeType, Button selectedBtn)
    {
        buyBtn.gameObject.SetActive(true);
        equipPrice.gameObject.SetActive(true);
        currentBtn = selectedBtn;

        switch (upgradeType)
        {
            case UpgradeType.NOS:
                equipText.text = "Nitrous";
                equipPrice.text = nitrousUpPrice.ToString("N0") + " $";
                PurchasePossibilityCheck(nitrousUpPrice);
                break;
            case UpgradeType.Engine:
                equipText.text = "Engine";
                equipPrice.text = engineUpPrice.ToString("N0") + " $";
                PurchasePossibilityCheck(engineUpPrice);
                break;
            case UpgradeType.Armor:
                equipText.text = "Armor";
                equipPrice.text = armorUpPrice.ToString("N0") + " $";
                PurchasePossibilityCheck(armorUpPrice);
                break;
            case UpgradeType.Ammo:
                equipText.text = "Ammo";
                equipPrice.text = ammoUpPrice.ToString("N0") + " $";
                PurchasePossibilityCheck(ammoUpPrice);
                break;
            case UpgradeType.Mines:
                equipText.text = "Mines";
                equipPrice.text = minesUpPrice.ToString("N0") + " $";
                PurchasePossibilityCheck(minesUpPrice);
                break;
            default:
                Debug.Log("No handler found for " + upgradeType);
                break;
        }

        void PurchasePossibilityCheck(int price)
        {
            if (playerData.Money < price)
            {
                allowedToBuy = false;
                buyBtnImg.color = Color.grey;
                buyBtnTxt.color = Color.grey;
            }
            else
            {
                allowedToBuy = true;
                buyBtnImg.color = Color.white;
                buyBtnTxt.color = Color.white;
            }
        }
    }

    public void BuyEquipment()
    {
        if (allowedToBuy)
        {
            Debug.Log("Покупка совершена!");
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
