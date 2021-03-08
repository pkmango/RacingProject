using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskTest : MonoBehaviour
{
    public LayerMask mask;

    //LayerMask mask;

    //private void Start()
    //{
    //    //mask = 1 << 11;
    //    //Convert Layer Name to Layer Number
    //    int cubeLayerIndex = LayerMask.NameToLayer("Checkpoint");
    //    Debug.Log(cubeLayerIndex);


    //    //Check if layer is valid
    //    if (cubeLayerIndex == -1)
    //    {
    //        Debug.LogError("Layer Does not exist");
    //    }

    //    mask = 1 << cubeLayerIndex;
    //}
    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask)) // Пускаем вниз луч, ищем поверхность
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.yellow);
            //Debug.Log("Найдено пересечение в точке: " + hit.point + " " + hit.collider.name);
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 1000, Color.white);
            //Debug.Log("Пересечение не найдено!");
        }
    }
}
