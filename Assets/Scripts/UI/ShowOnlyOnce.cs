using UnityEngine;

public class ShowOnlyOnce : MonoBehaviour
{
    private static bool _isFirstLoad = true;

    private void Start()
    {
        if (_isFirstLoad)
        {
            _isFirstLoad = false;  // После первого показа помечаем, что больше не нужно показывать
        }
        else
        {
            gameObject.SetActive(false);  // Скрываем объект
        }
    }
}
