using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Flicker : MonoBehaviour
{
    // Класс реализует мерцание текста между значениями minAlpha и maxAlpha
    [SerializeField, Min(0f)]
    private float minAlpha = 0.6f;
    private float maxAlpha;
    private Text loadingText;
    private Color color;
    private float alpha;

    void Start()
    {
        loadingText = GetComponent<Text>();
        color = loadingText.color;
        maxAlpha = loadingText.color.a;

        if (maxAlpha <= minAlpha)
        {
            Debug.Log("Error: maxAlpha <= minAlpha");
            minAlpha = 0f;
        }
    }

    void Update()
    {
        alpha = Mathf.PingPong(Time.time, maxAlpha)* (1f - minAlpha) + minAlpha;
        loadingText.color = new Color(color.r, color.g, color.b, alpha);
        Debug.Log("alpha = " + alpha + "  Time.time = " + Time.time);
    }
}
