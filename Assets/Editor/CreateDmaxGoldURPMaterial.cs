// Put this file in: Assets/Editor/CreateDmaxGoldURPMaterial.cs
// Then in Unity: Tools -> Materials -> Create D-Max Gold (URP)
// Creates a less-metallic "automotive gold" URP/Lit material (more like car paint, less like solid metal).

using UnityEditor;
using UnityEngine;

public static class CreateDmaxGoldURPMaterial
{
    [MenuItem("Tools/Materials/Create D-Max Gold (URP)")]
    public static void Create()
    {
        // Approx "champagne / topaz gold" base. Tweak in Inspector if you want it warmer/cooler.
        Color baseColor = new Color(0.72f, 0.55f, 0.17f, 1f); // ~ #B88B2B

        // Car paint feel: not full metal, fairly smooth.
        float metallic   = 0.32f; // try 0.20–0.45
        float smoothness = 0.68f; // try 0.55–0.80

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("URP/Lit shader not found. Are you sure this project uses URP?");
            return;
        }

        var mat = new Material(shader);
        mat.name = "D-Max Gold (URP)";

        // Base workflow
        if (mat.HasProperty("_BaseColor"))  mat.SetColor("_BaseColor", baseColor);
        if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic", metallic);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

        // Optional: clear coat for that "automotive clear layer" look (if your URP version supports it)
        // Keep it subtle; too much can look plasticky.
        if (mat.HasProperty("_ClearCoatMask"))
        {
            mat.EnableKeyword("_CLEARCOAT");
            mat.SetFloat("_ClearCoatMask", 0.35f);       // 0–1 (coat amount)
        }
        if (mat.HasProperty("_ClearCoatSmoothness"))
        {
            mat.SetFloat("_ClearCoatSmoothness", 0.85f); // coat gloss
        }

        // Save asset
        string path = "Assets/D-Max Gold (URP).mat";
        AssetDatabase.CreateAsset(mat, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created material: " + path);
        Selection.activeObject = mat;
    }
}
