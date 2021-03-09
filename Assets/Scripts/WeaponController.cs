using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bullet;
    public Transform spawnPoint;
    
    public void Fire()
    {
        Instantiate(bullet, spawnPoint.position, transform.rotation);
    }
}
