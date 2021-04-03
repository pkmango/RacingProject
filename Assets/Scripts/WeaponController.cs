using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawnPoint;
    public int numberOfBullets;
    private int currentNumberOfBullets;

    public GameObject mine;
    public Transform mineSpawnPoint;
    public int numberOfMines;
    private int currentNumberOfMines;
    public string[] mineBlockingObjectTags = { "Wall", "LandMine" };

    [System.Serializable]
    public class AmmoChangeEvent: UnityEvent<int, int> { } // Создаем типа соыбтия которе может передавать 2 параметра
    public AmmoChangeEvent ammoIsChanged;

    private float checkRadius = 0.45f;

    private void Start()
    {
        if (bullet == null)
            Debug.Log("bullet is not assigned");

        if (bulletSpawnPoint == null)
            Debug.Log("bulletSpawnPoint is not assigned");

        if (mine == null)
            Debug.Log("mine is not assigned");

        if (mineSpawnPoint == null)
            Debug.Log("mineSpawnPoint is not assigned");

        AmmoReload();
    }

    public void Fire()
    {
        if (currentNumberOfBullets > 0)
        {
            currentNumberOfBullets--;
            ammoIsChanged?.Invoke(currentNumberOfBullets, currentNumberOfMines);
            Instantiate(bullet, bulletSpawnPoint.position, transform.rotation);
        }
        else
        {
            Debug.Log("No bullets");
        }
        
    }

    public void SetMine()
    {
        if (currentNumberOfMines > 0)
        {
            // Проверям не ли стены или другой мины в точке респауна
            Collider[] сolliders = Physics.OverlapSphere(mineSpawnPoint.position, checkRadius);
            foreach (var сollider in сolliders)
            {
                foreach (string _tag in mineBlockingObjectTags)
                {
                    if (сollider.tag == _tag)
                    {
                        return;
                    }
                }
            }

            currentNumberOfMines--;
            ammoIsChanged?.Invoke(currentNumberOfBullets, currentNumberOfMines);
            Instantiate(mine, mineSpawnPoint.position, transform.rotation);
        }
        else
        {
            Debug.Log("No mines");
        }
    }

    public void AmmoReload()
    {
        currentNumberOfBullets = numberOfBullets;
        currentNumberOfMines = numberOfMines;
        ammoIsChanged?.Invoke(currentNumberOfBullets, currentNumberOfMines);
    }
}
