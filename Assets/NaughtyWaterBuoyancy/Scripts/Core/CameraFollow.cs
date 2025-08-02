using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 따라갈 대상 (예: 보트)
    public Vector3 offset = new Vector3(0f, 5f, -10f); // 카메라가 떨어진 거리
    public float followSpeed = 5f; // 카메라가 따라가는 속도
    public bool lookAtTarget = true; // 대상 바라볼지 여부

    void LateUpdate()
    {
        if (target == null) return;

        // 원하는 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 부드럽게 이동 (Lerp)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 대상 바라보기
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
