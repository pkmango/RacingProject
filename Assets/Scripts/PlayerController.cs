﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public bool isAgent; // Это агент?
    private AgentController agent;

    public string playerName = "Player_1";
    private PlayerData playerData;
    public float forwardForce;
    private float currentForwardForce;
    public float backForce;
    public float rotationSpeed;
    public float jumpForce;
    [SerializeField, Min(0)] private float nitrousForce = 20f;
    [SerializeField, Min(0)] private float nitrousTime = 1.3f;
    [SerializeField] private ParticleSystem nitrousVFX;
    private Coroutine nitrousActionCor; // Корутина для Нитро
    private bool isNitrousOn = false;
    [HideInInspector] public bool nitrousReady = true; // Включаем нитро в начале круга
    public string wheelTag = "Wheel";
    public float maxSpeed;
    public float wheelTurningSpeed;
    [HideInInspector] public float flyTorque; // Крутящий момент в полете
    private bool addedFlyTorque = false; // Крутящий момент в полете добавлен?
    public LayerMask surfaceSearchMask;
    public GameObject explosion;
    private Vector3 distanceToGround; // Расстояние начала координат агента до земли

    public int hp = 3; // Health Points
    [HideInInspector] public int currentHp = 3;
    public HealthBar healthBar;

    private float currentRotationSpeed = 0f;
    [HideInInspector] public float turnSpeedRatio = 1f;
    private bool firstAcceleration = true;
    private bool moveForward = false;

    public float drag = 0.5f;
    public float dragMax = 1.5f;
    private float dragDelta;
    private float penaltyDrag = 0f;

    [HideInInspector] public Rigidbody rb;
    //private float mass;
    //public IEnumerator changeMassCor;
    [HideInInspector] public bool isCollision = true;
    [HideInInspector] public Vector3 spawnPosition;
    [HideInInspector] public Quaternion spawnRotation;
    public GameObject minimapMarker;
    public float Speed { get; private set; } // Абсолютная скорость игрока в горизонтальной плоскости

    [HideInInspector] public ControlManager controls;
    [HideInInspector] public bool leftBtnOn, rightBtnOn, gasOn, reverseOn; // Нажата ли кнопка?
    [HideInInspector] public WeaponController weaponController;

    [HideInInspector] public bool isSpinOut = false; // Происходит ли неконтролиуемое вращение
    private float spinOutTimeRatio = 0.02f; // Коэффициент времени вращения, умножаем на скорость и получаем время вращения
    public IEnumerator spinOutCor; // Корутина для SpinOut

    private bool flipCheck = false; // Идет проверка на переворот
    private Coroutine flipCor; // Корутина для проверки на переворот
    public float flipCheckDelay = 2f; // Задержка для проверки на переворот

    private const string ForbiddenAreaTag = "ForbiddenArea"; // Этим тэгом помечаются объекты при пересечении которых игрок считается в запретной зоне
    private bool isForbiddenAreaTouch = false;
    public float forbiddenAreaDelay = 2f; // Задержка (секунд) для уничтожения авто пересекающего запретную зону
    private Coroutine forbiddenAreaTouchCor;

    private const string HotZoneTag = "HotZone"; // Для зоны с при въезде в которую происходит мгновенное уничтожение
    private bool isHotZoneTouch = false;

    [Header("Upgrade Levels for Agents")]
    [SerializeField, Range(0, 3)]
    private int nitrousLvl = 0;
    [SerializeField, Range(0, 3)]
    private int engineLvl = 0;
    [SerializeField, Range(0, 3)]
    private int armorLvl = 0;
    [SerializeField, Range(0, 3)]
    private int ammoLvl = 0;
    [SerializeField, Range(0, 3)]
    private int minesLvl = 0;

    [Space()]
    public Transform[] agentCheckPoints; // Агентские чекпоинты используем для определения оставшейся дистанции
    [HideInInspector] public int currentCheckPointInd = 0; // Текущий индекс массива для агентских чекпоинтов
    [HideInInspector] public Vector3 currentAgentCheckPoint; // Текущий агентский чекпоинт (следующий который нужно пересечь)
    [HideInInspector] public int currentLap = 1; // Текущий номер круга

    [Space()]
    public UnityEvent lapIsOver; // Круг окончен
    public UnityEvent pressNitrous; // Прожатие Нитро

    [System.Serializable]
    public class HpChangedEvent : UnityEvent<int, int> { } // Создаем тип события которе может передавать 2 параметра
    public HpChangedEvent hpIsChanged; // Кол-во hp изменилось

    private void Awake()
    {
        playerData = gameObject.AddComponent<PlayerData>();

        weaponController = GetComponent<WeaponController>();
        if (weaponController == null)
            Debug.Log("Error. WeaponController required");

        Upgrade();

        currentForwardForce = forwardForce;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
        rb.drag = drag;
        dragDelta = dragMax - drag;
        //mass = rb.mass;

        controls = new ControlManager();

        // Создаем Healthbar
        if (healthBar != null)
            healthBar.CreateHealthbar(hp);
        else
            Debug.Log(name + ": Healthbar not found");

    }

    private void Start()
    {
        ResetAgentCheckPoints();
        minimapMarker.SetActive(true);

        // Вычисляем расстояние от центра координат машины до земли
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, surfaceSearchMask);
        distanceToGround = transform.position - hit.point;

        currentHp = hp;

        if (!isAgent)
        {
            controls.Player.Respawn.performed += _ => Respawn();
            controls.Player.Jump.performed += _ => Jump();
            controls.Player.NOS.performed += _ => Nitrous();
            controls.Player.Fire.performed += _ => weaponController.Fire();
            controls.Player.LandMine.performed += _ => weaponController.SetMine();
            enabled = false;
        }
        else
        {
            agent = GetComponent<AgentController>();
        }
        
    }

    private void FixedUpdate()
    {
        Speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;

        if (isAgent)
        {
            if (agent.isExternalForce)
                return;
        }

        if (!isSpinOut)
        {
            if (!isAgent)
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
                        rb.AddRelativeForce(currentForwardForce, 0f, 0f);
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
                        PlayerRotation(currentRotationSpeed * turnSpeedRatio);
                    else
                        PlayerRotation(-currentRotationSpeed * turnSpeedRatio);
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
                    flipCor = StartCoroutine(Flip());
                }
            }
        }
    }

    // Метод возрвращает смещение относительно центра при респауне,
    // это нужно чтобы машины не респавнились в одну точку
    private Vector3 GetPlaceForRespawn()
    {
        GameObject placesForRespawnObj = GameObject.FindWithTag("PlacesForRespawn");

        if (placesForRespawnObj == null)
        {
            Debug.Log("GameObject placesForRespawn not found");
            return Vector3.zero;
        }

        PlacesForRespawn placesForRespawn = placesForRespawnObj.GetComponent<PlacesForRespawn>();

        if (placesForRespawn == null)
        {
            Debug.Log("Component PlacesForRespawn not found");
            return Vector3.zero;
        }

        return placesForRespawn.Place();
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

    private void Nitrous()
    {
        if (enabled && isCollision && !isNitrousOn && nitrousReady)
        {
            //Debug.Log("Nitrous активирован");
            isNitrousOn = true;
            nitrousReady = false;
            pressNitrous?.Invoke();
            currentForwardForce += nitrousForce;
            nitrousVFX?.Play();
            nitrousActionCor = StartCoroutine(NitrousAction());
        }
    }

    private IEnumerator NitrousAction()
    {
        yield return new WaitForSeconds(nitrousTime);

        NitrousStop();
    }

    private void NitrousStop()
    {
        currentForwardForce = forwardForce;
        isNitrousOn = false;
        nitrousVFX?.Stop();
    }

    public void Respawn()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Если включено нитро, то отключаем
        if (nitrousActionCor != null)
        {
            StopCoroutine(nitrousActionCor);
            NitrousStop();
        }

        // Ищем предыдущий чекпоинт
        Vector3 previousAgentCheckPoint;
        if (currentCheckPointInd - 1 < 0)
            previousAgentCheckPoint = agentCheckPoints[agentCheckPoints.Length - 1].position;
        else
            previousAgentCheckPoint = agentCheckPoints[currentCheckPointInd - 1].position;

        // Вычисляем новую позицию для машины
        PointOnLine respawnPoint = new PointOnLine(); // Точка ближайшая точка на прямой между текущим и предыдущим чекопоинтом
        Vector3 requiredPoint = respawnPoint.GetPointOnLine(previousAgentCheckPoint, currentAgentCheckPoint, transform.position); // Получаем ближайшую точку на прямой между чекпоинтами
        requiredPoint += GetPlaceForRespawn(); // Смещаем точку на свое заданное отклонение для респауна
        transform.position = requiredPoint + 100 * Vector3.up; // Максимально поднимаем точку над трассой чтобы пустить вниз луч и нащупать поверхность
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); // Меняем слой чтобы агент не обнаружил сам себя

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, surfaceSearchMask)) // Пускаем вниз луч, ищем поверхность
        {
            transform.position = hit.point + distanceToGround;
            //Debug.Log("Найдено пересечение в точке: " + hit.point + " " + hit.collider.name + "   transform.position = " + transform.position);
        }
        else
        {
            transform.position = requiredPoint + distanceToGround;
            //Debug.Log("Пересечение не найдено!");
        }
        Vector3 directionOnTarget = Vector3.ProjectOnPlane(currentAgentCheckPoint - previousAgentCheckPoint, Vector3.up); // Вычисляем направление на следующий чекпоинт спроецированное на поверхность
        transform.rotation = Quaternion.LookRotation(directionOnTarget) * Quaternion.AngleAxis(-90, Vector3.up);
        gameObject.layer = LayerMask.NameToLayer("Agent"); // После респауна возрващаем слой агента
        ResetCoroutines();
        
        currentHp = hp;
        healthBar.ChangeHP(hp, currentHp);
        hpIsChanged?.Invoke(hp, currentHp);
    }

    // Задаем вращение при наезде на маляное пятно. Запускается из OilStain
    public IEnumerator SpinOut(float force)
    {
        if (isAgent)
            agent.AddReward(-0.05f);

        isSpinOut = true;

        if (Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()) > 0)
            force = -force;

        rb.AddTorque(0f, force * Speed, 0f, ForceMode.Acceleration);
        yield return new WaitForSeconds(spinOutTimeRatio * Speed);
        isSpinOut = false;
    }

    public void HitHandler(int damage = 1)
    {
        currentHp -= damage;

        if (currentHp > 0)
        {
            healthBar.ChangeHP(hp, currentHp, true);
            hpIsChanged?.Invoke(hp, currentHp);
        }
        else
        {
            Respawn();
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
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));

        if (velocityAngle <= 90)
            rb.drag = drag + velocityAngle * dragDelta / 90f + penaltyDrag;
        else
            rb.drag = drag + (180 - velocityAngle) * dragDelta / 90f + penaltyDrag;
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

    private void OnCollisionExit(Collision collision)
    {
        isCollision = false;
        weaponController.groundCollision = false;
    }

    private void OnCollisionStay(Collision collision)
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
                    weaponController.groundCollision = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AgentCheckPoint") && other.transform == agentCheckPoints[currentCheckPointInd])
        {
            if (isAgent)
            {
                //agent.AddReward(1f / agentCheckPoints.Length);
                agent.AddReward(0.05f);
            }

            currentCheckPointInd++;

            if (currentCheckPointInd == agentCheckPoints.Length)
            {
                if (isAgent && agent.training)
                {
                    agent.EndEpisode();
                }
                else
                {
                    ResetAgentCheckPoints();
                    currentLap++;
                }

                lapIsOver?.Invoke();
                nitrousReady = true;
            }
            else
            {
                currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
            }
        }
        
        if (other.CompareTag(ForbiddenAreaTag) && isForbiddenAreaTouch == false)
        {
            isForbiddenAreaTouch = true;
            forbiddenAreaTouchCor = StartCoroutine(ForbiddenAreaTouch());
        }

        // Добавляем горячие зоны с моментальным уничтожением
        if (other.CompareTag(HotZoneTag) && isHotZoneTouch == false)
        {
            Debug.Log("HotZone!");
            isHotZoneTouch = true;
            StartCoroutine(HotZoneTouch());
        }
    }

    IEnumerator HotZoneTouch()
    {
        yield return new WaitForFixedUpdate(); // Чтобы исключить несколько одновременных срабатываний, нужно дождаться следующего FixedUpdate
        Respawn();
    }

    IEnumerator ForbiddenAreaTouch()
    {
        Debug.Log($"{playerName} ForbiddenAreaTouch");

        yield return new WaitForSeconds(forbiddenAreaDelay);
        Respawn();
    }

    // Функция принимает словарь со значением дистанций для всех чекпоинтов, длину круга и возвращает оставшиюся до финиша дистанцию
    public float GetRemainingDistance(Dictionary<Transform, float> checkpointDistances, float lapLength, int numberOfLaps)
    {
        Transform currentCheckPoint = agentCheckPoints[currentCheckPointInd];
        float distanceToCheckpoint = (agentCheckPoints[currentCheckPointInd].position - transform.position).magnitude;
        float remainingDistance = (numberOfLaps - currentLap) * lapLength + checkpointDistances[currentCheckPoint] + distanceToCheckpoint;

        return remainingDistance;
    }

    private void ResetAgentCheckPoints()
    {
        currentCheckPointInd = 0;
        currentAgentCheckPoint = agentCheckPoints[currentCheckPointInd].position;
    }

    IEnumerator Flip()
    {
        flipCheck = true;
        float flipStartTime = Time.time;
        
        // Начата проверка на переворот
        while (Time.time - flipStartTime < flipCheckDelay)
        {
            if (isCollision)
            {
                flipCheck = false;
                yield break;
            }
            else
            {
                yield return null;
            }
        }
        // Проверка закончена
        flipCheck = false;
        Debug.Log(name + " flip!");
        Respawn();
    }

    public IEnumerator SlimePenalty(float newPenaltyDrag, float modificationTime, float newTurnSpeedRatio)
    {
        if (isAgent)
            agent.AddReward(-0.05f);

        // Время воздействия в зависимости от текущей скорости. Чем выше скорость, тем дольше воздействие
        modificationTime *= (Speed / maxSpeed);

        // Меняем rb.drag и скорость поворота авто
        turnSpeedRatio = newTurnSpeedRatio;
        penaltyDrag = newPenaltyDrag;

        yield return new WaitForSeconds(modificationTime);

        penaltyDrag = 0f;
        turnSpeedRatio = 1f;
    }

    private void Upgrade()
    {
        if (!isAgent)
        {
            nitrousLvl = playerData.NitrousUpgradeLvl;
            engineLvl = playerData.EngineUpgradeLvl;
            armorLvl = playerData.ArmorUpgradeLvl;
            ammoLvl = playerData.AmmoUpgradeLvl;
            minesLvl = playerData.MinesUpgradeLvl;
        }

        nitrousForce += UpgradeRatio.nitrous * nitrousLvl;
        forwardForce += UpgradeRatio.engine * engineLvl;
        hp += UpgradeRatio.armor * armorLvl;
        weaponController.numberOfBullets += UpgradeRatio.ammo * ammoLvl;
        weaponController.numberOfMines += UpgradeRatio.mines * minesLvl;

        if (!isAgent)
        {
            Debug.Log($"engineLvl = {engineLvl}  forwardForce = {forwardForce}" +
                $"    nitrousLvl = {nitrousLvl}  nitrousForce = {nitrousForce}" +
                $"    armorLvl = {armorLvl}  hp = {hp}" +
                $"    ammoLvl = {ammoLvl}  numberOfBullets = {weaponController.numberOfBullets}" +
                $"    minesLvl = {minesLvl}  numberOfMines = {weaponController.numberOfMines}");
        }
    }

    public void Restart()
    {
        ResetCoroutines();
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.drag = drag;
        currentLap = 1;
        ResetAgentCheckPoints();
        currentHp = hp;
        healthBar.ChangeHP(hp, currentHp);
        hpIsChanged?.Invoke(hp, currentHp);
        weaponController.AmmoReload();
    }

    private void ResetCoroutines()
    {
        isForbiddenAreaTouch = false;
        isHotZoneTouch = false;
        flipCheck = false;
        isSpinOut = false;
        //rb.mass = mass;
        turnSpeedRatio = 1f;
        penaltyDrag = 0f;
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        controls.Enable();
        weaponController.AutoMode(true);
    }

    private void OnDisable()
    {
        controls.Disable();
        weaponController.AutoMode(false);     
    }
}
