using UnityEngine;

public class LapEndTrigger : MonoBehaviour
{
    public int maxCheckPointNumber; // Наибольший номер чекопоинта

    private GameController gameController;
    private int checkPointNumber = 0; // Считаем этот коллайдер чекпоинтом с номером 0

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        else
        {
            Debug.Log("GameController not found");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if(gameController.checkPointNumber == maxCheckPointNumber)
            {
                Debug.Log("Игрок пересек линию финиша");
                gameController.StopCoroutine(gameController.lapTimerCor);
                gameController.lapTimerCor = gameController.StartCoroutine(gameController.LapTimer());
            }

            gameController.checkPointNumber = checkPointNumber;
            gameController.checkPointNumberText.text = "CP: " + checkPointNumber.ToString();
        }
    }
}
