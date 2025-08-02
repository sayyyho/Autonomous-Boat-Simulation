using UnityEngine;
using System.Collections.Generic;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("장애물 설정")]
    public GameObject cylinderPrefab;
    [Range(1, 50)] public int obstacleCount = 10;
    public float obstacleHeight = 0.5f;
    public Vector3 obstacleScale = new Vector3(0.2f, 0.5f, 0.2f);
    public float cellPadding = 0.1f; // 장애물 간격

    [Header("생성 기준 위치 및 영역")]
    public Vector3 origin = Vector3.zero;  // 기준 위치 추가
    public Vector2 areaSize = new Vector2(50f, 30f);
    public float gridCellSize = 0.5f;

    [Header("마크 설정")]
    public GameObject markPrefab;
    public float markOffset = 0.5f;
    public Color selectedColor = Color.red;
    public Color defaultColor = Color.white;

    [Header("제외할 위치")]
    public Transform waypoint2;
    public float exclusionRadius = 1.0f;  // 제외할 반경


    private List<GameObject> obstacles = new List<GameObject>();
    private Dictionary<GameObject, GameObject> obstacleMarks = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        GenerateObstacles(waypoint2.position, exclusionRadius);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.parent == this.transform)
            {
                ToggleObstacleSelection(hit.transform.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateObstacles(waypoint2.position, exclusionRadius);
        }

    }

    public void GenerateObstacles(Vector3 waypoint2Position, float excludeRadius)
    {
        ClearExistingObstacles();

        int gridX = Mathf.FloorToInt(areaSize.x / gridCellSize);
        int gridZ = Mathf.FloorToInt(areaSize.y / gridCellSize);
        obstacleCount = Mathf.Clamp(obstacleCount, 1, gridX * gridZ);

        List<Vector2Int> gridPositions = new List<Vector2Int>();
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                Vector3 spawnPos = new Vector3(
                    (x * gridCellSize) - (areaSize.x / 2) + (gridCellSize / 2),
                    obstacleHeight,
                    (z * gridCellSize) - (areaSize.y / 2) + (gridCellSize / 2)
                ) + origin;

                // Waypoint2와의 거리가 excludeRadius 이상인 것만 추가
                if (Vector3.Distance(spawnPos, waypoint2Position) >= excludeRadius)
                {
                    gridPositions.Add(new Vector2Int(x, z));
                }
            }
        }

        // 섞기 (Fisher-Yates Shuffle)
        for (int i = 0; i < gridPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, gridPositions.Count);
            Vector2Int temp = gridPositions[randomIndex];
            gridPositions[randomIndex] = gridPositions[i];
            gridPositions[i] = temp;
        }

        int actualCount = Mathf.Min(obstacleCount, gridPositions.Count);
        for (int i = 0; i < actualCount; i++)
        {
            Vector2Int gridPos = gridPositions[i];
            Vector3 spawnPos = new Vector3(
                (gridPos.x * gridCellSize) - (areaSize.x / 2) + (gridCellSize / 2),
                obstacleHeight,
                (gridPos.y * gridCellSize) - (areaSize.y / 2) + (gridCellSize / 2)
            );

            spawnPos += origin;

            GameObject newObstacle = Instantiate(cylinderPrefab, spawnPos, Quaternion.identity);
            newObstacle.transform.localScale = obstacleScale;
            newObstacle.transform.SetParent(this.transform);

            if (newObstacle.GetComponent<Collider>() == null)
            {
                newObstacle.AddComponent<BoxCollider>();
            }

            SetObstacleColor(newObstacle, defaultColor);
            obstacles.Add(newObstacle);
        }
    }


    void ToggleObstacleSelection(GameObject obstacle)
    {
        if (obstacleMarks.ContainsKey(obstacle))
        {
            SetObstacleColor(obstacle, defaultColor);
            Destroy(obstacleMarks[obstacle]);
            obstacleMarks.Remove(obstacle);
        }
        else
        {
            SetObstacleColor(obstacle, selectedColor);
            CreateMark(obstacle);
        }
    }

    void CreateMark(GameObject obstacle)
    {
        int direction = Random.Range(0, 4);
        Vector3 markPosition = obstacle.transform.position;

        switch (direction)
        {
            case 0: markPosition += Vector3.forward * markOffset; break;
            case 1: markPosition += Vector3.back * markOffset; break;
            case 2: markPosition += Vector3.left * markOffset; break;
            case 3: markPosition += Vector3.right * markOffset; break;
        }

        GameObject mark = Instantiate(markPrefab, markPosition, Quaternion.identity);
        mark.transform.SetParent(this.transform);
        mark.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Collider markCollider = mark.GetComponent<Collider>();
        if (markCollider != null) Destroy(markCollider);

        obstacleMarks.Add(obstacle, mark);
    }

    void SetObstacleColor(GameObject obstacle, Color color)
    {
        Renderer renderer = obstacle.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material newMat = new Material(Shader.Find("Standard")); // 또는 URP용 셰이더
            newMat.color = color;
            renderer.material = newMat;
        }
    }

    void ClearExistingObstacles()
    {
        foreach (var pair in obstacleMarks)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value);
            }
        }
        obstacleMarks.Clear();

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        obstacles.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector3(origin.x, obstacleHeight, origin.z),
            new Vector3(areaSize.x, 0.1f, areaSize.y)
        );
    }

    public void ClearAllSelections()
    {
        List<GameObject> keys = new List<GameObject>(obstacleMarks.Keys);
        foreach (GameObject obstacle in keys)
        {
            SetObstacleColor(obstacle, defaultColor);
            Destroy(obstacleMarks[obstacle]);
        }
        obstacleMarks.Clear();
    }
}
