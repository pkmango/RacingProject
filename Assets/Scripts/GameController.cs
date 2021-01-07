using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text lapTimeText; // Текстовое поле для отображения времени круга
    public Text checkPointNumberText; // Текстовое поле для отображения номера пройденного чекпоинта (временно)
    public int checkPointNumber; // Номер пройденного чекпоинта

    public Coroutine lapTimerCor;

    void Start()
    {
        lapTimerCor = StartCoroutine(LapTimer());
    }

    void Update()
    {

    }

    public IEnumerator LapTimer()
    {
        float startLapTime = Time.time;

        while (true)
        {
            lapTimeText.text = Math.Round(Time.time - startLapTime, 1).ToString();


            yield return new WaitForSeconds(0.1f);
        }
    }
}
