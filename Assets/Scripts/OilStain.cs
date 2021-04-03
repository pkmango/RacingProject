using UnityEngine;

public class OilStain : MonoBehaviour
{
    public float force = 10; // Сила подкрутки
    public float minSpeed = 10f; // Минимальная скорость срабатывания

    private string playerTag = "Player";
    private string agentTag = "Agent";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag || other.tag == agentTag)
        {
            PlayerController car = other.GetComponent<PlayerController>();

            if (!car.isSpinOut && car.enabled && car.isCollision && car.Speed > minSpeed)
            {
                car.spinOutCor = car.SpinOut(force);
                StartCoroutine(car.spinOutCor);
            }
        }
    }
}
