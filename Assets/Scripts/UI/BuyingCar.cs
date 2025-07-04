﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyingCar : MonoBehaviour
{
    [SerializeField]
    private GarageCar garageCar;
    [SerializeField]
    private Text carName, carPrice;
    [SerializeField]
    private Material defaultColor; // Должен совпадать с цветом включенного по умолчанию Toogle
    [SerializeField]
    private Button buyBtn;
    [SerializeField]
    private Text buyBtnTxt;
    public UnityEvent moneyHasChanged;
    [SerializeField]
    private GameObject vfx;

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

        // Проверка условий для покупки и установка режима отображения кнопки "Buy"
        SetBuyButton(!PlyerCarComparison() && playerData.Money >= currentCar.carPrice);
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
        currentCar.gameObject.SetActive(true);
        carName.text = currentCar.carName;

        ChangeCarColor(currentColor);
    }


    // Сравнение с машиной игрока и установка отображения цены
    private bool PlyerCarComparison()
    {
        // Узнаем равны ли отображаемая машина и машина игрока
        bool equality = playerData.CarPrefabNumber == nextIndex && currentColor == playerData.GetCarMaterial();

        if (equality)
        {
            carPrice.text = "0 $";
        }
        else
        {
            carPrice.text = currentCar.carPrice.ToString("N0") + " $";
        }
        
        return equality;
    }

    private void SetBuyButton(bool activate)
    {
        if (activate)
        {
            buyBtn.interactable = true;
            buyBtnTxt.color = Color.white;
        }
        else
        {
            buyBtn.interactable = false;
            buyBtnTxt.color = Color.grey;
        }
    }

    public void BuyCar()
    {
        //При покупке новой машины нужно сбросить все апгрейды на 0
        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
        {
            playerData.SetUpgradeLvl(upgradeType, 0);
        }

        if (playerData.Money < currentCar.carPrice) // Избыточно?
            return;

        playerData.CarPrefabNumber = nextIndex;
        playerData.Money -= currentCar.carPrice;
        moneyHasChanged?.Invoke();

        int colorIndex = Array.IndexOf(playerData.carMaterials, currentColor); // Если меньше нуля - цвет не найден
        if (colorIndex >= 0)
            playerData.CarColorNumber = colorIndex;
        else
            Debug.Log($"Цвет {currentColor} не найден в массиве  playerData.carMaterials");

        Instantiate(vfx);
        SetBuyButton(false);
        carPrice.text = "0 $";


    }

    private void OnDisable()
    {
        garageCar.SetPlayerCar();
    }
}
