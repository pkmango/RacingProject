using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;
    public string targetLayerName = "Agent";
    public string playerLayerName = "Player";
    public string checkpointLayerName = "Checkpoint";
    public float destroyTime = 3;

    //private bool isHit = false; // Попадание случилось?
    private int hitLayer = -1;
    private GameObject hitObject;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.right * speed);

        BulletHitCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        hitLayer = other.gameObject.layer;
        hitObject = other.gameObject;
    }

    private void BulletHitCheck()
    {
        if (hitObject == null)
            return;

        if (hitLayer == LayerMask.NameToLayer(checkpointLayerName))
            return;

        if (hitLayer == LayerMask.NameToLayer(targetLayerName))
        {
            Debug.Log("Попадание в агента");
            hitObject.GetComponentInParent<AgentController>().AgentRespawn();
        }

        if (hitLayer == LayerMask.NameToLayer(playerLayerName))
        {
            Debug.Log("Попадание в игрока");
            hitObject.GetComponentInParent<PlayerController>().Respawn();
        }

        Destroy(gameObject);
    }
}
