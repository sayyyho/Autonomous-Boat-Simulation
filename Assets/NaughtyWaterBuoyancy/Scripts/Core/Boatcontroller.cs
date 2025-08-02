using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    public float moveSpeed = 5f;      // 전진/후진 속도
    public float turnSpeed = 50f;     // 회전 속도

    private Rigidbody rb;

    public float CurrentSpeed => rb.linearVelocity.magnitude;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Unity 기본 축
        // Vertical : W(1)/S(-1), Horizontal : A(-1)/D(1)
        float moveInput = Input.GetAxis("Vertical");    // 전진/후진
        float turnInput = Input.GetAxis("Horizontal");  // 좌/우 회전

        // 전진/후진
        Vector3 forwardForce = transform.forward * moveInput * moveSpeed;
        rb.AddForce(forwardForce, ForceMode.Force);

        // 회전 (yaw)
        float turnTorque = turnInput * turnSpeed;
        rb.AddTorque(0f, turnTorque, 0f, ForceMode.Force);
    }
}
