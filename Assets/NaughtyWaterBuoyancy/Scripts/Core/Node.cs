using UnityEngine;

namespace Pathfinding
{
    public class Node
    {
        public Vector3 position;
        public bool walkable;
        public float gCost;
        public float hCost;
        public Node parent;

        public float fCost => gCost + hCost;

        public Node(Vector3 position, bool walkable)
        {
            this.position = position;
            this.walkable = walkable;
            this.gCost = float.MaxValue;
            this.hCost = 0f;
            this.parent = null;
        }
    }
}