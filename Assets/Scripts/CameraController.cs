using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float cameraDeflection; // Значение отклонения камеры
    public float deflectionDelay; // Время задержки для отклонения камеры
    public float deflectionMinAngel; // Минимальный угол для срабатывания отклонения камеры
    public float deflectionSpeed; // Скорость отклонения камеры
    public float speedRatio; // Передаточный коэффициент зависимости скорости камеры от скорости игрока
    //public GameObject testSphere;

    private Vector3 zeroPosition;
    private Rigidbody playerRB;
    //private float lastRBAngle = 0f; // Угол поворота игрока на прошлом кадре
    private float currentCameraDeflection = 0f; // Текущее значение отклонения камеры

    void Start()
    {
        zeroPosition = transform.position;
        playerRB = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float angleRad = (90f + playerRB.rotation.eulerAngles.y) * Mathf.Deg2Rad;
        Vector2 cameraDeflectionXZ = new Vector2(cameraDeflection * Mathf.Cos(angleRad), cameraDeflection * Mathf.Sin(angleRad));
        Vector3 newPosition = new Vector3(player.position.x + zeroPosition.x + (playerRB.velocity.x * speedRatio), transform.position.y, player.position.z + zeroPosition.z + (playerRB.velocity.z * speedRatio));

        transform.position = Vector3.Lerp(transform.position, newPosition, deflectionSpeed);
    }

    //IEnumerator SmoothCameraDeflection()
    //{
    //    yield return new WaitForSeconds(deflectionDelay);

    //    while (currentCameraDeflection < cameraDeflection)
    //    {
    //        currentCameraDeflection += deflectionSpeed;

    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}
