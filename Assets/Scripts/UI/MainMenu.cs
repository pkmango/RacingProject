using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private SceneLoader sceneLoader; // Компонент для загрузки другой сцены
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private Text[] moneyUITxt;
    [SerializeField]
    private Text score;
    [SerializeField]
    private GameObject intro; // Приветственный интерфейс который показываем только 1 раз при запуске игры
    [SerializeField]
    private GameObject mainMenu; // Интерфейс главного меню

    private static bool isFirstLoad = true; // Флаг для первой загрузки сцены

    private void Awake()
    {
        SetMoneyUI();
        score.text = playerData.CurrentScore.ToString();

        FirstLoad(); // Интро отображаем только при первой загрузке
    }

    private void FirstLoad()
    {
        if (isFirstLoad)
        {
            isFirstLoad = false;
        }
        else
        {
            ShowTargetCanvas(mainMenu, intro);
        }
    }

    public void SetMoneyUI()
    {
        foreach(Text moneyTxt in moneyUITxt)
        {
            moneyTxt.text = playerData.Money.ToString("N0") + " $";
        }
    }

    public void LoadScene(string scene)
    {
        sceneLoader.gameObject.SetActive(true);
        sceneLoader.LoadScene(scene);
    }

    public void ShowTargetCanvas(GameObject target, GameObject current)
    {
        target.SetActive(true);
        current.SetActive(false);
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Add100K()
    {
        playerData.Money += 100000;
        SetMoneyUI();
    }

    public void Add100pts()
    {
        playerData.CurrentScore += 100;
        playerData.TotalScore += 100;
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
