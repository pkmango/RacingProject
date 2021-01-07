using UnityEngine;

public class PlayerController : MonoBehaviour
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
    private Vector3 spawnPoint;

    void Start()
    {
        spawnPoint = gameObject.transform.position;

        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;

        rb.drag = drag;
        dragDelta = dragMax - drag;
    }

    void Update()
    {
        if (isCollision)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(0f, jumpForce, 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.transform.position = spawnPoint;
            gameObject.transform.rotation = Quaternion.identity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
    }

    void FixedUpdate()
    {
        if (isCollision)
        {
            addedFlyTorque = false;
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0f, rb.angularVelocity.z);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (firstAcceleration)
                {
                    currentRotationSpeed = 0f;
                    firstAcceleration = false;
                }

                if (Input.GetKey(KeyCode.W))
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

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                TurningWheels();

                if (Input.GetKey(KeyCode.D))
                    PlayerRotation(currentRotationSpeed);
                else
                    PlayerRotation(-currentRotationSpeed);
            }
            else
            {
                currentRotationSpeed = 0f;
            }
        }
        else
        {
            if (!addedFlyTorque)
            {
                FlyBehavior();
                addedFlyTorque = true;
            }
        }

        if (rb.velocity.magnitude != 0f)
            ChangeDrag();

        //Debug.Log(Mathf.DeltaAngle(rb.rotation.eulerAngles.y, VelocityAngle()));
    }

    void PlayerRotation(float _rotationSpeed)
    {
        Vector3 newRotationAngle = rb.rotation.eulerAngles;

        if (Input.GetKey(KeyCode.W))
        {
            newRotationAngle += new Vector3(0f, _rotationSpeed, 0f);
        }else if (Input.GetKey(KeyCode.S))
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

    float VelocityAngle()
    {
        float velocityAngle = 0f;
        float magnitudeXZ = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);

        if (magnitudeXZ != 0f)
        {
            if (rb.velocity.z > 0f)
                velocityAngle = 360f - Mathf.Acos(rb.velocity.x / magnitudeXZ) * Mathf.Rad2Deg;
            else
                velocityAngle = Mathf.Acos(rb.velocity.x / magnitudeXZ) * Mathf.Rad2Deg;
        }

        return velocityAngle;
    }

    void FlyBehavior()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeTorque(0f, -flyTorque, 0f);
        }

        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
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
                    isCollision = true;
            }
        }  
    }
}
