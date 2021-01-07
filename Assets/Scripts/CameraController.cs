using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float cameraDeflection; // Значение отклонения камеры
    public float deflectionDelay; // Время задержки для отклонения камеры
    public float deflectionMinAngel; // Минимальный угол для срабатывания отклонения камеры
    public float deflectionSpeed; // Скорость отклонения камеры
    public float speedRatio; // Передаточный коэффициент зависимости скорости камеры от скорости игрока
    public GameObject testSphere;

    private Vector3 zeroPosition;
    private Rigidbody playerRB;
    private float lastRBAngle = 0f; // Угол поворота игрока на прошлом кадре
    private float currentCameraDeflection = 0f; // Текущее значение отклонения камеры

    void Start()
    {
        zeroPosition = transform.position;
        playerRB = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //transform.position = new Vector3(player.position.x + zeroPosition.x, transform.position.y, player.position.z + zeroPosition.z);
        //transform.position = new Vector3(player.position.x + zeroPosition.x + (playerRB.velocity.x * 0.1f), transform.position.y, player.position.z + zeroPosition.z + (playerRB.velocity.z * 0.1f));

        float angleRad = (90f + playerRB.rotation.eulerAngles.y) * Mathf.Deg2Rad;

        //if (angleRad - lastRBAngle > deflectionMinAngel)
        //{

        //}

        //float dx = currentCameraDeflection * Mathf.Cos(angleRad);
        //float dz = currentCameraDeflection * Mathf.Sin(angleRad);
        Vector2 cameraDeflectionXZ = new Vector2(cameraDeflection * Mathf.Cos(angleRad), cameraDeflection * Mathf.Sin(angleRad));


        //Debug.Log("dx = " + cameraDeflectionXZ.x + "  dy = " + cameraDeflectionXZ.y + " угол = " + playerRB.rotation.eulerAngles.y);
        //Debug.Log(Mathf.Cos(playerRB.rotation.eulerAngles.y * Mathf.Deg2Rad));

        //Vector3 newPosition = new Vector3(player.position.x + zeroPosition.x + cameraDeflectionXZ.y, transform.position.y, player.position.z + zeroPosition.z + cameraDeflectionXZ.x);
        Vector3 newPosition = new Vector3(player.position.x + zeroPosition.x + (playerRB.velocity.x * speedRatio), transform.position.y, player.position.z + zeroPosition.z + (playerRB.velocity.z * speedRatio));

        transform.position = Vector3.Lerp(transform.position, newPosition, deflectionSpeed);

        //transform.position = newPosition;

        //testSphere.transform.position = new Vector3(player.position.x + cameraDeflectionXZ.y, player.position.y, player.position.z + cameraDeflectionXZ.x);
        //testSphere.transform.position = new Vector3(player.position.x + (playerRB.velocity.x * speedRatio), player.position.y, player.position.z + (playerRB.velocity.z * speedRatio));
    }

    IEnumerator SmoothCameraDeflection()
    {
        yield return new WaitForSeconds(deflectionDelay);

        while (currentCameraDeflection < cameraDeflection)
        {
            currentCameraDeflection += deflectionSpeed;

            yield return new WaitForEndOfFrame();
        }
    }
}
