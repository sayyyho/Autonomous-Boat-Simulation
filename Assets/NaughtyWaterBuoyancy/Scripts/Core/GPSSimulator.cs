using UnityEngine;
using UnityEngine.UI;

public class GPSSimulator : MonoBehaviour
{
    [Header("GPS Settings")]
    [Tooltip("보트의 기준 원점. 예를 들어 (0,0,0) 또는 원하는 시작 좌표")]
    public Vector3 origin = Vector3.zero;

    [Tooltip("Unity 좌표를 몇 배로 표시할지 결정 (1 = 1:1)")]
    public float scale = 1f;

    [Header("UI Reference")]
    [Tooltip("GPS 정보를 표시할 UI Text")]
    public Text gpsDisplay; // Text 대신 TMPro.TMP_Text를 사용할 수도 있습니다.

    // 이전 위치 저장용
    private Vector3 lastPosition;
    private float currentSpeed = 0f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // 1. 위치 연산
        Vector3 relativePos = transform.position - origin;
        float gpsX = relativePos.x * scale;
        float gpsZ = relativePos.z * scale;

        // 2. 속도 계산 (프레임당 거리 / 시간)
        float distance = Vector3.Distance(transform.position, lastPosition);
        currentSpeed = distance / Time.deltaTime; // 단위: m/s

        // 3. UI 표시
        if (gpsDisplay != null)
        {
            gpsDisplay.text = string.Format(
                "GPS \nX: {0:F2} m\nZ: {1:F2} m\nSpeed: {2:F2} m/s",
                gpsX, gpsZ, currentSpeed);
        }

        // 4. 위치 갱신
        lastPosition = transform.position;
    }
}
