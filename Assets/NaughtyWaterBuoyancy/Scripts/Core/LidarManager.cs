using UnityEngine;
using System.Collections.Generic;

public class LidarManager : MonoBehaviour
{
    public static LidarManager Instance { get; private set; }

    [Header("Base Settings")]
    public GameObject lidarDotPrefab;
    public GameObject boatDotPrefab;
    public float radarRadius = 100f;
    public bool visualizeRays = true;

    private Dictionary<int, LidarSensor> sensors = new Dictionary<int, LidarSensor>();
    private int nextSensorId = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int RegisterSensor(LidarSensor sensor)
    {
        int sensorId = nextSensorId++;
        sensors.Add(sensorId, sensor);
        return sensorId;
    }

    public void UpdateRadarDisplay(int sensorId, float[] distances)
    {
        if (sensors.TryGetValue(sensorId, out LidarSensor sensor))
        {
            sensor.UpdateDisplay(distances);
        }
    }

    public GameObject CreateRadarDot(Transform parent)
    {
        return Instantiate(lidarDotPrefab, parent);
    }

    public GameObject CreateBoatDot(Transform parent)
    {
        return Instantiate(boatDotPrefab, parent);
    }
}