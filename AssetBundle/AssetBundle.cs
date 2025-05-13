using System.IO;
using BepInEx;
using UnityEngine;

namespace BepInExTemplates.AssetBundle;

public class AssetBundleLoader : MonoBehaviour
{
    private UnityEngine.AssetBundle _assetBundle;

    private void Awake()
    {
        // Load Asset Bundle
        _assetBundle = LoadAssetBundle("modname_assets");

        if (!_assetBundle) return;
        // Load specific asset from Asset Bundle (e.g., texture)
        var texture = (Texture2D)_assetBundle.LoadAsset("myTexture");
        if (!texture)
        {
            Debug.LogError("Failed to load texture 'myTexture' from asset bundle");
            return;
        }

        // Use the texture in your mod
        var material = new Material(Shader.Find("Standard"));
        material.mainTexture = texture;

        // Apply it to a game object
        var myObject = new GameObject("ModdedObject");
        myObject.AddComponent<Renderer>().material = material;
    }

    private static UnityEngine.AssetBundle LoadAssetBundle(string bundleName)
    {
        // Try loading the Asset Bundle from the mod's directory or streaming assets
        var path = Path.Combine(Paths.PluginPath, bundleName);
        if (File.Exists(path))
        {
            return UnityEngine.AssetBundle.LoadFromFile(path);
        }
        Debug.LogError($"Asset Bundle '{bundleName}' not found at path: {path}");
        return null;
    }

    private void OnDestroy()
    {
        // Clean up and unload the Asset Bundle
        _assetBundle?.Unload(true);
    }
}
