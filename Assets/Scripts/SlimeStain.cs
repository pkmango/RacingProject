using UnityEngine;

public class SlimeStain : MonoBehaviour
{
    [Range(0.3f, 0.8f)]
    public float turnSpeedRatio = 0.5f;
    public float massMltiplier = 10f;
    public float modificationTime;

    private string playerTag = "Player";
    private string agentTag = "Agent";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag || other.tag == agentTag)
        {
            PlayerController car = other.GetComponent<PlayerController>();

            if (car.enabled && car.isCollision && car.turnSpeedRatio == 1f)
            {
                car.changeMassCor = car.ChangeMass(massMltiplier, modificationTime, turnSpeedRatio);
                StartCoroutine(car.changeMassCor);
            }
        }
    }
}
