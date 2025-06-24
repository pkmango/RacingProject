using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;
    public int damage = 1;
    public string enemyTag = "Agent";
    public string playerTag = "Player";
    [SerializeField]
    private string[] exceptionTags = { "AgentCheckPoint", "Wheel", "Stain" };
    [SerializeField]
    private float destroyTime = 3;
    public ParticleSystem bulletExplosion;

    private GameObject hitObject;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
        Debug.Log($"bullet.speed = {speed}");
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

        if (hitObject.tag == enemyTag || hitObject.tag == playerTag)
        {
            PlayerController car = hitObject.GetComponent<PlayerController>();

            if (car.enabled)
                car.HitHandler(damage);
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
