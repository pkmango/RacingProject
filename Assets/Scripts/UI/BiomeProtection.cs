using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeProtection : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private GameObject biomLock;
    [SerializeField]
    private Image image;
    [SerializeField]
    private int scoresToUnlock = 1600;

    private void OnEnable()
    {
        Debug.Log(playerData.CurrentScore);
        Debug.Log(playerData.TotalScore);

        if (playerData.CurrentScore < scoresToUnlock)
        {
            image.raycastTarget = false;
        }
        else
        {
            biomLock.SetActive(false);
            image.color = Color.clear;
        }
    }
}
