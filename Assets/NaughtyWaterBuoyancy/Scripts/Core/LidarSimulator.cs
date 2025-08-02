using UnityEngine;

public class LidarSimulator : MonoBehaviour
{
    [Header("LiDAR Settings")]
    public int raysCount = 360;    // 몇 개의 Ray를 발사할지 (1도 단위면 360)
    public float maxDistance = 20f; // LiDAR 최대 감지 거리

    // 외부에서 참조하기 위해 public 배열로 선언
    public float[] distances;

    void Start()
    {
        distances = new float[raysCount];
    }

    void Update()
    {
        for (int i = 0; i < raysCount; i++)
        {
            // 각도 계산 (y축 기준 회전)
            float angle = i * (360f / raysCount);
            // angle만큼 회전한 방향 벡터 (전방 Vector3.forward 기준)
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            // Raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, maxDistance))
            {
                // 충돌 시, 충돌 거리 기록
                distances[i] = hit.distance;
            }
            else
            {
                // 충돌 없으면 최대 거리로
                distances[i] = maxDistance;
            }

            // Scene 뷰에서 Ray를 그려보고 싶다면 (디버그용)
            Debug.DrawRay(transform.position, dir * distances[i], Color.red);
        }
    }
}
