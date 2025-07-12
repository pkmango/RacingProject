using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Компонент для загрузки другой сцены")]
    private SceneLoader sceneLoader;
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    [Tooltip("Массив в который добавляем все UI тесктовые поля с отображением денежного баланса игрока")]
    private Text[] moneyUITxt;
    [SerializeField]
    private Text score;
    [SerializeField]
    [Tooltip("Приветственный интерфейс, который показываем только 1 раз при запуске игры")]
    private GameObject intro;
    [SerializeField]
    [Tooltip("Интерфейс главного меню")]
    private GameObject mainMenu;
    [SerializeField]
    [Tooltip("Название трека из AudioLibrary, звучащего в главном меню")]
    private SoundType musicName;
    [SerializeField]
    [Tooltip("Название звука из AudioLibrary, при клике")]
    private SoundType clickSFX = SoundType.Click;

    private static bool isFirstLoad = true; // Флаг для первой загрузки сцены

    //private void Awake()
    //{
    //    SetMoneyUI();
    //    score.text = playerData.CurrentScore.ToString();

    //    FirstLoad(); // Интро отображаем только при первой загрузке
    //}

    private void Start()
    {
        SetMoneyUI();
        score.text = playerData.CurrentScore.ToString();

        FirstLoad(); // Интро отображаем только при первой загрузке

        AudioManager.Instance.PlayMusic(musicName);
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
        AudioManager.Instance.StartCoroutine(AudioManager.Instance.FadeOutAndStopMusic());
        sceneLoader.gameObject.SetActive(true);
        sceneLoader.LoadScene(scene);
    }

    public void ShowTargetCanvas(GameObject target, GameObject current)
    {
        AudioManager.Instance.PlaySFX(clickSFX);
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
        AudioManager.Instance.PlaySFX(clickSFX);
    }

    public void Quit()
    {
        AudioManager.Instance.PlaySFX(clickSFX);
        Application.Quit();
    }
}
