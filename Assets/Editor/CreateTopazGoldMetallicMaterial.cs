// Put this file in: Assets/Editor/CreateTopazGoldMetallicMaterial.cs
// Then in Unity: Tools -> Materials -> Create Topaz Gold Metallic
// Works in both Built-in (Standard) and URP (Lit) automatically.

using UnityEditor;
using UnityEngine;

public static class CreateTopazGoldMetallicMaterial
{
    [MenuItem("Tools/Materials/Create Topaz Gold Metallic")]
    public static void Create()
    {
        // Topaz-gold-ish color
        Color topazGold = new Color(0.82f, 0.63f, 0.22f, 1f);
        float metallic = 1.0f;
        float smoothness = 0.68f;

        // Prefer URP Lit if present, otherwise fallback to Standard
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        if (shader == null)
        {
            Debug.LogError("Could not find URP/Lit or Standard shader in this project.");
            return;
        }

        var mat = new Material(shader);
        mat.name = "Topaz Gold Metallic";

        // URP Lit properties
        if (shader.name == "Universal Render Pipeline/Lit")
        {
            mat.SetColor("_BaseColor", topazGold);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
        }
        else // Standard
        {
            mat.SetColor("_Color", topazGold);
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", smoothness);
        }

        // Save asset
        string path = "Assets/Topaz Gold Metallic.mat";
        AssetDatabase.CreateAsset(mat, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created material: " + path);
        Selection.activeObject = mat;
    }
}
