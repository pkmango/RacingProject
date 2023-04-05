using System.Collections;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
public class AutoGun : MonoBehaviour
{
    [Min(0.01f)]
    public float interval = 0.2f; // Интервал(с) срабатывания поиска и применения оружия
    [Min(1f)]
    public float detectionDistance = 10f;
    public LayerMask targetLayerMask;
    public string[] targetTags = { "Player" };
    public bool weaponActivated = true;

    private WeaponController weaponController;

    void Start()
    {
        weaponController = GetComponent<WeaponController>();
        
        if (weaponController != null)
        {
            StartCoroutine(SeekAndFire());
        }
        else
        {
            Debug.Log("Error. Require component WeaponController");
        }
    }

    private IEnumerator SeekAndFire()
    {
        RaycastHit hit;

        while (weaponActivated)
        {
            if (Physics.Raycast(transform.position, transform.right, out hit, detectionDistance, targetLayerMask))
            {
                foreach (string _tag in targetTags)
                {
                    if (hit.transform.gameObject.CompareTag(_tag))
                    {
                        Debug.Log("Цель обнаружена спереди! " + hit.distance);
                        weaponController.Fire();
                    }
                }
            }

            if (Physics.Raycast(transform.position, -transform.right, out hit, detectionDistance, targetLayerMask))
            {
                foreach (string _tag in targetTags)
                {
                    if (hit.transform.gameObject.CompareTag(_tag))
                    {
                        Debug.Log("Цель обнаружена сзади! " + hit.distance);
                        weaponController.SetMine();
                    }
                }
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
