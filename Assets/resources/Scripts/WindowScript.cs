using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class StatsWindow : EditorWindow
{
    GameObject[] allObjects;
    Component[][] allObjectComponents;
    public List<int> vertexes;
    public List<int> triangles;
    bool setup = false;
    bool[][] menuOptions;
    Vector2 scrollPos = Vector2.zero;
    ObjectWindow objectWindow;
    float totalVertexCount, totalTriangleCount, totalTextureSizes;

    [MenuItem("Window/GameObject Statistics")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StatsWindow));
    }

    void OnGUI()
    {
        if (setup == false)
        {
            SetupArray();
            setup = true;
        }

        GUILayout.Label("List of GameObjects", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height/2));
        EditorGUI.indentLevel = 1;
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (menuOptions != null)
            {
                menuOptions[i][0] = EditorGUILayout.Foldout(menuOptions[i][0], allObjects[i].name);
                if (menuOptions[i][0] && allObjectComponents.Length > 0)
                {
                    if (allObjects[i].GetComponent<MeshFilter>())
                    {
                        EditorGUI.indentLevel = 2;
                        EditorGUILayout.LabelField("MeshFilter", EditorStyles.boldLabel);
                        MeshFilter tempMesh = allObjects[i].GetComponent<MeshFilter>();
                        float vertexPercentage = (tempMesh.sharedMesh.vertexCount / totalVertexCount) * 100;
                        int vertexPercentageInt = (int)vertexPercentage;
                        EditorGUI.indentLevel = 3;
                        EditorGUILayout.LabelField("");
                        EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), vertexPercentage / 100, "Vertex Count (" + vertexPercentageInt + "%)");
                        float trianglePercentage = ((tempMesh.sharedMesh.triangles.Length / 3) / totalTriangleCount) * 100;
                        int trianglePercentageInt = (int)trianglePercentage;
                        EditorGUILayout.LabelField("");
                        EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), trianglePercentage / 100, "Triangle Count (" + trianglePercentageInt + "%)");
                    }
                    if (allObjects[i].GetComponent<MeshRenderer>())
                    {
                        MeshRenderer tempMeshRenderer = allObjects[i].GetComponent<MeshRenderer>();
                        if (tempMeshRenderer.sharedMaterials[0].mainTexture)
                        {
                            EditorGUI.indentLevel = 2;
                            EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
                            for (int j = 0; j < tempMeshRenderer.sharedMaterials.Length; j++)
                            {
                                EditorGUI.indentLevel = 3;
                                EditorGUILayout.LabelField("Texture (" + tempMeshRenderer.sharedMaterials[j].mainTexture.name + ")");
                                EditorGUILayout.LabelField("");
                                float textureSizePercentage = (Profiler.GetRuntimeMemorySize(tempMeshRenderer.sharedMaterials[j].mainTexture) / totalTextureSizes) * 100;
                                int textureSizePercentageInt = (int) textureSizePercentage;
                                EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), textureSizePercentage / 100, "Texture Size (" + textureSizePercentageInt + "%)");
                            }
                        }
                    }
                    EditorGUI.indentLevel = 1;
                    if (GUILayout.Button("Analyse"))
                    {
                        if (objectWindow != ScriptableObject.CreateInstance<ObjectWindow>())
                        {
                            objectWindow = ScriptableObject.CreateInstance<ObjectWindow>();
                        }
                        objectWindow.SetupObject(allObjects[i]);
                        objectWindow.Show();
                    }
                }
            }
        }

        GUILayout.EndScrollView();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Total Memory Used: ", System.GC.GetTotalMemory (true) / 1024 + " kB", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Total Memory Available: ", SystemInfo.systemMemorySize * 1024 + " kB", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Total Memory Left: ", (SystemInfo.systemMemorySize * 1024) - (System.GC.GetTotalMemory(true) / 1024) + " kB", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Screen Resolution: ", UnityStats.screenRes, EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Render Time: ", UnityStats.renderTime.ToString() + " secs", EditorStyles.boldLabel);
            float tempFPS = 1 / UnityStats.frameTime;
            EditorGUILayout.LabelField("FPS: ", tempFPS.ToString(), EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total Draw Calls: ", UnityStats.drawCalls.ToString(), EditorStyles.boldLabel);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Update"))
        {
            SetupArray();
            Debug.ClearDeveloperConsole();
        }
        if (GUILayout.Button("Close"))
        {
            Close();
        }
    }

    void SetupArray()
    {
        totalVertexCount = 0;
        totalTriangleCount = 0;
        totalTextureSizes = 0;
        allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        allObjectComponents = new Component[allObjects.Length][];

        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjectComponents[i] = allObjects[i].GetComponents(typeof(Component));
            if (allObjects[i].GetComponent<MeshFilter>())
            {
                MeshFilter tempMesh = allObjects[i].GetComponent<MeshFilter>();
                totalVertexCount += tempMesh.sharedMesh.vertexCount;
                totalTriangleCount += tempMesh.sharedMesh.triangles.Length / 3;
            }
            if (allObjects[i].GetComponent<MeshRenderer>())
            {
                MeshRenderer tempMeshRenderer = allObjects[i].GetComponent<MeshRenderer>();
                if (tempMeshRenderer.sharedMaterials[0].mainTexture)
                {
                    totalTextureSizes += Profiler.GetRuntimeMemorySize(tempMeshRenderer.sharedMaterials[0].mainTexture);
                }
            }
        }

        menuOptions = new bool[allObjects.Length][];

        for (int i = 0; i < allObjects.Length; i++)
        {
            for (int j = 0; j < allObjectComponents[i].Length; j++)
            {
                menuOptions[i] = new bool[allObjectComponents[i].Length];
                menuOptions[i][j] = false;
            }
        }        
        objectWindow = ScriptableObject.CreateInstance<ObjectWindow>();
    }
}