using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerController player;
    public AgentController[] agents;
    public Button startBtn;
    public Text lapTimeText; // Текстовое поле для отображения времени круга
    public Text speedText; // Текстовое поле для отображения скорости
    public Text lapNumberText; // Текстовое поле для отображения номера круга
    public int numberOfLaps;  // Количество кругов
    public int currentLapNumber = 1; // Текущий номер круга
    public Text positionText; // Текстовое поле для отображения текущей позиции
    //public Text checkPointNumberText; // Текстовое поле для отображения номера пройденного чекпоинта (временно)
    [HideInInspector] public int checkPointNumber; // Номер пройденного чекпоинта
    public Coroutine lapTimerCor; // Корутина для периодического отображения времени круга
    public float speedRatio; // Коэффициент для более реалистичного отображения скорости
    public GameObject trafficLights; // Светофор перед стартом
    public float startDelay = 4f; // Время задержки перед стартом для отсчета
    public GameObject pauseBtn;
    public GameObject pauseGroup;

    private float timeDisplayFrequency = 0.1f; // Частота отображения времени круга

    void Start()
    {
        trafficLights.SetActive(false);
        //Time.timeScale = 0f;
        player.enabled = false;
        foreach(AgentController i in agents)
        {
            i.enabled = false;
        }

        // Для тренировки агента
        //startBtn.gameObject.SetActive(false);
        //startDelay = 0;
        //StartCoroutine(StartRace());
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

    public void OnPressStart()
    {
        startBtn.gameObject.SetActive(false);
        trafficLights.SetActive(true);
        StartCoroutine(StartRace());
    }

    private IEnumerator StartRace()
    {
        yield return new WaitForSeconds(startDelay);

        trafficLights.SetActive(false);
        player.enabled = true;
        foreach (AgentController i in agents)
        {
            i.enabled = true;
        }
        lapTimerCor = StartCoroutine(LapTimer());
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseBtn.SetActive(false);
        pauseGroup.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseBtn.SetActive(true);
        pauseGroup.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
