using UnityEngine;

public class MinimapMarkerMover : MonoBehaviour
{
    private Transform car;
    private Quaternion rotation;

    private void Start()
    {
        car = transform.parent;
        rotation = transform.rotation;
        transform.parent = null;
    }
    private void Update()
    {
        transform.position = new Vector3(car.position.x, transform.position.y, car.position.z);
        transform.rotation = rotation;
    }
}
