using UnityEngine;

public class BuyingCar : MonoBehaviour
{
    [SerializeField]
    private GameObject[] carList;
    [SerializeField]
    private GameObject playerCar;

    public Vector3 carPosition;
    public Quaternion carRotation;

    private GameObject currentCar;

    private void OnEnable()
    {
        playerCar.SetActive(false);
        currentCar = Instantiate(carList[0], carPosition, carRotation);
    }

    private void OnDisable()
    {
        if(playerCar != null)
            playerCar.SetActive(true);
    }
}
