using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    private Vector3 zeroPosition;

    void Start()
    {
        zeroPosition = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(player.position.x + zeroPosition.x, transform.position.y, player.position.z + zeroPosition.z);
    }
}
