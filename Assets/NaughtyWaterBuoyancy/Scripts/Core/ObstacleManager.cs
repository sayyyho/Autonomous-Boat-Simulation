using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Vector2 areaSize = new Vector2(50f, 30f);
    [SerializeField] private float gridCellSize = 2f;
    [SerializeField] private float obstacleHeight = 0.5f;

    public void GenerateObstacles(int count)
    {
        ClearObstacles();
        
        // 격자 계산
        int gridX = Mathf.FloorToInt(areaSize.x / gridCellSize);
        int gridZ = Mathf.FloorToInt(areaSize.y / gridCellSize);
        int maxPossible = gridX * gridZ;
        count = Mathf.Clamp(count, 1, maxPossible);

        // 격자 위치 생성 및 셔플
        Vector2Int[] gridPositions = new Vector2Int[gridX * gridZ];
        for (int x = 0, i = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++, i++)
            {
                gridPositions[i] = new Vector2Int(x, z);
            }
        }

        // Fisher-Yates 셔플
        for (int i = 0; i < gridPositions.Length; i++)
        {
            int r = Random.Range(i, gridPositions.Length);
            (gridPositions[r], gridPositions[i]) = (gridPositions[i], gridPositions[r]);
        }

        // 장애물 생성
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos = gridPositions[i];
            Vector3 spawnPos = new Vector3(
                pos.x * gridCellSize - areaSize.x / 2 + gridCellSize / 2,
                obstacleHeight,
                pos.y * gridCellSize - areaSize.y / 2 + gridCellSize / 2
            );

            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
        }
    }

    private void ClearObstacles()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}