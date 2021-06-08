using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FixMaterialWindow : EditorWindow
{

    [MenuItem("Yo/Material Fixer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        FixMaterialWindow window = (FixMaterialWindow)EditorWindow.GetWindow(typeof(FixMaterialWindow));
        window.Show();
    }


    private void OnGUI()
    {

        if (GUILayout.Button("Fix"))
        {
            FixMaterials();
        }
    }

    private void FixMaterials()
    {
        string[] materialsGUID = AssetDatabase.FindAssets("t:material", null);
        string newShaderName = "HDRP/Lit";
        Shader shader = Shader.Find(newShaderName);

        if (shader == null)
        {
            Debug.LogError($"Could not find {newShaderName}");
            return;
        }

        foreach (var guid in materialsGUID)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null) continue;

            //Debug.Log(m.shader.name);

            if (m.shader.name == "Hidden/InternalErrorShader")
            {
                m.shader = shader;
            }

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
