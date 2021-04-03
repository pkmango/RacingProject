using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentController : Agent
{
    public PlayerController car;
    public bool training; // Тренировка? Для тренировки агента нужно включить
    public Transform[] startPositions;
    private Rigidbody rb;
    private float maxSpeedForNormalization = 69f; // Максимальная возможная скорость агента, нужно для нормализации
    public bool heuristicAlowed;

    //[HideInInspector] public bool finished = false; // Агент финишировал? Статус используется в GameController
    [HideInInspector] public DateTime raceTime = new DateTime(); // Общее время со старта гонки передается из GameController

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        car.Restart();

        if (training)
        {
            int randomInd = Random.Range(0, startPositions.Length - 1);
            transform.position = startPositions[randomInd].position;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Вычисляем угол между направлением скорости и поворотом авто
        float velocityAngle = Mathf.Abs(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, car.VelocityAngle()));
        sensor.AddObservation(velocityAngle / 360);

        // Скалярное произведение показывает правильность направления движения
        Vector3 direction = (car.currentAgentCheckPoint - transform.position).normalized;
        sensor.AddObservation(Vector3.Dot(rb.velocity.normalized, direction));

        // Quaternion.Dot показывает поворот агента относительно направления на чекпоинт
        sensor.AddObservation(Quaternion.Dot(rb.rotation, Quaternion.LookRotation(direction)));

        // Текущая нормализованная скорость в плоскости x-z
        sensor.AddObservation(car.Speed / maxSpeedForNormalization);
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

    private void ActionHandler(ActionSegment<int> act)
    {
        AddReward(-0.0002f);
        int rotateAction = act[0];
        int forwardAction = act[1];
        //Debug.Log("Action");
        if (rotateAction == 1) // Поворот налево
        {
            car.leftBtnOn = true;
            car.rightBtnOn = false;
        }
        else if (rotateAction == 2) // Поворот направо
        {
            car.leftBtnOn = false;
            car.rightBtnOn = true;
        }
        else
        {
            car.leftBtnOn = false;
            car.rightBtnOn = false;
        }

        if (forwardAction == 1) // Газуем вперед
        {
            car.gasOn = true;
            car.reverseOn = false;
        }
        else if (forwardAction == 2) // Газуем назад
        {
            car.gasOn = false;
            car.reverseOn = true;
        }
        else
        {
            car.gasOn = false;
            car.reverseOn = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            AddReward(-0.015f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (heuristicAlowed)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut.Clear();

            if (car.controls.Player.Right.ReadValue<float>() > 0)
            {
                discreteActionsOut[0] = 2;
            }
            if (car.controls.Player.Gas.ReadValue<float>() > 0)
            {
                discreteActionsOut[1] = 1;
            }
            if (car.controls.Player.Left.ReadValue<float>() > 0)
            {
                discreteActionsOut[0] = 1;
            }
            if (car.controls.Player.Reverse.ReadValue<float>() > 0)
            {
                discreteActionsOut[1] = 2;
            }
        }
    }
}
