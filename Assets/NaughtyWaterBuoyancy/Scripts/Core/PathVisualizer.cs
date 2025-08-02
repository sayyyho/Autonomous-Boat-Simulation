using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    [Tooltip("PathFollower instance")] public PathFollower follower;
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.widthMultiplier = 0.05f;    // 선 두께 조정
        lr.positionCount = 0;

    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    void Update()
    {
        var path=follower.CurrentPath;
        if (path == null || path.Count < 4)
        {
            lr.positionCount = 0;
            return;
        }


        int idx = Mathf.Clamp(follower.PathIndex - 1, 0, path.Count - 4); // 최소 4개 점 필요
        List<Vector3> smoothPoints = new List<Vector3>();

        for (int i = idx; i < path.Count - 3; i++)
        {
            Vector3 p0 = path[i];
            Vector3 p1 = path[i + 1];
            Vector3 p2 = path[i + 2];
            Vector3 p3 = path[i + 3];

            for (int j = 0; j <= 10; j++) // 10등분
            {
                float t = j / 10f;
                Vector3 pos = CatmullRom(p0, p1, p2, p3, t);
                pos.y += 0.1f; // 살짝 위로 올리기
                smoothPoints.Add(pos);
            }
        }

        lr.positionCount = smoothPoints.Count;
        lr.SetPositions(smoothPoints.ToArray());
    }
}
