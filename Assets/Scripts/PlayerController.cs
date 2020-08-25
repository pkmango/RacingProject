using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardForce;
    public float backForce;
    public float torque;
    public float jumpForce;
    public string wheelTag = "Wheel";

    private Rigidbody rb;
    [SerializeField] private bool isCollision = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
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
        
    }

    void FixedUpdate()
    {
        if (isCollision)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddRelativeForce(forwardForce, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddRelativeForce(backForce, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddRelativeTorque(0f, torque, 0f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddRelativeTorque(0f, -torque, 0f);
            }
        }
        
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    isCollision = true;
    //}

    void OnCollisionExit(Collision collision)
    {
        isCollision = false;
    }

    void OnCollisionStay(Collision collision)
    {
        //Collider myCollider = collision.contacts[0].thisCollider;
        //Debug.Log(myCollider.gameObject.tag);

        //Debug.Log(collision.GetContact(0).thisCollider.tag);

        if (collision.GetContact(0).thisCollider.tag == wheelTag)
        {
            isCollision = true;
        }
        else
        {
            isCollision = false;
        }

        
    }
}
