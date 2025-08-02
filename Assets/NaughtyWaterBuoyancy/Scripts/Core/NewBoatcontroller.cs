using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NewBoatController : MonoBehaviour
{
    public float moveSpeed = 5f;      // 전진/후진 속도
    public float turnSpeed = 50;     // 회전 속도

    private Rigidbody rb;

    public float CurrentSpeed => rb.linearVelocity.magnitude;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");    // W/S
        float turnInput = Input.GetAxis("Horizontal");  // A/D

        if (Mathf.Approximately(moveInput, 0f) && Mathf.Approximately(turnInput, 0f))
        {
            // 입력 없을 때 속도/회전속도 강제로 0으로 설정
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // 전진/후진
        Vector3 forwardForce = transform.forward * moveInput * moveSpeed;
        rb.AddForce(forwardForce, ForceMode.Force);

        // 회전 (Yaw)
        float turnTorque = turnInput * turnSpeed;
        rb.AddTorque(0f, turnTorque, 0f, ForceMode.Force);
    }
}
