using System;
using UnityEngine;
using UnityEngine.UI;

public class BuyingCar : MonoBehaviour
{
    [SerializeField]
    private GarageCar garageCar;
    [SerializeField]
    private Text carName, carPrice;
    [SerializeField]
    private Material defaultColor; // Должен совпадать с цветом включенного по умолчанию Toogle

    private Material currentColor;
    private ShopCar currentCar;
    private int nextIndex = 0; // Индекс следющего элемента массива авто после нажатия на стрелку
    private PlayerData playerData;

    private void Awake()
    {
        playerData = garageCar.playerData;
    }

    private void OnEnable()
    {
        nextIndex = 0;
        currentColor = defaultColor;

        garageCar.playerCar?.gameObject.SetActive(false);

        ShowNextCar();
    }

    public void ChangeCarColor(Material newColor)
    {
        if(newColor != currentColor)
            currentColor = newColor;

        currentCar.GetComponent<Renderer>().material = currentColor;
    }

    public void LeftArrowCick()
    {
        if (--nextIndex < 0)
            nextIndex = garageCar.cars.Length - 1;

        ShowNextCar();
    }

    public void RightArrowClick()
    {
        if (++nextIndex > garageCar.cars.Length - 1)
            nextIndex = 0;

        ShowNextCar();
    }

    private void ShowNextCar()
    {
        currentCar?.gameObject.SetActive(false);

        currentCar = garageCar.cars[nextIndex];
        ChangeCarColor(currentColor);
        currentCar.gameObject.SetActive(true);
        carName.text = currentCar.carName;
        carPrice.text = currentCar.carPrice.ToString("N0") + " $";
    }

    public void BuyCar()
    {
        playerData.CarPrefabNumber = nextIndex;

        int colorIndex = Array.IndexOf(playerData.carMaterials, currentColor); // Если меньше нуля - цвет не найден
        if (colorIndex >= 0)
            playerData.CarColorNumber = colorIndex;
        else
            Debug.Log("Цвет " + currentColor + " не найден в массиве  playerData.carMaterials");
    }

    private void OnDisable()
    {
        garageCar.SetPlayerCar();
    }
}
