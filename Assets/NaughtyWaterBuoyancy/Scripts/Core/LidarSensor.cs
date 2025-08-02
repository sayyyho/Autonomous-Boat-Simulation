
using UnityEngine;

public class LidarSensor : MonoBehaviour
{
    [Header("Sensor Configuration")]
    public int raysCount = 360;
    public float maxDistance = 10f;
    public float scanFrequency = 10f;
    public LayerMask detectionLayers;

    [Header("Display Settings")]
    public RectTransform radarPanel;
    public float angleOffset = 180f;

    private float[] distances;
    private float scanTimer;
    private int sensorId;
    private GameObject[] dots;
    private GameObject boatDot;

    private void Start()
    {
        distances = new float[raysCount];
        sensorId = LidarManager.Instance.RegisterSensor(this);
        InitializeDisplay();
    }

    private void InitializeDisplay()
    {
        if (radarPanel == null) return;

        dots = new GameObject[raysCount];
        for (int i = 0; i < raysCount; i++)
        {
            dots[i] = LidarManager.Instance.CreateRadarDot(radarPanel);
        }

        boatDot = LidarManager.Instance.CreateBoatDot(radarPanel);
        boatDot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer >= 1f / scanFrequency)
        {
            scanTimer = 0f;
            PerformScan();
            LidarManager.Instance.UpdateRadarDisplay(sensorId, distances);
        }
    }

    private void PerformScan()
    {
        for (int i = 0; i < raysCount; i++)
        {
            float angle = i * (360f / raysCount);
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, maxDistance, detectionLayers))
            {
                distances[i] = hit.distance;
                if (LidarManager.Instance.visualizeRays)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red, 1f/scanFrequency);
                }
            }
            else
            {
                distances[i] = maxDistance;
                if (LidarManager.Instance.visualizeRays)
                {
                    Debug.DrawRay(transform.position, dir * maxDistance, Color.green, 1f/scanFrequency);
                }
            }
        }
    }

    public void UpdateDisplay(float[] distances)
    {
        if (radarPanel == null || dots == null) return;

        for (int i = 0; i < raysCount; i++)
        {
            float ratio = distances[i] / maxDistance;
            float r = LidarManager.Instance.radarRadius * ratio;
            float angle = i * (360f / raysCount) + angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            
            RectTransform rt = dots[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(-r * Mathf.Cos(rad), r * Mathf.Sin(rad));
        }
    }
}