using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public string playerName = "Player_1";
    public float forwardForce;
    public float backForce;
    public float rotationSpeed;
    public float jumpForce;
    public string wheelTag = "Wheel";
    public float maxSpeed;
    public float wheelTurningSpeed;
    public float flyTorque; // Крутящий момент в полете
    private bool addedFlyTorque = false; // Крутящий момент в полете добавлен?

    private float tiresFrictionDelta;
    private float currentRotationSpeed = 0f;
    private bool firstAcceleration = true;
    private bool moveForward = false;

    public float drag = 0.5f;
    public float dragMax = 1.5f;
    private float dragDelta;

    private GameController gameController;
    private Rigidbody rb;
    [SerializeField] private bool isCollision = false;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    public GameObject minimapMarker;
    public float Speed { get; private set; } // Абсолютная скорость игрока в горизонтальной плоскости

    private ControlManager controls;
    private bool leftBtnOn, rightBtnOn, gasOn, reverseOn; // Нажата ли кнопка?

    public Transform[] agentCheckPoints; // Агентские чекпоинты используем для определения оставшейся дистанции
    private int currentCheckPointInd = 0; // Текущий индекс массива для агентских чекпоинтов
    private Vector3 currentAgentCheckPoint; // Текущий агентский чекпоинт (следующий который нужно пересечь)
    private int currentLap = 1; // Текущий номер круга

    private void Awake()
    {
        controls = new ControlManager();
    }

    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
            gameController = gameControllerObject.GetComponent<GameController>();
        else
            Debug.Log("GameController not found");

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        ResetAgentCheckPoints();
        minimapMarker.SetActive(true);

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        rb.drag = drag;
        dragDelta = dragMax - drag;

        controls.Player.Respawn.performed += _ => Respawn();
        controls.Player.Jump.performed += _ => Jump();

        //Debug.Log(controls.Player);
    }

    void FixedUpdate()
    {
        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        CheckButtons();

        if (isCollision)
        {
            addedFlyTorque = false;
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0f, rb.angularVelocity.z);

            if (gasOn || reverseOn)
            {
                if (firstAcceleration)
                {
                    currentRotationSpeed = 0f;
                    firstAcceleration = false;
                }

                if (gasOn)
                {
                    moveForward = true;
                    rb.AddRelativeForce(forwardForce, 0f, 0f);
                }
                else
                {
                    moveForward = false;
                    rb.AddRelativeForce(backForce, 0f, 0f);
                } 
            }
            else
            {
                firstAcceleration = true;
            }

            if (rightBtnOn || leftBtnOn)
            {
                TurningWheels();

                if (rightBtnOn)
                    PlayerRotation(currentRotationSpeed);
                else
                    PlayerRotation(-currentRotationSpeed);
            }
            else
            {
                currentRotationSpeed = 0f;
            }

            if (Speed > 0.1f)
                ChangeDrag();
        }
        else
        {
            if (!addedFlyTorque)
            {
                FlyBehavior();
                addedFlyTorque = true;
            }
        }

        //Debug.Log(VelocityAngle());
    }

    private void CheckButtons()
    {
        rightBtnOn = controls.Player.Right.ReadValue<float>() > 0 ? true : false;
        leftBtnOn = controls.Player.Left.ReadValue<float>() > 0 ? true : false;
        gasOn = controls.Player.Gas.ReadValue<float>() > 0 ? true : false;
        reverseOn = controls.Player.Reverse.ReadValue<float>() > 0 ? true : false;
    }

    public void Jump()
    {
        if (isCollision)
        {
            rb.drag = drag;
            rb.AddForce(0f, jumpForce, 0f);
        }
    }

    public void Respawn()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void PlayerRotation(float _rotationSpeed)
    {
        Vector3 newRotationAngle = rb.rotation.eulerAngles;

        if (gasOn)
        {
            newRotationAngle += new Vector3(0f, _rotationSpeed, 0f);
        }else if (reverseOn)
        {
            newRotationAngle -= new Vector3(0f, _rotationSpeed, 0f);
        }else if (rb.velocity.magnitude != 0f)
        {
            float speedPercent = rb.velocity.magnitude / (maxSpeed * 0.01f) / 100f;

            if (moveForward)
                newRotationAngle += new Vector3(0f, _rotationSpeed * speedPercent, 0f);
            else
                newRotationAngle -= new Vector3(0f, _rotationSpeed * speedPercent, 0f);
        }

        rb.rotation = Quaternion.Euler(newRotationAngle);
    }

    void ChangeDrag()
    {
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));

        if (velocityAngle <= 90)
            rb.drag = drag + velocityAngle * dragDelta / 90f;
        else
            rb.drag = drag + (180 - velocityAngle) * dragDelta / 90f;
    }

    void TurningWheels()
    {
        if (currentRotationSpeed < rotationSpeed)
        {
            currentRotationSpeed += wheelTurningSpeed;
        }
        else
        {
            currentRotationSpeed = rotationSpeed;
        }
    }

    public float VelocityAngle() // Угол наравления скорости
    {
        float velocityAngle = 0f;

        if (Speed != 0f)
        {
            velocityAngle = Mathf.Acos(Mathf.Clamp(rb.velocity.x / Speed, -1f, 1f)) * Mathf.Rad2Deg;

            if (rb.velocity.z > 0f)
            {
                velocityAngle = 360f - velocityAngle;
            }
        }
        return velocityAngle;
    }

    void FlyBehavior()
    {
        rb.drag = drag;

        if (leftBtnOn && gasOn || rightBtnOn && reverseOn)
        {
            rb.AddRelativeTorque(0f, -flyTorque, 0f);
        }

        if (rightBtnOn && gasOn || leftBtnOn && reverseOn)
        {
            rb.AddRelativeTorque(0f, flyTorque, 0f);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isCollision = false;
    }

    void OnCollisionStay(Collision collision)
    {
        int numberOfContacts = 0;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(0).thisCollider.tag == wheelTag)
            {
                numberOfContacts++;

                if (numberOfContacts > 1)
                {
                    isCollision = true;
                }
            }
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AgentCheckPoint") && other.transform == agentCheckPoints[currentCheckPointInd])
        {
            currentCheckPointInd++;
            if (currentCheckPointInd == agentCheckPoints.Length)
            {
                ResetAgentCheckPoints();
                currentLap++;
            }
            else
            {
                currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
            }
        }
    }

    // Функция принимает словарь со значением дистанций для всех чекпоинтов, длину круга и возвращает оставшиюся до финиша дистанцию
    public float GetRemainingDistance (Dictionary<Transform, float> checkpointDistances, float lapLength, int numberOfLaps)
    {
        float remainingDistance = 0;
        Transform currentCheckPoint = agentCheckPoints[currentCheckPointInd];
        float distanceToCheckpoint = (agentCheckPoints[currentCheckPointInd].position - transform.position).magnitude;

        remainingDistance = (numberOfLaps - currentLap) * lapLength + checkpointDistances[currentCheckPoint] + distanceToCheckpoint;

        return remainingDistance;
    }

    private void ResetAgentCheckPoints()
    {
        currentCheckPointInd = 0;
        currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}
