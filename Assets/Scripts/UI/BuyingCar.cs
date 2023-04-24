using UnityEngine;
using UnityEngine.UI;

public class BuyingCar : MonoBehaviour
{
    [SerializeField]
    private ShopCar[] carList; // Массив с доступными для покупки авто
    [SerializeField]
    private GameObject playerCar;
    [SerializeField]
    private Text carName, carPrice;
    [SerializeField]
    private Material currentCarColor;

    private ShopCar currentCar;
    private int nextIndex = 0; // Индекс следющего элемента массива авто после нажатия на стрелку

    private void OnEnable()
    {
        ShowNextCar(playerCar);
    }

    public void ChangeCarColor(Material newColor)
    {
        if(newColor != currentCarColor)
            currentCarColor = newColor;

        currentCar.GetComponent<Renderer>().material = currentCarColor;
    }

    public void LeftArrowCick()
    {
        if (--nextIndex < 0)
            nextIndex = carList.Length - 1;

        ShowNextCar(currentCar.gameObject);
    }

    public void RightArrowClick()
    {
        if (++nextIndex > carList.Length - 1)
            nextIndex = 0;

        ShowNextCar(currentCar.gameObject);
    }

    private void ShowNextCar(GameObject objectToHide)
    {
        objectToHide.SetActive(false); // Прячем текущий авто, при первом запуске это машина игрока
        currentCar = carList[nextIndex];
        ChangeCarColor(currentCarColor);
        currentCar.gameObject.SetActive(true);
        carName.text = currentCar.carName;
        carPrice.text = currentCar.carPrice.ToString("N0") + " $";
    }

    private void OnDisable()
    {
        //nextIndex = 0;
        
        if (currentCar != null)
            currentCar.gameObject.SetActive(false);

        if (playerCar != null)
            playerCar.SetActive(true);
    }
}
