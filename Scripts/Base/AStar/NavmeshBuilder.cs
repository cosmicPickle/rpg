using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class NavmeshBuilder : MonoBehaviour {

    public bool hideNavmesh = false;
    public bool hideNodes = false;
    public bool drawInvalidConnections = false;
    public bool drawInvalidNodes = false;

    public float agentSize = 1f;
    public int minConnectivity = 4;

    public Vector2 navmeshSize;
    public LayerMask collisionMask;

    Dictionary<Vector2, Navmesh.NavmeshNode> nodesDictionary;
    List<Navmesh.NavmeshNode> nodes;

    Navmesh.NavmeshBoundsWorld navmeshBounds = new Navmesh.NavmeshBoundsWorld();

    [HideInInspector]
    public Grid grid;
    bool showingNavmesh = false;

    void Start()
    {
        grid = GetComponent<Grid>();
    }

    public Navmesh GetSerializable()
    {
        Dictionary<Vector2, Navmesh.SerializableNavmeshNode> navmeshDictionary = new Dictionary<Vector2, Navmesh.SerializableNavmeshNode>();

        Navmesh navmesh = new Navmesh();
        navmesh.paths = new List<Navmesh.SerializableNavmeshNode>();

        for (int i = 0; i < nodes.Count; i++)
        {
            //Adding the serializable info
            Navmesh.SerializableNavmeshNode sNode = new Navmesh.SerializableNavmeshNode(navmesh.paths.Count, nodes[i].position, nodes[i].isValid);
            navmeshDictionary.Add(nodes[i].position, sNode);
            navmesh.paths.Add(sNode);
        }


        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes[i].neighbours.Length; j++)
            {
                Vector2 nPos = nodes[i].neighbours[j].position;
                if (!navmeshDictionary.ContainsKey(nPos))
                    continue;

                navmesh.paths[i].AddNeighbours(navmeshDictionary[nPos].id, nodes[i].neighbours[j].isValidConnection);
            }

            //print(navmesh[i].ToString());
        }

        return navmesh;
    }

	public void BuildNavmesh()
    {
        grid = GetComponent<Grid>();

        nodesDictionary = new Dictionary<Vector2, Navmesh.NavmeshNode>();
        nodes = new List<Navmesh.NavmeshNode>();

        if (grid.cellSize.x != grid.cellSize.y)
        {
            throw new Exception("Navmesh works only with symetrical grids.");
        }

        CalculateNavmeshBounds();
        BuildNodes();
        BuildConnections();

        showingNavmesh = true;
    }

    void OnDrawGizmos()
    {
        if (hideNavmesh)
            return;

        List<Navmesh.NavmeshConnection> connections = new List<Navmesh.NavmeshConnection>();
        if (showingNavmesh && nodes != null && nodes.Count > 0)
        {
            for(int i = 0; i < nodes.Count; i ++)
            {
                DrawNode(nodes[i], ref connections);
            }
        }
    }

    void DrawNode(Navmesh.NavmeshNode node, ref List<Navmesh.NavmeshConnection> connections)
    {

        if(!drawInvalidNodes && ! node.isValid)
        {
            return;
        }

        if (!hideNodes)
        {
            Gizmos.color = (node.isValid) ? Color.gray : Color.red;
            Gizmos.DrawSphere(node.position, 0.1f);
        }

        for (int i = 0; i < node.neighbours.Length; i++)
        {
           if(node.neighbours[i].node != null)
           {
                DrawConnection(node, node.neighbours[i].node, ref connections, !node.neighbours[i].isValidConnection);
           }
        }
    }

    void DrawConnection(Navmesh.NavmeshNode n1, Navmesh.NavmeshNode n2, ref List<Navmesh.NavmeshConnection> connections, bool forceInvalidConnection = false)
    {
        bool connectionValid = !forceInvalidConnection && n1.isValid && n2.isValid;

        if(!connectionValid && !drawInvalidConnections)
        {
            return;
        }

        Navmesh.NavmeshConnection conn = new Navmesh.NavmeshConnection(n1, n2);
        if(connections.Contains(conn))
        {
            return;
        }

        connections.Add(conn);
        Gizmos.color = connectionValid ? Color.green : Color.red;
        Gizmos.DrawLine(n1.position, n2.position);
    }

    void BuildNodes()
    {
        for (int x = -(int)navmeshSize.x / 2 + 1; x < (int)navmeshSize.x / 2; x++)
        {
            for (int y = -(int)navmeshSize.y / 2 + 1; y < (int)navmeshSize.y / 2; y++)
            {
                Vector2[] positions = new Vector2[5];

                //Center
                positions[0] = grid.CellToWorld(new Vector3Int(x, y, 0));

                //TopLeft
                positions[1] = new Vector2(
                    positions[0].x - grid.cellSize.x / 2,
                    positions[0].y + grid.cellSize.y / 2
                );

                //TopRight
                positions[2] = new Vector2(
                    positions[0].x + grid.cellSize.x / 2,
                    positions[0].y + grid.cellSize.y / 2
                );

                //BottomLeft
                positions[3] = new Vector2(
                    positions[0].x - grid.cellSize.x / 2,
                    positions[0].y - grid.cellSize.y / 2
                );

                //BottomRight
                positions[4] = new Vector2(
                    positions[0].x + grid.cellSize.x / 2,
                    positions[0].y - grid.cellSize.y / 2
                );

                for (int i = 0; i < positions.Length; i++)
                {
                    

                    Navmesh.NavmeshNode node = new Navmesh.NavmeshNode();
                    node.position = positions[i];

                    if (nodesDictionary.ContainsKey(node.position))
                    {
                        continue;
                    }
                    node.CalculateNeighbouringNodes(grid.cellSize);

                    nodesDictionary.Add(node.position, node);
                    nodes.Add(node);
                    
                }
            }
        }
    }

    void BuildConnections()
    {
        for (int i = 0; i < nodes.Count; i++)
        {

            for (int j = 0; j < nodes[i].neighbours.Length; j++)
            {
                if (!nodesDictionary.ContainsKey(nodes[i].neighbours[j].position))
                {
                    continue;
                }

                Navmesh.NavmeshNode neighbourNode = nodesDictionary[nodes[i].neighbours[j].position];
                nodes[i].neighbours[j].node = neighbourNode;

                Vector2 nPos = nodes[i].neighbours[j].position;

                if (neighbourNode.isValid == false)
                {
                    //Already checked this node and it is invalid
                    continue;
                }

                if (!navmeshBounds.WithinBounds(nPos))
                {
                    neighbourNode.isValid = false;
                    continue;
                }


                ///TODO: This maybe better check
                Collider2D hit = Physics2D.OverlapCircle(nPos, agentSize / 2, collisionMask);

                if (hit)
                {
                    nodes[i].neighbours[j].node.isValid = false;
                }
                else
                {
                    nodes[i].neighbours[j].isValidConnection = true;
                    nodes[i].validNeighbours++;
                }

                //Vector2 heading = nPos - nodes[i].position;
                //RaycastHit2D hit = Physics2D.Raycast(nodes[i].position, heading.normalized, heading.magnitude, collisionMask);

                //if (hit)
                //{
                //    nodes[i].neighbours[j].isValidConnection = false;
                //}
                //else
                //{
                //    Vector2 boxSize = new Vector2(agentSize / 2, agentSize / 2);
                //    Vector2 center1 = (nodes[i].position + heading / 2) + Vector2.Perpendicular(heading).normalized * agentSize / 2;
                //    Collider2D overlap1 = Physics2D.OverlapBox(center1, boxSize, Vector2.Angle(Vector2.one, heading), collisionMask);

                //    Vector2 center2 = (nodes[i].position + heading / 2) + Vector2.Perpendicular(heading).normalized * agentSize / 2;
                //    Collider2D overlap2 = Physics2D.OverlapBox(center2, boxSize, Vector2.Angle(Vector2.one, heading), collisionMask);

                //    if (overlap1 && overlap2)
                //    {
                //        nodes[i].neighbours[j].isValidConnection = false;
                //    }
                //    else
                //    {
                //        nodes[i].neighbours[j].isValidConnection = true;
                //        nodes[i].validNeighbours++;
                //    }
                //}

            }

            if(nodes[i].validNeighbours <= minConnectivity)
            {
                nodes[i].isValid = false;
            }
        }

    }

    void CalculateNavmeshBounds()
    {
        Vector3 bottomLeft = grid.CellToWorld(new Vector3Int(
            Mathf.FloorToInt(-navmeshSize.x / 2),
            Mathf.FloorToInt(-navmeshSize.y / 2),
            0
        ));
        Vector3 topRight = grid.CellToWorld(new Vector3Int(
            Mathf.FloorToInt(navmeshSize.x / 2),
            Mathf.FloorToInt(navmeshSize.y / 2),
            0
        ));

        navmeshBounds.top = topRight.y;
        navmeshBounds.bottom = bottomLeft.y;
        navmeshBounds.left = bottomLeft.x;
        navmeshBounds.right = topRight.x;
    } 
}
