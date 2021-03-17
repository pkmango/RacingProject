using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;
    public int damage = 1;
    public string enemyTag = "Agent";
    public string playerTag = "Player";
    public string[] exceptionTags = { "AgentCheckPoint", "Wheel" };
    public float destroyTime = 3;
    public ParticleSystem bulletExplosion;

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
        foreach (string _name in exceptionTags)
        {
            if (other.tag == _name)
                return;
        }

        hitObject = other.gameObject;
    }

    private void BulletHitCheck()
    {
        if (hitObject == null)
            return;

        if (hitObject.tag == enemyTag)
        {
            Debug.Log("Попадание в агента");
            AgentController agent = hitObject.GetComponent<AgentController>();

            if (!agent.finished)
                agent.HitHandler(damage);
        }

        if (hitObject.tag == playerTag)
        {
            Debug.Log("Попадание в игрока");
            PlayerController player = hitObject.GetComponent<PlayerController>();

            if (player.enabled)
                player.HitHandler(damage);
        }

        if (bulletExplosion != null)
            BulletExplosion();
        else
            Destroy(gameObject);
    }

    private void BulletExplosion()
    {
        bulletExplosion.transform.parent = null;
        bulletExplosion.Play();
        Destroy(bulletExplosion.gameObject, bulletExplosion.main.duration);
        Destroy(gameObject);
    }
}
