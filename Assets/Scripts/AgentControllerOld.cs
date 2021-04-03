using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using System;
using Random = UnityEngine.Random;

public class AgentControllerOld : Agent
{
    public string agentName = "Agent_1";
    public float forwardForce;
    public float backForce;
    public float rotationSpeed;
    public float jumpForce;
    public string wheelTag = "Wheel";
    public float maxSpeed;
    public float wheelTurningSpeed;
    public float flyTorque; // Крутящий момент в полете
    private bool addedFlyTorque = false; // Крутящий момент в полете добавлен?
    public PlacesForRespawn placesForRespawn; // Массив с вариантами смещения относительно центра при респауне
    [Range(0, 3)]
    public int respawnPlaceID; // Номер элемента массива placesForRespawn
    public LayerMask surfaceSearchMask;
    public GameObject explosion; // Эффект взрыва при уничтожении
    private Vector3 distanceToGround; // Расстояние начала координат агента до земли

    public int hp = 3; // Health Points
    [HideInInspector] public int currentHp = 3;
    public HealthBar healthBar;

    private float tiresFrictionDelta;
    private float currentRotationSpeed = 0f;
    private bool firstAcceleration = true;
    private bool moveForward = false;

    public float drag = 0.5f;
    public float dragMax = 1.5f;
    private float dragDelta;

    private Rigidbody rb;
    [SerializeField] private bool isCollision = false;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    public GameObject minimapMarker;
    public float Speed { get; private set; } // Абсолютная скорость игрока в горизонтальной плоскости

    private ControlManager controls;
    private bool leftBtnOn, rightBtnOn, gasOn, reverseOn; // Нажата ли кнопка?
    public bool heuristicAlowed;

    // Настройки агента
    public bool training; // Нужно включить если будем обучать агента
    private Vector3 startPosition;
    public Transform[] startPositions;

    private bool flipCheck = false; // Идет проверка на переворот
    private Coroutine flipCor; // Корутина для проверки на переворот
    private float flipCheckDelay = 2f; // Задержка для проверки на переворот
    private float maxSpeedForNormalization = 69f; // Максимальная возможная скорость агента, нужно для нормализации
    public bool newAgent = false; // Для тестов новых агентов

    public Transform[] agentCheckPoints;
    private int currentCheckPointInd = 0;
    private Vector3 currentAgentCheckPoint;
    private int currentLap = 1; // Текущий номер круга
    [HideInInspector] public bool finished = false; // Агент финишировал? Статус используется в GameController
    [HideInInspector] public DateTime raceTime = new DateTime(); // Общее время со старта гонки передается из GameController

    private void Awake()
    {
        controls = new ControlManager();
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        rb.drag = drag;
        dragDelta = dragMax - drag;

        minimapMarker.SetActive(true);
        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }

    void Start()
    {
        // Вычисляем расстояние от центра координат машины до земли
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, surfaceSearchMask);
        distanceToGround = transform.position - hit.point;

        // Создаем Healthbar
        if (healthBar != null)
            healthBar.CreateHealthbar(hp);
        else
            Debug.Log(agentName + ": Healthbar not found");

        currentHp = hp;
    }

    public override void OnEpisodeBegin()
    {
        currentLap = 1;
        ResetAgentCheckPoints();
        currentHp = hp;
        healthBar.ChangeHP(hp, currentHp);

        flipCheck = false;
        if (flipCor != null)
            StopCoroutine(flipCor);

        if (training)
        {
            int randomInd = Random.Range(0, startPositions.Length - 1);
            transform.position = startPositions[randomInd].position;
        }
        else
        {
            transform.position = spawnPosition;
        }
        
        transform.rotation = spawnRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Вычисляем угол между направлением скорости и поворотом авто
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));
        sensor.AddObservation(velocityAngle / 360);

        // Скалярное произведение показывает правильность направления движения
        Vector3 direction = (currentAgentCheckPoint - transform.position).normalized;
        sensor.AddObservation(Vector3.Dot(rb.velocity.normalized, direction));

        // Quaternion.Dot показывает поворот агента относительно направления на чекпоинт
        sensor.AddObservation(Quaternion.Dot(rb.rotation, Quaternion.LookRotation(direction)));

        if (newAgent)
        {
            // Текущая нормализованная скорость в плоскости x-z
            sensor.AddObservation(Speed / maxSpeedForNormalization);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        ActionHandler(actionBuffers.DiscreteActions);
    }

    public void Freeze()
    {
        GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.HeuristicOnly;
    }

    public void UnFreeze()
    {
        GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
        EndEpisode();
    }

    void FixedUpdate()
    {
        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

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
            if (!flipCheck)
            {
                flipCheck = true;
                flipCor = StartCoroutine(Flip());
            }
        }
    }

    private void ActionHandler(ActionSegment<int> act)
    {
        AddReward(-0.0002f);
        int rotateAction = act[0];
        int forwardAction = act[1];
        //Debug.Log("Action");
        if (rotateAction == 1) // Поворот налево
        {
            leftBtnOn = true;
            rightBtnOn = false;
        }
        else if (rotateAction == 2) // Поворот направо
        {
            leftBtnOn = false;
            rightBtnOn = true;
        }
        else
        {
            leftBtnOn = false;
            rightBtnOn = false;
        }

        if (forwardAction == 1) // Газуем вперед
        {
            gasOn = true;
            reverseOn = false;
        }else if (forwardAction == 2) // Газуем назад
        {
            gasOn = false;
            reverseOn = true;
        }
        else
        {
            gasOn = false;
            reverseOn = false;
        }
    }

    public void AgentRespawn()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Ищем предыдущий чекпоинт
        Vector3 previousAgentCheckPoint;
        if (currentCheckPointInd - 1 < 0)
            previousAgentCheckPoint = agentCheckPoints[agentCheckPoints.Length - 1].position;
        else
            previousAgentCheckPoint = agentCheckPoints[currentCheckPointInd - 1].position;

        // Вычисляем новую позицию для машины
        PointOnLine respawnPoint = new PointOnLine(); // Точка ближайшая точка на прямой между текущим и предыдущим чекопоинтом
        Vector3 requiredPoint = respawnPoint.GetPointOnLine(previousAgentCheckPoint, currentAgentCheckPoint, transform.position); // Получаем ближайшую точку на прямой между чекпоинтами
        requiredPoint += placesForRespawn.placesForRespawn[respawnPlaceID]; // Смещаем точку на свое заданное отклонение для респауна
        transform.position = requiredPoint + 20 * Vector3.up; // Максимально поднимаем точку над трассой чтобы пустить вниз луч и нащупать поверхность
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // Меняем слой чтобы агент не обнаружил сам себя

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, surfaceSearchMask)) // Пускаем вниз луч, ищем поверхность
        {
            transform.position = hit.point + distanceToGround;
            //Debug.Log("Найдено пересечение в точке: " + hit.point + " " + hit.collider.name);
        }
        else
        {
            transform.position = requiredPoint + distanceToGround;
            //Debug.Log("Пересечение не найдено!");
        }
        transform.rotation = Quaternion.LookRotation(currentAgentCheckPoint - previousAgentCheckPoint) * Quaternion.AngleAxis(-90, Vector3.up);
        gameObject.layer = LayerMask.NameToLayer("Agent"); // После респауна возрващаем слой агента

        flipCheck = false;
        if (flipCor != null)
            StopCoroutine(flipCor);

        currentHp = hp;
        healthBar.ChangeHP(hp, currentHp);
    }

    public void HitHandler(int damage = 1)
    {
        currentHp -= damage;

        if (currentHp > 0)
        {
            healthBar.ChangeHP(hp, currentHp, true);
        }
        else
        {
            AgentRespawn();
        }
    }

    private void PlayerRotation(float _rotationSpeed)
    {
        Vector3 newRotationAngle = rb.rotation.eulerAngles;

        if (gasOn)
        {
            newRotationAngle += new Vector3(0f, _rotationSpeed, 0f);
        }
        else if (reverseOn)
        {
            newRotationAngle -= new Vector3(0f, _rotationSpeed, 0f);
        }
        else if (rb.velocity.magnitude != 0f)
        {
            float speedPercent = rb.velocity.magnitude / (maxSpeed * 0.01f) / 100f;

            if (moveForward)
                newRotationAngle += new Vector3(0f, _rotationSpeed * speedPercent, 0f);
            else
                newRotationAngle -= new Vector3(0f, _rotationSpeed * speedPercent, 0f);
        }

        rb.rotation = Quaternion.Euler(newRotationAngle);
    }

    private void ChangeDrag()
    {
        // Вычисляем угол между направлением скорости и поворотом авто
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));

        if (velocityAngle <= 90)
            rb.drag = drag + velocityAngle * dragDelta / 90f;
        else
            rb.drag = drag + (180 - velocityAngle) * dragDelta / 90f;
    }

    private void TurningWheels()
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

    private void FlyBehavior()
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

    IEnumerator Flip()
    {
        // Начата проверка на переворот
        yield return new WaitForSeconds(flipCheckDelay);
        // Проверка закончена
        flipCheck = false;
        if (!isCollision)
        {
            AgentRespawn();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            AddReward(-0.015f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AgentCheckPoint") && other.transform == agentCheckPoints[currentCheckPointInd])
        {
            AddReward(1f / agentCheckPoints.Length);

            currentCheckPointInd++;
            if (currentCheckPointInd == agentCheckPoints.Length)
            {
                if (training)
                {
                    EndEpisode();
                }
                else
                {
                    ResetAgentCheckPoints();
                    currentLap++;
                }
            }
            else
            {
                currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
            }
        }
    }

    // Функция принимает словарь со значением дистанций для всех чекпоинтов, длину круга и возвращает оставшиюся до финиша дистанцию
    public float GetRemainingDistance(Dictionary<Transform, float> checkpointDistances, float lapLength, int numberOfLaps)
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

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (heuristicAlowed)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut.Clear();
            if (controls.Player.Right.ReadValue<float>() > 0)
            {
                discreteActionsOut[0] = 2;
            }
            if (controls.Player.Gas.ReadValue<float>() > 0)
            {
                discreteActionsOut[1] = 1;
            }
            if (controls.Player.Left.ReadValue<float>() > 0)
            {
                discreteActionsOut[0] = 1;
            }
            if (controls.Player.Reverse.ReadValue<float>() > 0)
            {
                discreteActionsOut[1] = 2;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        controls.Player.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        controls.Player.Disable();
    }
}
