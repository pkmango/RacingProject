using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerController player;
    public Button startBtn;
    public Text lapTimeText; // Текстовое поле для отображения времени круга
    public Text speedText; // Текстовое поле для отображения скорости
    public Text checkPointNumberText; // Текстовое поле для отображения номера пройденного чекпоинта (временно)
    [HideInInspector] public int checkPointNumber; // Номер пройденного чекпоинта
    public Coroutine lapTimerCor; // Корутина для периодического отображения времени круга
    public float speedRatio; // Коэффициент для более реалистичного отображения скорости

    private float timeDisplayFrequency = 0.1f; // Частота отображения времени круга

    void Start()
    {
        //Time.timeScale = 0f;
        player.enabled = false;
    }

    // Корутина для периодического отображения времени круга
    public IEnumerator LapTimer()
    {
        DateTime lapTime = new DateTime();

        while (true)
        {
            lapTime = lapTime.AddMilliseconds(100);
            lapTimeText.text = lapTime.ToString("mm:ss:f");

            speedText.text = Mathf.Round(player.Speed * speedRatio) + " km/h";

            yield return new WaitForSeconds(timeDisplayFrequency);
        }
    }

    public void StartRace()
    {
        //Time.timeScale = 1f;
        player.enabled = true;
        lapTimerCor = StartCoroutine(LapTimer());

        startBtn.gameObject.SetActive(false);
    }
}
