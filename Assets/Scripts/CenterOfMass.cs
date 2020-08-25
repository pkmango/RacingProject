using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Transform centerOfMassTransform;

    void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.Scale(centerOfMassTransform.localPosition, transform.localScale);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.1f);
    }
}
