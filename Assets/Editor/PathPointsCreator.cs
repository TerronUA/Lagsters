using UnityEngine;
using System.Collections;
using UnityEditor;

public class PathPointsCreator
{
    [MenuItem("Assets/Create/Lagsters/Path")]
    public static PathPoints Create()
    {
        PathPoints asset = ScriptableObject.CreateInstance<PathPoints>();

        AssetDatabase.CreateAsset(asset, "Assets/PathPoints.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
