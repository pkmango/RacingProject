using UnityEngine;

public class GarageCar : MonoBehaviour
{
    public ShopCar[] cars;
    [HideInInspector]
    public ShopCar playerCar;
    public PlayerData playerData;

    private void Awake() // Проблема
    {
        SetPlayerCar();
    }

    public void SetPlayerCar()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i] == null)
                break;

            if (i == playerData.CarPrefabNumber)
            {
                cars[i].GetComponent<Renderer>().material = playerData.GetCarMaterial();
                cars[i].gameObject.SetActive(true);
                playerCar = cars[i];
            }
            else
            {
                cars[i].gameObject.SetActive(false);
            }
        }
    }
}
