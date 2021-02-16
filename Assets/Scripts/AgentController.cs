using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class AgentController : Agent
{
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

    private Rigidbody rb;
    [SerializeField] private bool isCollision = false;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    public float Speed { get; private set; } // Абсолютная скорость игрока в горизонтальной плоскости

    private ControlManager controls;
    private bool leftBtnOn, rightBtnOn, gasOn, reverseOn; // Нажата ли кнопка?

    // Настройки агента
    public bool training; // Нужно включить если будем обучать агента
    private Vector3 startPosition;
    public Transform[] startPositions;
    public Transform[] agentCheckPoints;
    private int currentCheckPointInd = 0;
    private Vector3 currentAgentCheckPoint;

    private void Awake()
    {
        controls = new ControlManager();
    }

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        rb.drag = drag;
        dragDelta = dragMax - drag;

        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        //controls.Player.Respawn.performed += _ => Respawn();
        //controls.Player.Jump.performed += _ => Jump();

        //Debug.Log(controls.Player);
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("EpisodeBegine");
        //foreach (Transform i in agentCheckPoints)
        //{
        //    i.gameObject.SetActive(true);
        //}
        ResetAgentCheckPoints();

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

        //sensor.AddObservation(transform.localRotation.eulerAngles.y / 360f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        ActionHandler(actionBuffers.DiscreteActions);
    }

    void FixedUpdate()
    {
        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        //CheckButtons();

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

    //private void CheckButtons()
    //{
    //    rightBtnOn = controls.Player.Right.ReadValue<float>() > 0 ? true : false;
    //    leftBtnOn = controls.Player.Left.ReadValue<float>() > 0 ? true : false;
    //    gasOn = controls.Player.Gas.ReadValue<float>() > 0 ? true : false;
    //    reverseOn = controls.Player.Reverse.ReadValue<float>() > 0 ? true : false;
    //}

    private void ActionHandler(ActionSegment<int> act)
    {
        AddReward(-0.0002f);

        int rotateAction = act[0];
        int forwardAction = act[1];

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

    //public void Jump()
    //{
    //    if (isCollision)
    //    {
    //        rb.drag = drag;
    //        rb.AddForce(0f, jumpForce, 0f);
    //    }
    //}

    //public void Respawn()
    //{
    //    transform.position = spawnPosition;
    //    transform.rotation = spawnRotation;
    //    rb.velocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;
    //}

    void PlayerRotation(float _rotationSpeed)
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

    void ChangeDrag()
    {
        // Вычисляем угол между направлением скорости и поворотом авто
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));

        if (velocityAngle <= 90)
            rb.drag = drag + velocityAngle * dragDelta / 90f;
        else
            rb.drag = drag + (180 - velocityAngle) * dragDelta / 90f;

        //Debug.Log(rb.drag);
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
            //Debug.Log(currentCheckPointInd);
            //other.gameObject.SetActive(false);
            AddReward(1f / agentCheckPoints.Length);
            currentCheckPointInd++;
            if (currentCheckPointInd == agentCheckPoints.Length)
            {
                if (training)
                    EndEpisode();
                else
                    ResetAgentCheckPoints();
            }
            else
            {
                currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
            }
        }
    }

    private void ResetAgentCheckPoints()
    {
        currentCheckPointInd = 0;
        currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
    }

    //private void OnEnable()
    //{
    //    controls.Player.Enable();
    //}

    //private void OnDisable()
    //{
    //    controls.Player.Disable();
    //}

    public override void Heuristic(in ActionBuffers actionsOut)
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
