﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public SpriteRenderer hpSprite;
    public float healthbarWidth = 3f; // Ширина всего healthbar
    public float interval = 0.1f; // Расстояние между ячеек
    public Color spentHpColor; // Цвет для потраченного очка здоровья
    public float setActiveTime = 2f; // После попадания, healthbar появляется на это время

    private Color basicHpColor; // Основной цвет
    private List<SpriteRenderer> hpSprites = new List<SpriteRenderer>();
    private Coroutine visibilityTimerCor; // Корутина для VisibilityTimer
    private Transform parent;
    private Vector3 relativePosition;

    // Метод вызывается из Awake PlayerController
    public void CreateHealthbar(int hp = 1)
    {
        // Добавляем первый спрайт для правильно позиционирования healthbar
        if (hpSprite != null)
        {
            hpSprites.Add(hpSprite);
        }
        else
        {
            Debug.Log("hpSprite not found");
            return;
        }

        // Нужно сохранить ссылку на transform.parent, т.к. дальше он будет обнулен
        parent = transform.parent;

        // Определям позицию healthbar относительно авто
        float scaleX = transform.parent.localScale.x;
        relativePosition = transform.localPosition * scaleX;

        // Запоминаем базовый цвет
        basicHpColor = hpSprite.color;
        
        if (hp <= 1)
        {
            Debug.Log("hp <= 1");
            return;
        }

        float hpSpriteSizeX = (healthbarWidth - interval * (hp - 1)) / hp; // Определяем необходимую ширину одного спрайта чтобы вписаться в общую ширину healthbar
        hpSprite.size = new Vector2(hpSpriteSizeX, hpSprite.size.y);
        float deltaX = (healthbarWidth - hpSprite.size.x) * 0.5f; // Величина смещения для первого спрайта. Нужно чтобы итоговый набор спрайтов был отцентрован
        hpSprite.transform.localPosition = new Vector3(hpSprite.transform.localPosition.x - deltaX, hpSprite.transform.localPosition.y, hpSprite.transform.localPosition.z);

        for (int i = 1; i < hp; i++)
        {
            float newPositionX = hpSprite.transform.localPosition.x + (hpSprite.size.x + interval) * i;
            GameObject newHpSpriteObgect = Instantiate(hpSprite.gameObject, transform);
            newHpSpriteObgect.transform.localPosition = new Vector3(newPositionX, transform.localPosition.y, transform.localPosition.z);
            hpSprites.Add(newHpSpriteObgect.GetComponent<SpriteRenderer>());
        }

        transform.parent = null;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        transform.position = parent.position + relativePosition;
        transform.LookAt(Camera.main.transform);
    }

    IEnumerator VisibilityTimer()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    public void ChangeHP(int hp, int currentHp, bool setActive = false)
    {
        if (setActive)
        {
            SetPosition();
            gameObject.SetActive(true);

            if (visibilityTimerCor != null)
                StopCoroutine(visibilityTimerCor);

            visibilityTimerCor = StartCoroutine(VisibilityTimer());
        }

        int spentHp = hp - currentHp;

        for (int i = 0; i < spentHp; i++)
        {
            hpSprites[i].color = spentHpColor;
        }

        for (int i = spentHp; i < hp; i++)
        {
            try
            {
                hpSprites[i].color = basicHpColor;
            }
            catch
            {
                Debug.Log("ИСКЛЮЧЕНИЕ!, i = " + i + "  hpSprites.Count = " + hpSprites.Count);
            }
        }
    }
}
