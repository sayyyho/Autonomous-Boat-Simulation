using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(LineRenderer))]
public class PathFollower : MonoBehaviour
{
    private Vector3 lastPosition;
    public float CurrentSpeed { get; private set; } // m/s 단위 속도 노출

    public enum ShapeType { Triangle, Circle, Square }

    [Header("Main Waypoints (1st–5th)")]
    [Tooltip("Assign exactly 5 main waypoints in order.")]
    public List<Transform> mainWaypoints;

    [Header("Stop Settings")]
    [Tooltip("Seconds to wait at each main waypoint before next action.")]
    public float stopDuration = 3f;

    [Header("Shape Routing (at 2nd WP)")]
    [Tooltip("Choose the correct shape to follow.")]
    public ShapeType correctShape;
    [Tooltip("Define waypoint sequences for each shape between 2nd and 3rd WP.")]
    public List<ShapeRoute> shapeRoutes;

    [Serializable]
    public struct ShapeRoute
    {
        public ShapeType type;
        [Tooltip("Waypoints to travel if this shape is selected.")]
        public List<Transform> route;
    }

    [Header("Dynamic Obstacle Avoidance (from 3rd WP)")]
    [Tooltip("Distance to travel before re-planning path (meters).")]
    public float replanDistance = 5f;

    [Header("Motion Settings")]
    [Tooltip("Movement speed (m/s).")]
    public float speed = 3f;
    [Tooltip("Rotation speed (deg/s).")]
    public float rotationSpeed = 30f;
    [Tooltip("Distance threshold to consider waypoint reached.")]
    public float threshold = 0.1f;
    [Tooltip("Y coordinate for pathfinding grid.")]
    public float waterHeight = 0.6f;

    private AStarPathfinder pf;
    private LineRenderer lr;
    private Dictionary<ShapeType, List<Transform>> routeMap;

    // exposed for PathVisualizer
    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    public List<Vector3> CurrentPath => currentPath;
    public int PathIndex => pathIndex;

    private int collisionCount = 0; // 충돌 횟수 기록용
    private Dictionary<GameObject, float> lastCollisionTimes = new Dictionary<GameObject, float>();
    public float collisionCooldown = 1.0f; // 1초 동안 같은 장애물과 중복 집계 안 함

    private void OnCollisionEnter(Collision collision)
    {

        float now = Time.time;

        // 최근 충돌 시간이 있으면 쿨다운 체크
        if (lastCollisionTimes.TryGetValue(collision.gameObject, out float lastTime))
        {
            if (now - lastTime < collisionCooldown) return; // 쿨다운 중이면 무시
        }

        // 집계 및 쿨다운 시간 갱신
        collisionCount++;
        lastCollisionTimes[collision.gameObject] = now;
        Debug.LogWarning($"[경고] 충돌 발생! 총 {collisionCount}회 충돌."); // 상대: {collision.gameObject.name}
    }

    public int CollisionCount => collisionCount; // 외부에서 읽을 수 있게 프로퍼티 추가

    void Awake()
    {
        // find A* pathfinder
        pf = UnityEngine.Object.FindFirstObjectByType<AStarPathfinder>();
        // setup line renderer
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.widthMultiplier = 0.05f;

        // build shape-route map
        routeMap = new Dictionary<ShapeType, List<Transform>>();
        foreach (var entry in shapeRoutes)
            routeMap[entry.type] = entry.route;
    }

    void Start()
    {
        if (mainWaypoints.Count != 5)
        {
            Debug.LogError("PathFollower requires exactly 5 main waypoints set in inspector.");
            return;
        }
            
        lastPosition = transform.position; // 속도 측정 초기 위치 설정
        StartCoroutine(ExecuteMission());
    }

    void Update()
    {
        CurrentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

    // 콘솔 출력 (원하면 주석 처리 가능)
        Debug.Log($"[속도] 현재 속도 : {CurrentSpeed:F2} m/s");
    }

    // 경로 보간 함수 1
    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    // 경로 보간 함수 2
    private List<Vector3> SmoothPath(List<Vector3> path)
    {
        List<Vector3> smoothed = new List<Vector3>();

        if (path.Count < 4)
            return path; // 곡선 보간할 만큼 충분하지 않음

        for (int i = 0; i < path.Count - 3; i++)
        {
            Vector3 p0 = path[i];
            Vector3 p1 = path[i + 1];
            Vector3 p2 = path[i + 2];
            Vector3 p3 = path[i + 3];

            for (int j = 0; j < 10; j++) // 10등분해서 세밀하게 추가
            {
                float t = j / 10f;
                smoothed.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        return smoothed;
    }


    private IEnumerator ExecuteMission()
    {
        // 1st -> 2nd (standard A*)
        yield return MoveTo(mainWaypoints[0].position);
        yield return new WaitForSeconds(stopDuration);

        // 2nd wp: rotate to west smoothly
        yield return MoveTo(mainWaypoints[1].position);
        Quaternion targetRot = Quaternion.Euler(0f, 270f, 0f);
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot,
                rotationSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(stopDuration);

        // shape-based routing
        if (routeMap.TryGetValue(correctShape, out var subRoute))
        {
            Debug.Log($"Detected correct shape: {correctShape}");
            foreach (var wp in subRoute)
                yield return MoveTo(wp.position);
        }

        //2nd-> 3rd(standard A *)
        yield return MoveTo(mainWaypoints[2].position);
        yield return new WaitForSeconds(stopDuration);

        // 3rd -> 4th (dynamic avoidance via A* replan)
        yield return FollowDynamic(mainWaypoints[3].position);
        yield return new WaitForSeconds(stopDuration);

        // 4th -> 5th (dynamic avoidance)
        yield return FollowDynamic(mainWaypoints[4].position);
        yield return new WaitForSeconds(stopDuration);

        // clear renderer
        lr.positionCount = 0;
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        currentPath = pf.FindPath(
            new Vector3(transform.position.x, waterHeight, transform.position.z),
            new Vector3(target.x, waterHeight, target.z));

        currentPath = SmoothPath(currentPath);
        pathIndex = 0;

        lr.positionCount = currentPath.Count;
        for (int i = 0; i < currentPath.Count; i++)
            lr.SetPosition(i, currentPath[i] + Vector3.up * 0.1f);

        while (pathIndex < currentPath.Count)
        {
            Vector3 node = currentPath[pathIndex];
            Vector3 dir = (node - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
                //Debug.Log("회전 속도: " + rotationSpeed);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(node.x, transform.position.y, node.z),
                speed * Time.deltaTime);

            if (Vector3.Distance(transform.position,
                new Vector3(node.x, transform.position.y, node.z)) < threshold)
                pathIndex++;

            yield return null;
        }
    }

    private IEnumerator FollowDynamic(Vector3 target)
    {
        Vector3 goal = new Vector3(target.x, waterHeight, target.z);
        float traveled = 0f;
        Vector3 lastPos = transform.position;

        while (Vector3.Distance(transform.position, goal) > threshold)
        {
            // 1) A* 경로 생성
            currentPath = pf.FindPath(
                new Vector3(transform.position.x, waterHeight, transform.position.z),
                goal
            );

            // --- 여기서 경로가 없으면 루프 탈출 ---
            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogWarning("[FollowDynamic] 경로를 찾을 수 없습니다. 동적 회피를 중단합니다.");
                yield break;
            }
            // ------------------------------------

            pathIndex = 0;
            traveled = 0f;
            lastPos = transform.position;

            // 2) 이동하면서 replanDistance만큼 이동할 때마다 재탐색
            while (pathIndex < currentPath.Count && traveled < replanDistance)
            {
                Vector3 node = currentPath[pathIndex];
                Vector3 dir  = (node - transform.position).normalized;
                if (dir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        Quaternion.LookRotation(dir),
                        rotationSpeed * Time.deltaTime
                    );

                Vector3 nextPos = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(node.x, transform.position.y, node.z),
                    speed * Time.deltaTime
                );
                traveled += Vector3.Distance(transform.position, nextPos);
                transform.position = nextPos;

                if (Vector3.Distance(transform.position,
                    new Vector3(node.x, transform.position.y, node.z)) < threshold)
                {
                    pathIndex++;
                }
                yield return null;
            }
        }
    }
}
