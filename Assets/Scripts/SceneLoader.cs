using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public float transitionTime = 1f;
    public string sceneName = "MainMenu";
    public GameObject loadingText;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(SmoothSceneTransition(0f));
    }

    // sceneChange указывает на необходимость загрузки новой сцены
    IEnumerator SmoothSceneTransition(float targetAlpha, bool sceneChange = false)
    {
        float startAlpha = canvasGroup.alpha;
        targetAlpha = Mathf.Clamp01(targetAlpha);
        float transitionSpeed = Mathf.Abs((targetAlpha - startAlpha) / transitionTime);
        float t = 0f; // Интерполятор для Lerp

        while (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            t += transitionSpeed * Time.deltaTime;

            yield return null;
        }

        if (sceneChange)
        {
            loadingText.SetActive(true);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    public void LoadScene(string name)
    {
        sceneName = name;
        StartCoroutine(SmoothSceneTransition(1f, true));
    }
}
