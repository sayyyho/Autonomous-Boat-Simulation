using UnityEngine;
using UnityEngine.UI;

public class ObstacleAutoSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("생성할 장애물 복제 개수")]
    public int obstacleCount = 5;

    [Tooltip("무작위 배치 시 기준이 되는 중심점")]
    public Vector3 spawnCenter = Vector3.zero;

    [Tooltip("spawnCenter 주변에 장애물을 배치할 범위 (가로, 세로, 깊이)")]
    public Vector3 spawnSize = new Vector3(10, 0, 10);

    [Header("UI References (선택 사항)")]
    [Tooltip("장애물 개수를 입력받을 InputField (유니티 UI InputField)")]
    public InputField countInput;

    [Tooltip("장애물 생성 버튼")]
    public Button spawnButton;

    [Header("Spawning Control")]
    [Tooltip("true면 장애물을 스폰할 수 있고, false면 추가 스폰 안 함 (복제 무한 루프 방지)")]
    public bool allowSpawning = true;

    void Start()
    {
        // UI 버튼이 연결되어 있다면, 버튼 클릭 시 SpawnObstaclesFromUI를 호출
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(SpawnObstaclesFromUI);
        }
    }

    /// <summary>
    /// InputField에서 장애물 개수를 받아온 후 SpawnObstacles()를 실행합니다.
    /// </summary>
    public void SpawnObstaclesFromUI()
    {
        if (countInput != null)
        {
            int parsedCount;
            if (int.TryParse(countInput.text, out parsedCount))
            {
                obstacleCount = parsedCount;
            }
        }

        SpawnObstacles();
    }

    /// <summary>
    /// obstacleCount만큼 현재 오브젝트(this.gameObject)를 지정한 범위 내에서 무작위로 생성합니다.
    /// 새로 생성된 복제본은 allowSpawning을 false로 하여 추가 스폰을 막습니다.
    /// </summary>
    public void SpawnObstacles()
    {
        // 무한 복제를 방지하기 위해, allowSpawning이 false면 실행하지 않음
        if (!allowSpawning) return;

        for (int i = 0; i < obstacleCount; i++)
        {
            // spawnSize 범위 내에서 무작위 오프셋 생성
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnSize.x / 2, spawnSize.x / 2),
                Random.Range(-spawnSize.y / 2, spawnSize.y / 2),
                Random.Range(-spawnSize.z / 2, spawnSize.z / 2)
            );

            // 실제 스폰 위치 계산
            Vector3 spawnPos = spawnCenter + randomOffset;

            // 자기 자신(this.gameObject)을 복제
            GameObject clone = Instantiate(gameObject, spawnPos, Quaternion.identity);

            // 새로 생성된 복제본의 스크립트에서 allowSpawning을 false로 설정하여, 
            // 해당 복제본은 추가 스폰하지 않도록 함.
            ObstacleAutoSpawner spawner = clone.GetComponent<ObstacleAutoSpawner>();
            if (spawner != null)
            {
                spawner.allowSpawning = false;
            }
        }
    }
}