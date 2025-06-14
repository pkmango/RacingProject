﻿using UnityEngine;

//[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/PlayerData")]
public class PlayerData : MonoBehaviour
{
    public GameObject[] playerCars;
    public Material[] carMaterials;

    const string NameKey = "Name";
    const string CarPrefabNumberKey = "CarPrefabNumber";
    const string CarColorNumberKey = "CarColorNumber";
    const string MoneyKey = "Money";
    const string TotalScoreKey = "TotalScore";
    const string CurrentScoreKey = "CurrentScore";
    const string NitrousUpgradeLvlKey = "NitrousUpgradeLvl";
    const string EngineUpgradeLvlKey = "EngineUpgradeLvl";
    const string ArmorUpgradeLvlKey = "ArmorUpgradeLvl";
    const string AmmoUpgradeLvlKey = "AmmoUpgradeLvl";
    const string MinesUpgradeLvlKey = "MinesUpgradeLvl";

    public GameObject GetCar() => playerCars[CarPrefabNumber];
    public Material GetCarMaterial() => carMaterials[CarColorNumber];

    public int GetUpgradeLvl(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.NOS: return NitrousUpgradeLvl;
            case UpgradeType.Engine: return EngineUpgradeLvl;
            case UpgradeType.Armor: return ArmorUpgradeLvl;
            case UpgradeType.Ammo: return AmmoUpgradeLvl;
            case UpgradeType.Mines: return MinesUpgradeLvl;
            default:
                Debug.Log("GetUpgradeLvl: Upgrade type not found");
                return 0;
        }
    }

    public void SetUpgradeLvl(UpgradeType upgradeType, int lvlNumber)
    {
        switch (upgradeType)
        {
            case UpgradeType.NOS:
                NitrousUpgradeLvl = lvlNumber;
                break;
            case UpgradeType.Engine:
                EngineUpgradeLvl = lvlNumber;
                break;
            case UpgradeType.Armor:
                ArmorUpgradeLvl = lvlNumber;
                break;
            case UpgradeType.Ammo:
                AmmoUpgradeLvl = lvlNumber;
                break;
            case UpgradeType.Mines:
                MinesUpgradeLvl = lvlNumber;
                break;
            default:
                Debug.Log("SetUpgradeLvl: Upgrade type not found");
                break;
        }
    }

    public string Name
    {
        get
        {
            return PlayerPrefs.GetString(NameKey);
        }
        set
        {
            PlayerPrefs.SetString(NameKey, value);
            Debug.Log("Назначено имя: " + value);
        }
    }
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

    public int Money
    {
        get
        {
            return PlayerPrefs.GetInt(MoneyKey);
        }
        set
        {
            PlayerPrefs.SetInt(MoneyKey, value);
        }
    }

    public int TotalScore
    {
        get
        {
            return PlayerPrefs.GetInt(TotalScoreKey);
        }
        set
        {
            PlayerPrefs.SetInt(TotalScoreKey, value);
        }
    }

    public int CurrentScore
    {
        get
        {
            return PlayerPrefs.GetInt(CurrentScoreKey);
        }
        set
        {
            PlayerPrefs.SetInt(CurrentScoreKey, value);
        }
    }

    public int NitrousUpgradeLvl
    {
        get
        {
            return PlayerPrefs.GetInt(NitrousUpgradeLvlKey);
        }
        set
        {
            PlayerPrefs.SetInt(NitrousUpgradeLvlKey, value);
        }
    }

    public int EngineUpgradeLvl
    {
        get
        {
            return PlayerPrefs.GetInt(EngineUpgradeLvlKey);
        }
        set
        {
            PlayerPrefs.SetInt(EngineUpgradeLvlKey, value);
        }
    }

    public int ArmorUpgradeLvl
    {
        get
        {
            return PlayerPrefs.GetInt(ArmorUpgradeLvlKey);
        }
        set
        {
            PlayerPrefs.SetInt(ArmorUpgradeLvlKey, value);
        }
    }

    public int AmmoUpgradeLvl
    {
        get
        {
            return PlayerPrefs.GetInt(AmmoUpgradeLvlKey);
        }
        set
        {
            PlayerPrefs.SetInt(AmmoUpgradeLvlKey, value);
        }
    }

    public int MinesUpgradeLvl
    {
        get
        {
            return PlayerPrefs.GetInt(MinesUpgradeLvlKey);
        }
        set
        {
            PlayerPrefs.SetInt(MinesUpgradeLvlKey, value);
        }
    }
}
