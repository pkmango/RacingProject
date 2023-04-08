using UnityEngine;

//[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/PlayerData")]
public class PlayerData : MonoBehaviour
{
    //private int carPrefabNumber;
    //private int carColorNumber;

    public GameObject[] playerCars;

    const string CarPrefabNumberKey = "CarPrefabNumber";
    const string CarColorNumberKey = "CarColorNumber";

    public int CarPrefabNumber
    {
        get
        {
            //return carPrefabNumber;
            return PlayerPrefs.GetInt(CarPrefabNumberKey);
        }
        set
        {
            //carPrefabNumber = value;
            PlayerPrefs.SetInt(CarPrefabNumberKey, value);
        }
    }

    public int CarColorNumber
    {
        get
        {
            //return carPrefabNumber;
            return PlayerPrefs.GetInt(CarColorNumberKey);
        }
        set
        {
            //carPrefabNumber = value;
            PlayerPrefs.SetInt(CarColorNumberKey, value);
        }
    }
}
