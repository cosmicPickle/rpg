using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(NavmeshBuilder))]
public class NavmeshBuilderEditor : Editor {

    private string gameDataProjectFilePath = "/StreamingAssets/Navmesh/";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavmeshBuilder myScript = (NavmeshBuilder)target;
        if (GUILayout.Button("Build Navmesh"))
        {
            myScript.BuildNavmesh();

            Navmesh navmesh = myScript.GetSerializable();
            string navmeshFileName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name 
                                     + "_gs_" + myScript.grid.cellSize.x 
                                     + "_" + myScript.grid.cellSize.y
                                     + "_s_" + myScript.navmeshSize.x 
                                     + "_" + myScript.navmeshSize.y 
                                     + ".json";
            
            if(navmesh.paths.Count > 0)
            {
                string navmeshJson = JsonUtility.ToJson(navmesh);
                string path = Application.dataPath + gameDataProjectFilePath + navmeshFileName;

                File.WriteAllText(path, navmeshJson);
            }
        }
    }
}
