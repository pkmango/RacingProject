using UnityEngine;

public class LandMine : MonoBehaviour
{
    public int damage = 1;
    public string enemyTag = "Agent";
    public string playerTag = "Player";
    public ParticleSystem mineExplosion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == enemyTag || other.tag == playerTag)
        {
            PlayerController car = other.GetComponent<PlayerController>();
            Debug.Log(car.playerName + " подорвался на мине");

            if (car.enabled)
                car.HitHandler(damage);

            MineExplosion();
        }
    }

    private void MineExplosion()
    {
        if (mineExplosion != null)
        {
            mineExplosion.transform.parent = null;
            mineExplosion.Play();
            Destroy(mineExplosion.gameObject, mineExplosion.main.duration);
        }
        else
        {
            Debug.Log("No VFX for mine");
        }
        
        Destroy(gameObject);
    }
}
