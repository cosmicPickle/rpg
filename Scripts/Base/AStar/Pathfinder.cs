using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(NavmeshBuilder))]
[RequireComponent(typeof(NavmeshLoader))]
public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Agent;
    public int maxPathLength = 100;
    public bool debugDrawNodes = false;
    public bool debugDrawPath = false;

    List<Vector2> debugPath;

    void Awake()
    {
        if(Agent && Agent != this)
        {
            Destroy(gameObject);
        } else
        {
            Agent = this;
        }
    }

    void OnDrawGizmos()
    {
        if(debugPath != null && debugPath.Count > 0)
        {
            List<Vector2> drawnPositions = new List<Vector2>();
            for(int i = 1; i < debugPath.Count; i ++)
            {
                if (debugDrawNodes)
                {
                    Gizmos.color = drawnPositions.Contains(debugPath[i - 1]) ? Color.red : Color.green;
                    Gizmos.DrawSphere(debugPath[i - 1], 0.1f);
                    drawnPositions.Add(debugPath[i - 1]);
                }

                if (debugDrawPath)
                {
                    Debug.DrawLine(debugPath[i - 1], debugPath[i], Color.green);
                }
            }

            if (debugDrawNodes)
            {
                Gizmos.color = drawnPositions.Contains(debugPath[debugPath.Count - 1]) ? Color.red : Color.green;
                Gizmos.DrawSphere(debugPath[debugPath.Count - 1], 0.1f);
            }
        }
    }

    Vector2 GetClosestNode(Vector3 pos)
    {
        Grid grid = NavmeshLoader.Grid;
        Navmesh navmesh = NavmeshLoader.Navmesh;

        Vector2 cellWorld = grid.CellToWorld(grid.WorldToCell(pos));
        Vector2[] possible = new Vector2[5];

        possible[0] = cellWorld;
        possible[1] = cellWorld + Vector2.right * grid.cellSize.x;
        possible[2] = cellWorld + Vector2.up * grid.cellSize.x;
        possible[3] = cellWorld + Vector2.right * grid.cellSize.x + Vector2.up * grid.cellSize.x;
        possible[3] = cellWorld + Vector2.right * grid.cellSize.x / 2 + Vector2.up * grid.cellSize.x / 2;

        float minDistance = -1;
        Vector2 closest = new Vector2();

        for (int i = 0; i < possible.Length; i++)
        {
            if (!navmesh.nodeMap.ContainsKey(possible[i]))
                continue;

            Navmesh.SerializableNavmeshNode node = navmesh.nodeMap[possible[i]];

            if (!node.isValid)
                continue;

            float distance = Vector2.Distance(node.position, (Vector2)pos);
            if (minDistance == -1 || distance < minDistance)
            {
                minDistance = distance;
                closest = navmesh.nodeMap[possible[i]].position;
            }
        }

        return closest;
    }

    public List<Vector2> FindPath(Vector3 start, Vector3 end)
    {
        Navmesh navmesh = NavmeshLoader.Navmesh;

        Node startNode = new Node(null, GetClosestNode(start));
        Node endNode = new Node(null, GetClosestNode(end));

        if (!navmesh.nodeMap.ContainsKey(startNode.position) ||
           !navmesh.nodeMap.ContainsKey(endNode.position))
        {
            Debug.LogWarning("Pathfinder: Start or Finish are invalid positions");
            return null;
        }

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(startNode);

        int length = 0;
        while(openList.Count > 0 && length < maxPathLength)
        {
            length++;

            //Find current node
            Node currentNode = openList[0];
            int currentIndex = 0;

            for(int i = 0; i < openList.Count; i ++)
            {
                if(openList[i].f < currentNode.f)
                {
                    currentNode = openList[i];
                    currentIndex = i;
                }
            }

            Navmesh.SerializableNavmeshNode currentNodeData = navmesh.nodeMap[currentNode.position];
            openList.RemoveAt(currentIndex);
            closedList.Add(currentNode);

            if(currentNode == endNode)
            {
                List<Vector2> path = new List<Vector2>();
                

                Node current = currentNode;
                while(current != null)
                {
                    path.Add(current.position);
                    current = current.parent;
                }

                path.Reverse();
                if(path.Count >= 2 && Vector2.Dot((path[1] - (Vector2)start).normalized, (path[1] - path[0]).normalized) != 1)
                {
                    path[0] = (path[1] + (Vector2)start) / 2;
                }
                debugPath = path;
                return path;
            }

            List<Node> children = new List<Node>();

            foreach(Navmesh.SerializableNeighbour child in currentNodeData.neighbours)
            {
                if(!child.isValidConnection)
                {
                    continue;
                }

                if(!navmesh.paths[child.index].isValid)
                {
                    continue;
                }

                children.Add(new Node(currentNode, navmesh.paths[child.index].position));
            }

            foreach (Node child in children)
            {

                if (closedList.Contains(child))
                    continue;

                child.g = currentNode.g + 1;
                child.h = Vector2.Distance(child.position, endNode.position);
                child.f = child.g + child.h;

                bool foundInOpenList = false;
                foreach(Node openNode in openList)
                {
                    if(child == openNode)
                    {
                        if (child.g >= openNode.g)
                        {
                            openNode.parent = child.parent;
                            openNode.g = child.g;
                            openNode.f = openNode.g + openNode.h;
                        }

                        foundInOpenList = true;
                        break;
                    }
                }

                if (foundInOpenList)
                    continue;

                openList.Add(child);
            }
            
        }

        Debug.LogWarning("Pathfinder: Path not found");
        return null;
    }

    class Node
    {
        public Node parent;
        public Vector2 position;
        public float g = 0, h = 0, f = 0;

        public Node(Node p, Vector2 pos)
        {
            parent = p;
            position = pos;
        }

        public bool Equals(Node other)
        {
            if(other == null)
            {
                return false;
            }

            return position == other.position;
        }

        public static bool operator ==(Node n1, Node n2)
        {
            if (ReferenceEquals(n1, null))
            {
                return ReferenceEquals(n2, null);
            }

            return n1.Equals(n2);
        }

        public static bool operator !=(Node n1, Node n2)
        {
            if (ReferenceEquals(n1, null))
            {
                return !ReferenceEquals(n2, null);
            }

            return !n1.Equals(n2);
        }
    }
    
}
