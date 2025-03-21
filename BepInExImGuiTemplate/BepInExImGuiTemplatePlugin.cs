using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;

namespace BepInExImGuiTemplate;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]

public partial class BepInExImGuiTemplatePlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);
    public static new ManualLogSource Log;
    
    public override void Load()
    {
        Log = base.Log;
        
        Harmony.PatchAll();
        
        // Add the Menu component to the game object
        this.AddComponent<Menu>();
    }
}

public class Menu : MonoBehaviour
{
    // Create a rect for the window
    public Rect windowRect = new(10f, 10f, 300f, 300f);
    public bool showWindow = true;
    
    // Override OnGUI to draw the window
    public void OnGUI()
    {
        if (!showWindow) return;
        // Set the ID to something unique because it can cause incompatibility with other mods
        // if two mods (plugins) are trying to use the same ID
        windowRect = GUILayout.Window(123, windowRect, (GUI.WindowFunction)WindowFunction, "Window Title");
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Toggle the window with F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showWindow = !showWindow;
        }
    }
    
    // Window function
    public void WindowFunction(int id)
    {
        GUILayout.Label($"Window ID: {id}");
        if (GUILayout.Button("Example Button"))
        {
            BepInExImGuiTemplatePlugin.Log.LogInfo($"Example Button was pressed!");
        }
        GUI.DragWindow();
    }
}
