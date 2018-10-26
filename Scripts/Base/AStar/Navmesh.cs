using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Navmesh
{
    [SerializeField]
    public List<SerializableNavmeshNode> paths;
    public Dictionary<Vector2, SerializableNavmeshNode> nodeMap;

    public struct NavmeshBoundsWorld
    {
        public float top, bottom;
        public float left, right;

        public bool WithinBounds(Vector2 point)
        {
            return point.x > left &&
                   point.x < right &&
                   point.y > bottom &&
                   point.y < top;
        }
    }

    public class NavmeshNode
    {

        public Vector2 position;
        public bool isValid = true;
        public Neighbour[] neighbours = new Neighbour[8];
        public int validNeighbours;

        public void CalculateNeighbouringNodes(Vector2 cellSize)
        {
            //TopLeft
            neighbours[0] = new Neighbour(new Vector2(
                position.x - cellSize.x / 2,
                position.y + cellSize.y / 2
            ));

            //Top
            neighbours[2] = new Neighbour(new Vector2(
                position.x,
                position.y + cellSize.y
            ));


            //TopRight
            neighbours[3] = new Neighbour(new Vector2(
                position.x + cellSize.x / 2,
                position.y + cellSize.y / 2
            ));

            //Right
            neighbours[4] = new Neighbour(new Vector2(
                position.x + cellSize.x,
                position.y
            ));

            //BottomRight
            neighbours[5] = new Neighbour(new Vector2(
                position.x + cellSize.x / 2,
                position.y - cellSize.y / 2
            ));

            //Bottom
            neighbours[6] = new Neighbour(new Vector2(
                position.x,
                position.y - cellSize.y
            ));

            //BottomLeft
            neighbours[7] = new Neighbour(new Vector2(
                position.x - cellSize.x / 2,
                position.y - cellSize.y / 2
            ));

            //Left
            neighbours[1] = new Neighbour(new Vector2(
                position.x - cellSize.x,
                position.y
            ));
        }

        public bool Equals(NavmeshNode other)
        {
            return position == other.position;
        }

        
    }

    public struct Neighbour
    {
        public Vector2 position;
        public bool isValidConnection;
        public NavmeshNode node;

        public Neighbour(Vector2 p)
        {
            position = p;
            isValidConnection = false;
            node = null;
        }
    }


    [System.Serializable]
    public class SerializableNavmeshNode
    {
        [SerializeField]
        public int id;
        [SerializeField]
        public Vector2 position;
        [SerializeField]
        public bool isValid;
        [SerializeField]
        public List<SerializableNeighbour> neighbours;

        public SerializableNavmeshNode(int i, Vector2 p, bool v)
        {
            id = i;
            position = p;
            isValid = v;
            neighbours = new List<SerializableNeighbour>();
        }

        public void AddNeighbours(int i, bool isValid)
        {
            neighbours.Add(new SerializableNeighbour(i, isValid));
        }

        public bool Equals(SerializableNavmeshNode other)
        {
            return position == other.position;
        }

        public override string ToString()
        {
            string str = "\nNavmesh \n ====== \n id: " + id + "\n position: " + position + "\n isValid: \n" + isValid + " Neighbours(" + neighbours.Count + "): \n ========== \n";
            for (int i = 0; i < neighbours.Count; i++)
            {
                str += neighbours[i].ToString();
            }
            return str;
        }
    }

    [System.Serializable]
    public class SerializableNeighbour
    {
        [SerializeField]
        public int index;
        [SerializeField]
        public bool isValidConnection;

        public SerializableNeighbour(int i, bool isValid)
        {
            index = i;
            isValidConnection = isValid;
        }

        public override string ToString()
        {
            return "Neightbour: " + index + "\n isValidConnection: " + isValidConnection + "  \n";
        }
    }

    public class NavmeshConnection
    {

        public NavmeshNode start;
        public NavmeshNode end;

        public NavmeshConnection(NavmeshNode a, NavmeshNode b)
        {
            if (a == b)
            {
                throw new System.Exception("NavmeshConnection cannot be made with equal points");
            }

            start = a;
            end = b;
        }

        public bool Equals(NavmeshConnection other)
        {
            return (
                    start == other.start &&
                    end == other.end
                   ) || (
                    start == other.end &&
                    end == other.start
                   );
        }
    }
}