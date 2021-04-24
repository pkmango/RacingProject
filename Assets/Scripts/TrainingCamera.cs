using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingCamera : MonoBehaviour
{
    public GameObject car;

    private void FixedUpdate()
    {
        transform.position = new Vector3(car.transform.position.x, transform.position.y, car.transform.position.z);
    }
}
