using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[RequireComponent(typeof(Grid))]
public class NavmeshLoader : MonoBehaviour {

    public Vector2 navmeshSize;
    public static Navmesh Navmesh;

    public static Grid Grid;

    // Use this for initialization
    void Awake () {

        if(Grid)
        {
            Destroy(gameObject);
        }

        Grid = GetComponent<Grid>();
        string navmeshFileName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                                     + "_gs_" + Grid.cellSize.x
                                     + "_" + Grid.cellSize.y
                                     + "_s_" + navmeshSize.x
                                     + "_" + navmeshSize.y
                                     + ".json";

        LoadNavmesh(navmeshFileName);
    }
	
    void LoadNavmesh(string filename)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Navmesh");
        filePath = Path.Combine(filePath, filename);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string navmeshAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            Navmesh = JsonUtility.FromJson<Navmesh>(navmeshAsJson);
            Navmesh.nodeMap = new Dictionary<Vector2, Navmesh.SerializableNavmeshNode>();

            for (int i = 0; i < Navmesh.paths.Count; i++)
            {
                Navmesh.nodeMap.Add(Navmesh.paths[i].position, Navmesh.paths[i]);
            }
        }
        else
        {
           throw new Exception("Cannot load Navmesh!");
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
