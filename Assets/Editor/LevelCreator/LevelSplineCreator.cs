using UnityEngine;
using UnityEditor;

public class LevelSplineCreator
{
    [MenuItem("Assets/Create/Lagsters/Level Spline")]
    public static LevelSpline Create(string FilePath = "Assets/PathPoints.asset")
    {
        LevelSpline asset = ScriptableObject.CreateInstance<LevelSpline>();

        AssetDatabase.CreateAsset(asset, FilePath);
        AssetDatabase.SaveAssets();
        return asset;
    }
}
