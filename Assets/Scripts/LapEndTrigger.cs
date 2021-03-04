//using UnityEngine;

//public class LapEndTrigger : MonoBehaviour
//{
//    public int maxCheckPointNumber; // Наибольший номер чекопоинта

//    private GameController gameController;
//    private int checkPointNumber = 0; // Считаем этот коллайдер чекпоинтом с номером 0

//    void Start()
//    {
//        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
//        if (gameControllerObject != null)
//        {
//            gameController = gameControllerObject.GetComponent<GameController>();
//        }
//        else
//        {
//            Debug.Log("GameController not found");
//        }

//        gameController.lapNumberText.text = "Lap: " + gameController.currentLapNumber + "/" + gameController.numberOfLaps;
//    }

//    void OnTriggerExit(Collider col)
//    {
//        if (col.CompareTag("Player"))
//        {
//            if(gameController.checkPointNumber == maxCheckPointNumber)
//            {
//                Debug.Log("Игрок пересек линию финиша");

//                // Перезапускаем время круга
//                gameController.StopCoroutine(gameController.lapTimerCor);
//                gameController.lapTimerCor = gameController.StartCoroutine(gameController.LapTimer());

//                // Считаем круги
//                if (gameController.currentLapNumber == gameController.numberOfLaps)
//                {
//                    Debug.Log("Все круги пройдены!");
//                }
//                gameController.currentLapNumber++;
//                gameController.lapNumberText.text = "Lap: " + gameController.currentLapNumber + "/" + gameController.numberOfLaps;
//            }

//            gameController.checkPointNumber = checkPointNumber;
//            //gameController.checkPointNumberText.text = "CP: " + checkPointNumber.ToString();
//        }
//    }
//}
