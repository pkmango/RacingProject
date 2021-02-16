using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int checkPointNumber;

    private GameController gameController;

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

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if(gameController.checkPointNumber + 1 == checkPointNumber)
            {
                //gameController.checkPointNumberText.text = "CP: " + checkPointNumber.ToString();
                gameController.checkPointNumber = checkPointNumber;
            }
        }
    }
}
