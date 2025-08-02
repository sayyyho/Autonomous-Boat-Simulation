using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AStarPathfinder : MonoBehaviour
    {
        [Tooltip("Layer(s) to consider as obstacles")] public LayerMask obstacleMask;
        [Tooltip("Spacing between grid nodes in meters")] public float nodeSpacing = 1f;
        [Tooltip("Size of the search area (X,Z)")] public Vector2 gridSize = new Vector2(36, 11);
        [Tooltip("Y coordinate of the water surface")] public float waterHeight = 0.6f;

        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            Vector3[] directions = {
                Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
                Vector3.forward + Vector3.left, Vector3.forward + Vector3.right,
                Vector3.back + Vector3.left, Vector3.back + Vector3.right
            };
            foreach (var dir in directions)
            {
                Vector3 checkPos = node.position + dir * nodeSpacing;
                Vector3 checkPoint = new Vector3(checkPos.x, waterHeight, checkPos.z);
                if (!Physics.CheckSphere(checkPoint, nodeSpacing / 2f, obstacleMask))
                    neighbours.Add(new Node(checkPoint, true));
            }
            return neighbours;
        }

        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Debug.Log($"AStarPathfinder.FindPath called: start={startPos}, target={targetPos}");
            Vector3 startGrid = RoundToGrid(startPos);
            Vector3 targetGrid = RoundToGrid(targetPos);

            Node startNode = new Node(startGrid, true);
            Node targetNode = new Node(targetGrid, true);

            startNode.gCost = 0;
            startNode.hCost = Vector3.Distance(startGrid, targetGrid);

            List<Node> openSet = new List<Node> { startNode };
            HashSet<Vector3> closedSet = new HashSet<Vector3>();

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode.position);

                if (Vector3.Distance(currentNode.position, targetNode.position) < nodeSpacing * 0.5f)
                {
                    var path = RetracePath(startNode, currentNode);
                    Debug.Log($"Path found with {path.Count} nodes");
                    return path;
                }

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (closedSet.Contains(neighbour.position)) continue;
                    float newCost = currentNode.gCost + Vector3.Distance(currentNode.position, neighbour.position);
                    if (newCost < neighbour.gCost || !openSet.Exists(n => n.position == neighbour.position))
                    {
                        neighbour.gCost = newCost;
                        neighbour.hCost = Vector3.Distance(neighbour.position, targetNode.position);
                        neighbour.parent = currentNode;
                        if (!openSet.Exists(n => n.position == neighbour.position))
                            openSet.Add(neighbour);
                    }
                }
            }
            Debug.LogWarning("AStarPathfinder: No path found");
            return new List<Vector3>();
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Vector3> path = new List<Vector3>();
            Node current = endNode;
            while (current != startNode)
            {
                path.Add(current.position);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        private Vector3 RoundToGrid(Vector3 pos)
        {
            float x = Mathf.Round(pos.x / nodeSpacing) * nodeSpacing;
            float z = Mathf.Round(pos.z / nodeSpacing) * nodeSpacing;
            return new Vector3(x, waterHeight, z);
        }
    }
}