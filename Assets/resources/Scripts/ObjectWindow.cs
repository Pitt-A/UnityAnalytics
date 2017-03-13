using UnityEngine;
using UnityEditor;
using System.Collections;

public class ObjectWindow : EditorWindow {
    public bool showWindow = true;
    GameObject currentObject;
    bool setup = false;
    Vector2 scrollPos = Vector2.zero;
    int totalMemoryUsed;
    //GUIStyle red = new GUIStyle();
    //GUIStyle green = new GUIStyle();

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ObjectWindow));
    }

    // Update is called once per frame
    void OnGUI ()
    {
        if (setup)
        {
            EditorWindow.GetWindow(typeof(ObjectWindow));
            //red = new GUIStyle();
            //red.normal.textColor = Color.red;
            //green = new GUIStyle();
            //green.normal.textColor = Color.green;
            setup = false;
        }

        if (showWindow)
        {
            if (currentObject != null)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(Screen.width), GUILayout.Height(position.height));
                totalMemoryUsed = 0;
                GUILayout.Label(currentObject.name, EditorStyles.boldLabel);
                if (currentObject.GetComponent<MeshFilter>())
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.LabelField("MeshFilter", EditorStyles.boldLabel);
                    MeshFilter tempMesh = currentObject.GetComponent<MeshFilter>();
                    int tempVertexCount = tempMesh.sharedMesh.vertexCount;
                    EditorGUI.indentLevel = 3;
                    EditorGUILayout.LabelField("Vertex count: ", tempVertexCount.ToString());
                    int tempTriangleCount = tempMesh.sharedMesh.triangles.Length / 3;
                    EditorGUILayout.LabelField("Triangle count: ", tempTriangleCount.ToString());
                    EditorGUILayout.LabelField("Memory Used: ", Profiler.GetRuntimeMemorySize(tempMesh).ToString() + " bytes");
                    totalMemoryUsed += Profiler.GetRuntimeMemorySize(tempMesh);
                }
                if (currentObject.GetComponent<Collider>())
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.LabelField("Collider", EditorStyles.boldLabel);
                    Collider tempBoxCollider = currentObject.GetComponent<Collider>();
                    EditorGUI.indentLevel = 3;
                    EditorGUILayout.LabelField("Type: ", tempBoxCollider.name.ToString());
                    EditorGUILayout.LabelField("Center: ", tempBoxCollider.bounds.center.ToString());
                    EditorGUILayout.LabelField("Size: ", tempBoxCollider.bounds.size.ToString());
                    EditorGUILayout.LabelField("Memory Used: ", Profiler.GetRuntimeMemorySize(tempBoxCollider).ToString() + " bytes");
                    totalMemoryUsed += Profiler.GetRuntimeMemorySize(tempBoxCollider);
                }
                if (currentObject.GetComponent<MeshRenderer>())
                {
                    EditorGUI.indentLevel = 2;
                    MeshRenderer tempMeshRenderer = currentObject.GetComponent<MeshRenderer>();
                    if (tempMeshRenderer.sharedMaterials[0].mainTexture)
                    {
                        EditorGUILayout.LabelField("Texture (" + tempMeshRenderer.sharedMaterials[0].mainTexture.name + ")", EditorStyles.boldLabel);
                        Vector2 tempDimensions = new Vector2(tempMeshRenderer.sharedMaterials[0].mainTexture.width, tempMeshRenderer.sharedMaterials[0].mainTexture.height); //might need fixing
                        EditorGUI.indentLevel = 3;
                        EditorGUILayout.LabelField("Dimensions: ", tempDimensions.ToString());
                        EditorGUILayout.LabelField("Memory Used: ", Profiler.GetRuntimeMemorySize(tempMeshRenderer).ToString() + " bytes");
                        totalMemoryUsed += Profiler.GetRuntimeMemorySize(tempMeshRenderer);
                    } 
                }
                if (currentObject.GetComponent<Camera>())
                {
                    EditorGUI.indentLevel = 2;
                    Camera tempCamera = currentObject.GetComponent<Camera>();
                    EditorGUILayout.LabelField("Camera", EditorStyles.boldLabel);
                    EditorGUI.indentLevel = 3;
                    if (!tempCamera.useOcclusionCulling)
                    {
                        EditorGUILayout.LabelField("Occlusion culling is disabled on this camera.");
                        EditorGUILayout.LabelField("Enable it for improved performance.");
                        EditorGUILayout.Space();
                    }
                    EditorGUILayout.LabelField("Memory Used: ", Profiler.GetRuntimeMemorySize(tempCamera).ToString() + " bytes");
                    totalMemoryUsed += Profiler.GetRuntimeMemorySize(tempCamera);
                }

                OtherComponents();

                EditorGUI.indentLevel = 0;
                EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
                totalMemoryUsed += Profiler.GetRuntimeMemorySize(currentObject);
                EditorGUILayout.LabelField("Total Memory Used: ", totalMemoryUsed.ToString() + " bytes", EditorStyles.boldLabel);
            }
            if (GUILayout.Button("Close"))
            {
                Close();
                currentObject = null;
                setup = true;
                DestroyImmediate(this);
            }
            GUILayout.EndScrollView();
        }
        this.Repaint();
    }

    void OtherComponents()
    {
        Component[] components = currentObject.GetComponents(typeof(Component));

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] is MeshFilter || components[i] is Collider || components[i] is MeshRenderer || components[i] is Camera)
            {
                //do nothing
            }
            else
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.LabelField(components[i].GetType().Name, EditorStyles.boldLabel);
                EditorGUI.indentLevel = 3;
                EditorGUILayout.LabelField("Memory Used: ", Profiler.GetRuntimeMemorySize(components[i]).ToString() + " bytes");
                totalMemoryUsed += Profiler.GetRuntimeMemorySize(components[i]);
            }
        }
    }

    public void SetupObject(GameObject tempObject)
    {
        currentObject = tempObject;
        Debug.Log("Object received!");
    }

    public void OnInspectorGUI()
    {
        this.Repaint();
    }
}
