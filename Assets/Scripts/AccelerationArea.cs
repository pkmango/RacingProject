using UnityEngine;

public class AccelerationArea : MonoBehaviour
{
    public float force;
    private Vector3 v3Force;

    private string playerTag = "Player";
    private string agentTag = "Agent";

    private void Start()
    {
        v3Force = new Vector3(force, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag || other.tag == agentTag)
        {
            PlayerController car = other.GetComponent<PlayerController>();
            if (car.enabled && car.isCollision)
                car.rb.AddForce(transform.TransformDirection(v3Force));
        }
    }
}
