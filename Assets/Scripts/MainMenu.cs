using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader; // Компонент для загрузки другой сцены

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

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
