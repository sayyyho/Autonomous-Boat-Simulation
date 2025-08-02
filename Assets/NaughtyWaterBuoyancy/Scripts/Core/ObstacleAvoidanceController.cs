using UnityEngine;

[RequireComponent(typeof(LidarSimulator))]
public class ObstacleAvoidanceController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float turnSpeed = 100f;
    public float safeDistance = 2f;

    private Vector3 target;
    private LidarSimulator lidar;

    void Start()
    {
        lidar = GetComponent<LidarSimulator>();
    }

    public void SetTarget(Vector3 t)
    {
        target = t;
    }

    public void Navigate()
    {
        // 간단한 회피 전략: 정면 30도 이내에 장애물 감지 시 우회전
        int center = lidar.raysCount / 2;
        int checkRange = 30;

        bool obstacleDetected = false;
        for (int i = center - checkRange / 2; i <= center + checkRange / 2; i++)
        {
            int idx = (i + lidar.raysCount) % lidar.raysCount;
            if (lidar.distances[idx] < safeDistance)
            {
                obstacleDetected = true;
                break;
            }
        }

        if (obstacleDetected)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.LookAt(target);
        }
    }
}
