using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text lapTimeText; // Текстовое поле для отображения времени круга
    public Text checkPointNumberText; // Текстовое поле для отображения номера пройденного чекпоинта (временно)
    [HideInInspector] public int checkPointNumber; // Номер пройденного чекпоинта
    public Coroutine lapTimerCor; // Корутина для периодического отображения времени круга

    private float timeDisplayFrequency = 0.1f; // Частота отображения времени круга

    void Start()
    {
        //DateTime now = DateTime.Now;

        lapTimerCor = StartCoroutine(LapTimer());
    }

    // Корутина для периодического отображения времени круга
    public IEnumerator LapTimer()
    {
        DateTime lapTime = new DateTime();

        while (true)
        {
            lapTime = lapTime.AddMilliseconds(100);
            lapTimeText.text = lapTime.ToString("mm:ss:f");

            yield return new WaitForSeconds(timeDisplayFrequency);
        }
    }

    
}
