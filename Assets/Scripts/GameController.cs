using System;
using System.Collections;
using System.Collections.Generic;
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

    public Transform[] agentCheckPoints; // Чекпоинты для агента. Здесь используются для определения позиций агентов и игрока на трассе
    private Dictionary<Transform, float> checkpointDistances = new Dictionary<Transform, float>(); // Дистанции до конца круга для всех агентских чекпоинтов
    private float lapLength; // Длина круга

    void Start()
    {
        trafficLights.SetActive(false);
        //Time.timeScale = 0f;
        player.enabled = false;
        foreach(AgentController i in agents)
        {
            i.enabled = false;
        }

        SetRemaningDistances();
        PlacesCheck();

        // Для тренировки агента
        //startBtn.gameObject.SetActive(false);
        //startDelay = 0;
        //StartCoroutine(StartRace());
    }

    // Корутина для периодического отображения времени круга, скорости, текущей позиции
    public IEnumerator LapTimer()
    {
        DateTime lapTime = new DateTime();

        while (true)
        {
            lapTime = lapTime.AddMilliseconds(100);
            lapTimeText.text = lapTime.ToString("mm:ss:f");

            speedText.text = Mathf.Round(player.Speed * speedRatio) + " km/h";
            PlacesCheck();

            yield return new WaitForSeconds(timeDisplayFrequency);
        }
    }

    private void FixedUpdate()
    {
        
    }

    private void PlacesCheck()
    {
        float playerRemainingDistance = player.GetRemainingDistance(checkpointDistances, lapLength, numberOfLaps);
        //Debug.Log(playerRemainingDistance);
        int playerPlace = 1; // Место игрока в гонке

        foreach (AgentController agent in agents)
        {
            if (agent.GetRemainingDistance(checkpointDistances, lapLength, numberOfLaps) < playerRemainingDistance)
                playerPlace++;
        }
        positionText.text = "Pos: " + playerPlace + "/" + (agents.Length + 1);
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

    public IEnumerator FinishRace()
    {
        yield return new WaitForSeconds(1f);
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

    // Вычисление оставшейся дистанции до конца круга
    //private float RemainingDistance(Vector3 currentPosition, int startIndex = 0)
    //{
    //    float lapLength = (agentCheckPoints[startIndex].position - currentPosition).magnitude;

    //    for (int i = startIndex + 1; i < agentCheckPoints.Length; i++)
    //    {
    //        lapLength += (agentCheckPoints[i].position - agentCheckPoints[i - 1].position).magnitude;
    //    }

    //    return lapLength;
    //}

    private void SetRemaningDistances()
    {
        float segmentLength = 0f;

        // Длина последнего отрезка равна нулю (точка финиша)
        checkpointDistances.Add(agentCheckPoints[agentCheckPoints.Length - 1], 0f);

        for (int i = agentCheckPoints.Length - 1; i > 0; i--)
        {
            segmentLength += (agentCheckPoints[i - 1].position - agentCheckPoints[i].position).magnitude;
            checkpointDistances.Add(agentCheckPoints[i - 1], segmentLength);
            //Debug.Log("чекпоинт №" + (i - 1) + " до финиша: " + checkpointDistances[agentCheckPoints[i - 1]]);
        }
        // Чтобы узнать полную длину круга добавляем оставшийся отрезок между первым и последним чекпоинтом
        lapLength = segmentLength + (agentCheckPoints[agentCheckPoints.Length - 1].position - agentCheckPoints[0].position).magnitude;
        //Debug.Log("Длина круга: " + lapLength);
}

    public void Quit()
    {
        Application.Quit();
    }
}
