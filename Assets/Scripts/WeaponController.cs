using System.Collections;
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

    private const float MineCheckRadius = 0.45f; // Радиус проверки перед постановкой мины

    // Переменные для реализации автоматической стрельбы
    public bool autoModeOn = false;
    [Min(0.01f)]
    public float interval = 0.2f; // Интервал(с) срабатывания поиска и применения оружия
    [Min(1f)]
    public float detectionDistance = 100f;
    public LayerMask targetLayerMask;
    public string[] targetTags = { "Player" };

    private Coroutine seekAndFireCor;

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
            //Debug.Log("No bullets");
        }
        
    }

    public void SetMine()
    {
        if (currentNumberOfMines > 0)
        {
            // Проверям нет ли стены или другой мины в точке респауна
            Collider[] сolliders = Physics.OverlapSphere(mineSpawnPoint.position, MineCheckRadius);
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
            //Debug.Log("No mines");
        }
    }

    public void AmmoReload()
    {
        Debug.LogFormat("AmmoReload");
        currentNumberOfBullets = numberOfBullets;
        currentNumberOfMines = numberOfMines;
        ammoIsChanged?.Invoke(currentNumberOfBullets, currentNumberOfMines);
    }

    public void AutoMode(bool isOn)
    {
        if (autoModeOn)
        {
            if (isOn)
            {
                seekAndFireCor = StartCoroutine(SeekAndFire());
            }
            else
            {
                if (seekAndFireCor != null)
                    StopCoroutine(seekAndFireCor);
            }
        }
    }

    private IEnumerator SeekAndFire()
    {
        RaycastHit hit;

        while (autoModeOn)
        {
            if (Physics.Raycast(transform.position, transform.right, out hit, detectionDistance, targetLayerMask))
            {
                foreach (string _tag in targetTags)
                {
                    if (hit.transform.gameObject.CompareTag(_tag))
                    {
                        //Debug.Log("Цель обнаружена спереди! " + hit.distance);
                        Fire();
                    }
                }
            }

            if (Physics.Raycast(transform.position, -transform.right, out hit, detectionDistance, targetLayerMask))
            {
                foreach (string _tag in targetTags)
                {
                    if (hit.transform.gameObject.CompareTag(_tag))
                    {
                        //Debug.Log("Цель обнаружена сзади! " + hit.distance);
                        SetMine();
                    }
                }
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
