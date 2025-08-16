using UnityEngine;

public class AudioListenerController : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        // Находим основную камеру один раз при старте
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("На сцене не найдена основная камера (с тегом 'MainCamera'). AudioListener не сможет правильно ориентироваться.", this);
        }
    }

    // Используем LateUpdate, чтобы гарантированно выполняться после всех обновлений камеры
    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.rotation = cameraTransform.rotation;
        }
    }
}
