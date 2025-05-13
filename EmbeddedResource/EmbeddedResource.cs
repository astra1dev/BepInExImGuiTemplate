using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BepInExTemplates.EmbeddedResource;

public class EmbeddedResource
{
    private static readonly Dictionary<string, Texture2D> TextureCache = new();

    /// <summary>
    /// Load a texture from the embedded resources.
    /// </summary>
    /// <param name="path">The path to the texture in the embedded resources.</param>
    /// <returns>The loaded texture.</returns>
    private static Texture2D LoadTexture(string path)
    {
        if (TextureCache.TryGetValue(path, out var cachedTexture))
            return cachedTexture;

        // Get the assembly of the current class
        var assembly = Assembly.GetExecutingAssembly();

        // Get the resource stream for the texture
        var stream = assembly.GetManifestResourceStream(path);

        if (stream == null)
        {
            Debug.LogError($"Failed to load texture from path: {path}");
            return null;
        }

        // Create a new Texture2D
        var texture = new Texture2D(2, 2);
        texture.LoadImage(ReadAllBytes(stream));

        TextureCache[path] = texture;
        return texture;
    }

    /// <summary>
    /// Read all bytes from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The byte array containing the data from the stream.</returns>
    private static byte[] ReadAllBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Unload a texture.
    /// </summary>
    /// <param name="texture">The texture to unload.</param>
    private static void UnloadTexture(Texture2D texture)
    {
        if (texture)
        {
            Object.Destroy(texture);
        }
    }

    /// <summary>
    /// Unload a collection of textures.
    /// </summary>
    /// <param name="textures">The collection of textures to unload.</param>
    public static void UnloadTextures(IEnumerable<Texture2D> textures)
    {
        foreach (var texture in textures)
        {
            UnloadTexture(texture);
        }
    }

    /// <summary>
    /// Load a sprite from the embedded resources.
    /// </summary>
    /// <see href="https://docs.unity3d.com/ScriptReference/Sprite.Create.html">Unity Sprite.Create</see>
    /// <param name="path">The path to the sprite in the embedded resources.</param>
    /// <returns>The loaded sprite.</returns>
    public static Sprite LoadSprite(string path)
    {
        var texture = LoadTexture(path);
        if (!texture)
        {
            Debug.LogError($"Failed to load sprite from path: {path}");
            return null;
        }

        // Create a new sprite from the texture
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    /// <summary>
    /// List all embedded resources in the assembly.
    /// </summary>
    /// <returns>A list of resource names.</returns>
    public static List<string> ListResources()
    {
        // Get the assembly of the current class
        var assembly = Assembly.GetExecutingAssembly();

        // Get all embedded resources
        var resources = assembly.GetManifestResourceNames();
        foreach (var resource in resources)
        {
            Debug.Log($"Found resource: {resource}");
        }
        return [..resources];
    }

    // You don't want to load / cache all sprites / textures at the start of the game
    // The main reason why is performance, if you have a lot of textures / sprites, it can take some time to load them
    // Instead, load them when needed ("on-demand" loading)
}
