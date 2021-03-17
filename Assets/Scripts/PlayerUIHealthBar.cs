using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHealthBar : MonoBehaviour
{
    public RectTransform oneHpRect;
    public float intervalWidth = 2f;
    public Color spentHpColor; // Цвет для потраченного очка здоровья
    private Color basicHpColor; // Основной цвет
    private List<Image> playerHPs = new List<Image>(); // Список хранит все полоски здоровья healthbar
    private float fullWidth;

    private void Start()
    {
        if (oneHpRect == null)
        {
            Debug.Log("oneHpRect not found");
            return;
        }

        fullWidth = oneHpRect.sizeDelta.x;
        playerHPs.Add(oneHpRect.GetComponent<Image>());
        basicHpColor = playerHPs[0].color;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            CreateHealthbar(playerObj.GetComponent<PlayerController>().hp);
        }
        else
        {
            Debug.Log("Player not found");
        }
    }

    public void OnHpIsChanged(int hp, int currentHp)
    {
        for (int i = 0; i < currentHp; i++)
        {
            playerHPs[i].color = basicHpColor;
        }

        for (int i = currentHp; i < hp; i++)
        {
            playerHPs[i].color = spentHpColor;
        }
    }

    private void CreateHealthbar(int hp = 1)
    {
        if (hp <= 1)
        {
            Debug.Log("hp <= 1");
            return;
        }
        
        float targetWidth = (fullWidth - intervalWidth * (hp - 1)) / hp;
        oneHpRect.sizeDelta = new Vector2(targetWidth, oneHpRect.sizeDelta.y);

        for (int i = 1; i < hp; i++)
        {
            RectTransform newHPoint = Instantiate(oneHpRect, transform).GetComponent<RectTransform>();
            float newPositionX = newHPoint.localPosition.x + (targetWidth + intervalWidth) * i;
            newHPoint.localPosition = new Vector3(newPositionX, newHPoint.localPosition.y, newHPoint.localPosition.z);
            playerHPs.Add(newHPoint.GetComponent<Image>());
        }
    }
}
