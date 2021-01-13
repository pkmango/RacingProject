using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float deflectionSpeed; // Скорость отклонения камеры
    public float speedRatio; // Передаточный коэффициент зависимости скорости камеры от скорости игрока
    //public GameObject testSphere;

    private Vector3 zeroPosition;
    private Rigidbody playerRB;
    private float angleRatio; // Угловой коэффициент, нужен для более сильного отклонения камеры когда игрок едет вниз
    private float angleRatioPercent = 0.1f; // Синус угла от скорости слишком большой, берем 10%
    private PlayerController playerController;

    void Start()
    {
        zeroPosition = transform.position;
        playerRB = player.GetComponent<Rigidbody>();
        playerController = player.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        angleRatio = angleRatioPercent * Mathf.Sin(playerController.VelocityAngle() * Mathf.Deg2Rad); // Берем процент угла от скорости
        float newSpeedRatio = speedRatio + angleRatio; // Из-за angleRatio камера будет отставать сильнее когда игрок едет вниз, сделано для улучшения обзора
        Vector3 newPosition = new Vector3
            (player.position.x + zeroPosition.x + (playerRB.velocity.x * newSpeedRatio),
            transform.position.y,
            player.position.z + zeroPosition.z + (playerRB.velocity.z * newSpeedRatio));

        transform.position = Vector3.Lerp(transform.position, newPosition, deflectionSpeed);
    }
}
