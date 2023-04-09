using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool isTraining; // Включаем во время обучения агентов

    public PlayerController player;
    public PlayerData playerData;
    public CameraController mainCamera;
    public string nextLevel = "Level_2";
    public string mainMenuName = "MainMenu";
    public SceneLoader sceneLoader; // Компонент для загрузки другой сцены
    public AgentController[] agents;

    public Button startBtn;
    public Text lapTimeText; // Текстовое поле для отображения времени круга
    public Text speedText; // Текстовое поле для отображения скорости
    public Text lapNumberText; // Текстовое поле для отображения номера круга
    public int numberOfLaps;  // Количество кругов
    public int currentLapNumber = 1; // Текущий номер круга
    public Text positionText; // Текстовое поле для отображения текущей позиции
    public Text positionSuffixText; // Суффикс для обозначения позиции
    private string[] positionSuffixes = {"", "st", "nd", "rd", "th", "th"}; // Набор суффиксов для отображения позиции
    public Text numberOfBulletsText, numberOfMinesText;

    public Coroutine lapTimerCor; // Корутина для периодического отображения времени круга
    public float speedRatio; // Коэффициент для более реалистичного отображения скорости
    public GameObject trafficLights; // Светофор перед стартом
    public float startDelay = 4f; // Время задержки перед стартом для отсчета
    public GameObject pauseBtn;
    public GameObject pauseGroup;
    public GameObject raceIsOverGroup;
    public PlaceCanvasData[] placeCanvasesData;

    //public RectTransform onePlayerHP; // Элемента UI из которых строится healthbar игрока
    //private List<RectTransform> playerHPs = new List<RectTransform>() ; // 

    private float timeDisplayFrequency = 0.1f; // Частота отображения времени круга
    private DateTime raceTime; // Таймер для подсчета общего времени гонки

    public Transform[] agentCheckPoints; // Чекпоинты для агента. Здесь используются для определения позиций агентов и игрока на трассе
    private Dictionary<Transform, float> checkpointDistances = new Dictionary<Transform, float>(); // Дистанции до конца круга для всех агентских чекпоинтов
    private float lapLength; // Длина круга
    private List<RacerFinishData> listOfFinishers = new List<RacerFinishData>(); // Список финишировавших

    //private PlayerData playerData;

    private void Awake()
    {
        AddPlayer();
    }

    void Start()
    {
        trafficLights.SetActive(false);
        pauseBtn.SetActive(false);

        if (!isTraining)
        {
            foreach (AgentController agent in agents)
            {
                agent.Freeze();
            }
        }

        lapNumberText.text = currentLapNumber + "/" + numberOfLaps;
        SetRemaningDistances();
        PlacesCheck();

        // Для тренировки агента
        if (isTraining)
        {
            startBtn.gameObject.SetActive(false);
            startDelay = 0;
            StartCoroutine(StartRace());
        }
    }

    private void AddPlayer()
    {
        if (playerData != null)
        {
            player.gameObject.SetActive(false); // Отключаем дефолтного игрока
            GameObject playerObj = Instantiate(playerData.GetCar(), player.transform.position, player.transform.rotation);
            player = playerObj.GetComponent<PlayerController>();
            player.agentCheckPoints = agentCheckPoints;
            player.lapIsOver.AddListener(OnLapIsOver);
            player.weaponController.ammoIsChanged.AddListener(SetUIAmmoValue);
        }
        else
        {
            Debug.Log("PlayerData not found! Default player prefab used");
            player.gameObject.SetActive(true);
        }

        mainCamera.SetPlayer(player.transform);
    }

    // Корутина для периодического отображения времени круга, скорости, текущей позиции
    // Также вызывает метод PlacesCheck(), который может завершить гонку
    public IEnumerator LapTimer()
    {
        DateTime lapTime = new DateTime();

        while (true)
        {
            speedText.text = Mathf.Round(player.Speed * speedRatio).ToString();
            PlacesCheck();

            yield return new WaitForSeconds(timeDisplayFrequency);

            lapTime = lapTime.AddSeconds(timeDisplayFrequency);
            raceTime = raceTime.AddSeconds(timeDisplayFrequency);
            lapTimeText.text = lapTime.ToString("mm:ss:f");
        }
    }


    // Проверяет занимаемое место и оставшееся расстояние до финиша, если меньше 0, то завершает гонку
    private void PlacesCheck()
    {
        float playerRemainingDistance = player.GetRemainingDistance(checkpointDistances, lapLength, numberOfLaps);
        int playerPlace = 1; // Место игрока в гонке

        if (playerRemainingDistance <= 0f)
        {
            FinishRace();
            return;
        }

        foreach (AgentController agent in agents)
        {
            float agentRemainingDistance = agent.car.GetRemainingDistance(checkpointDistances, lapLength, numberOfLaps);

            if (agentRemainingDistance < playerRemainingDistance)
                playerPlace++;
            
            if (agentRemainingDistance <= 0f && agent.car.enabled)
            {
                //agent.finished = true;
                agent.raceTime = raceTime;
                listOfFinishers.Add(new RacerFinishData(agent.car.playerName, agent.raceTime.ToString("mm:ss:f")));
                agent.car.enabled = false;
                Debug.Log("Агент " + agent.car.playerName + " финишировал на " + listOfFinishers.Count + " месте");
            }
        }

        positionText.text = playerPlace.ToString();
        if (positionSuffixes[playerPlace] != null)
            positionSuffixText.text = positionSuffixes[playerPlace];
    }

    // Формируем список нефинишировавших агентов и добавляем его к списку финишировавших для вывода таблицы результатов
    void AddingUnfinishedRacers()
    {
        List<RacerFinishData> unfinishedRacers = new List<RacerFinishData>();
        float raceTimeInSeconds = (float)(raceTime - DateTime.MinValue).TotalSeconds; // Получаем общее время гонки в секундах
        float playerAverageSpeed = lapLength * numberOfLaps / raceTimeInSeconds; // Вычисляем среднюю скорость игрока

        foreach (AgentController agent in agents)
        {
            if (agent.car.enabled)
            {
                float agentRemainingDistance = agent.car.GetRemainingDistance(checkpointDistances, lapLength, numberOfLaps);
                agent.raceTime = raceTime.AddSeconds(agentRemainingDistance / playerAverageSpeed);
                unfinishedRacers.Add(new RacerFinishData(agent.car.playerName, agent.raceTime.ToString("mm:ss:f"), agentRemainingDistance));
                //agent.finished = true;
                agent.car.enabled = false;
            }
        }

        // Сортируем список нефинишировавших агентов по оставшийся дистанции и добавляем к списку финишеров
        listOfFinishers.AddRange(unfinishedRacers.OrderBy(r => r.remainingDistance));
    }

    // Обработчик для события когда игрок закончил круг
    public void OnLapIsOver()
    {
        Debug.Log("Игрок пересек линию финиша");

        if (currentLapNumber < numberOfLaps)
        {
            // Перезапускаем время круга
            StopCoroutine(lapTimerCor);
            lapTimerCor = StartCoroutine(LapTimer());

            currentLapNumber++;
            lapNumberText.text = currentLapNumber + "/" + numberOfLaps;
        }
    }

    public void OnPressStart()
    {
        startBtn.gameObject.SetActive(false);
        speedText.transform.parent.gameObject.SetActive(true);
        trafficLights.SetActive(true);
        StartCoroutine(StartRace());
    }

    private IEnumerator StartRace()
    {
        yield return new WaitForSeconds(startDelay);

        trafficLights.SetActive(false);
        pauseBtn.SetActive(true);
        player.enabled = true;

        if (!isTraining)
        {
            foreach (AgentController agent in agents)
            {
                agent.car.enabled = true;
                agent.UnFreeze();
            }
        }
            
        raceTime = new DateTime();
        lapTimerCor = StartCoroutine(LapTimer());
    }

    void FinishRace()
    {
        pauseBtn.SetActive(false);
        listOfFinishers.Add(new RacerFinishData(player.playerName, raceTime.ToString("mm:ss:f")));
        Debug.Log("Игрок финишировал на " + listOfFinishers.Count + " месте. Гонка завершена");
        StopCoroutine(lapTimerCor);
        AddingUnfinishedRacers();
        player.enabled = false;
        StartCoroutine(ShowRaceResults());
    }

    IEnumerator ShowRaceResults()
    {
        yield return new WaitForSeconds(1f);

        raceIsOverGroup.SetActive(true);
        if (placeCanvasesData.Length != listOfFinishers.Count)
        {
            Debug.Log("Ошибка. Список финишировавших не полный");
            yield break;
        }
        for (int i = 0; i < placeCanvasesData.Length; i++)
        {
            placeCanvasesData[i].SetPlaceCanvasData(listOfFinishers[i].name, listOfFinishers[i].time);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.controls.Disable();
        pauseBtn.SetActive(false);
        pauseGroup.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        player.controls.Enable();
        pauseBtn.SetActive(true);
        pauseGroup.SetActive(false);
    }

    public void Restart()
    {
        Resume();
        if (lapTimerCor != null)
            StopCoroutine(lapTimerCor);

        listOfFinishers.Clear();
        foreach (AgentController agent in agents)
        {
            agent.car.enabled = true;
            agent.EndEpisode();
            agent.Freeze();
        }

        player.Restart();
        mainCamera.SetStartPosition();
        startBtn.gameObject.SetActive(true);
        currentLapNumber = 1;
        lapNumberText.text = currentLapNumber + "/" + numberOfLaps;
        speedText.text = "0";
        lapTimeText.text = "00:00:0";
        PlacesCheck();
        raceIsOverGroup.SetActive(false);

        LandMine[] allMines = FindObjectsOfType<LandMine>();
        foreach (LandMine mine in allMines)
        {
            Destroy(mine.gameObject);
        }
    }

    // Вычисление оставшихся дистанций пути для кадого agentCheckPoint и общей длины круга
    private void SetRemaningDistances()
    {
        float segmentLength = 0f;

        // Длина последнего отрезка равна нулю (точка финиша)
        checkpointDistances.Add(agentCheckPoints[agentCheckPoints.Length - 1], 0f);

        for (int i = agentCheckPoints.Length - 1; i > 0; i--)
        {
            segmentLength += (agentCheckPoints[i - 1].position - agentCheckPoints[i].position).magnitude;
            checkpointDistances.Add(agentCheckPoints[i - 1], segmentLength);
        }
        // Чтобы узнать полную длину круга добавляем оставшийся отрезок между первым и последним чекпоинтом
        lapLength = segmentLength + (agentCheckPoints[agentCheckPoints.Length - 1].position - agentCheckPoints[0].position).magnitude;
    }

    public void SetUIAmmoValue(int numberOfBullets, int numberOfMines)
    {
        numberOfBulletsText.text = numberOfBullets.ToString();
        numberOfMinesText.text = numberOfMines.ToString();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        sceneLoader.gameObject.SetActive(true);
        sceneLoader.LoadScene(mainMenuName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    class RacerFinishData
    {
        public string name;
        public string time;
        public float remainingDistance;

        public RacerFinishData(string racerName = "RacerName", string racerTime = "00:00", float racerRamainingDistance = 0f)
        {
            name = racerName;
            time = racerTime;
            remainingDistance = racerRamainingDistance;
        }
    }

    public void LoadScene()
    {
        sceneLoader.gameObject.SetActive(true);
        sceneLoader.LoadScene(nextLevel);
    }
}
