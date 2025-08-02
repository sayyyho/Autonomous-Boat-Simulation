using UnityEngine;
using UnityEngine.UI;

public class LidarRadarDisplay : MonoBehaviour
{
    [Header("References")]
    public LidarSimulator lidar;      // LiDAR 데이터 (Cube에 붙은 스크립트)
    public RectTransform radarPanel;  // Radar Panel (RectTransform)
    public GameObject dotPrefab;      // 장애물 점(Dot) 프리팹 (UI Image)
    
    [Header("Radar Settings")]
    public float radarRadius = 100f;  // UI 상에서의 최대 반경 (픽셀 단위)
    private float maxDistance;        // lidar.maxDistance를 가져올 예정

    // Ray 개수만큼 점을 생성해 관리
    private GameObject[] dots;

    [Header("Rotation & Center Dot")]
    public float angleOffset = 180f;  // 레이더 전체를 180도 뒤집기 위한 각도 오프셋
    public bool showCenterDot = true; // 중앙에 "보트 점" 표시 여부
    public GameObject boatDotPrefab;  // 보트 점(Dot) 프리팹
    private GameObject boatDot;

    void Start()
    {
        // LiDAR 최대 거리 가져오기
        if (lidar != null)
        {
            maxDistance = lidar.maxDistance;
        }

        // Ray 개수만큼 Dot 생성
        if (lidar != null && dotPrefab != null)
        {
            dots = new GameObject[lidar.raysCount];
            for (int i = 0; i < lidar.raysCount; i++)
            {
                // Panel의 자식으로 Dot Prefab 생성
                dots[i] = Instantiate(dotPrefab, radarPanel);
            }
        }

        // 중앙에 보트 점 찍기 (옵션)
        if (showCenterDot && boatDotPrefab != null)
        {
            boatDot = Instantiate(boatDotPrefab, radarPanel);
            // 중앙에 배치 (0,0)
            RectTransform brt = boatDot.GetComponent<RectTransform>();
            brt.anchoredPosition = Vector2.zero;
        }
    }

    void Update()
    {
        if (lidar == null || dots == null) return;

        // 각도별 LiDAR 거리값을 읽어 UI 위치 갱신
        for (int i = 0; i < lidar.raysCount; i++)
        {
            float dist = lidar.distances[i]; // 0 ~ maxDistance
            float ratio = dist / maxDistance; // 0~1 정규화
            float r = radarRadius * ratio;    // UI상 반지름

            // 각도 계산 (기본 0~360) + 오프셋(180도)
            float angle = i * (360f / lidar.raysCount) + angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            float x = r * Mathf.Cos(rad);
            float y = r * Mathf.Sin(rad);
            x = -x;
            // Dot의 RectTransform에 좌표 반영
            RectTransform rt = dots[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(x, y);
        }
    }
}
