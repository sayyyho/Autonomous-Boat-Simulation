using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleAutonomousBoat : MonoBehaviour
{
    public Transform[] waypoints;      // 반드시 지나야 하는 지점들
    public float moveSpeed = 3f;
    public float turnSpeed = 80f;
    public float arriveThreshold = 1.0f;

    private int currentTarget = 0;
    private Rigidbody rb;
    private LidarSimulator lidar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lidar = GetComponent<LidarSimulator>();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0 || currentTarget >= waypoints.Length)
            return;

        Vector3 targetPos = waypoints[currentTarget].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        // 장애물 감지 처리
        float frontDistance = GetLidarDistanceAtAngle(0);
        float leftDistance = GetLidarDistanceAtAngle(340); // 왼쪽 방향
        float rightDistance = GetLidarDistanceAtAngle(20); // 오른쪽 방향

        // 장애물 회피 로직 (간단한 반사형 회피)
        if (frontDistance < 2f)
        {
            if (leftDistance > rightDistance)
                Turn(-1); // 왼쪽 회피
            else
                Turn(1); // 오른쪽 회피
        }
        else
        {
            // 목표 방향 회전
            Quaternion targetRot = Quaternion.LookRotation(direction);
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRot);

            // 전진
            rb.AddForce(transform.forward * moveSpeed, ForceMode.Force);
        }

        // 도착 체크
        float distanceToWaypoint = Vector3.Distance(transform.position, targetPos);
        if (distanceToWaypoint < arriveThreshold)
        {
            currentTarget++;
        }
    }

    void Turn(int dir)
    {
        rb.AddTorque(0f, dir * turnSpeed * 0.5f, 0f, ForceMode.Force);
    }

    float GetLidarDistanceAtAngle(int angle)
    {
        if (lidar == null || lidar.distances == null || lidar.distances.Length == 0)
            return Mathf.Infinity;

        int index = angle % lidar.distances.Length;
        return lidar.distances[index];
    }
}
