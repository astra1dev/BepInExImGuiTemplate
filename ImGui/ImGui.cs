using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using HarmonyLib;

namespace BepInExTemplates.ImGui;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
public partial class ImGui : BasePlugin
{
    private Harmony Harmony { get; } = new(Id);
    public new static ManualLogSource Log;
    public static bool IsToggleEnabled { get; set; }

    // Asset stuff
    private static GameObject _lobbyPaintObject;
    private static Sprite _lobbyPaintSprite;

    public override void Load()
    {
        Log = base.Log;

        Harmony.PatchAll();

        // Add the Menu component to the game object
        AddComponent<Menu>();
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public class ExampleSpriteUsage
    {
        public static void Postfix()
        {
            // Example of how to use the sprite
            _lobbyPaintSprite = EmbeddedResource.EmbeddedResource.LoadSprite("BepInExImGuiTemplate.Assets.sandalinus.png");
            var leftBox = GameObject.Find("Leftbox");
            if (!leftBox) return;
            _lobbyPaintObject = Object.Instantiate(leftBox, leftBox.transform.parent.transform);
            _lobbyPaintObject.name = "Lobby Paint";
            _lobbyPaintObject.transform.localPosition = new Vector3(0.042f, -2.59f, -10.5f);
            var renderer = _lobbyPaintObject.GetComponent<SpriteRenderer>();
            renderer.sprite = _lobbyPaintSprite;
        }
    }
}

public class Menu : MonoBehaviour
{
    // For how to create GUIs, see https://docs.unity3d.com/ScriptReference/GUILayout.html

    // Create a rect for the window
    public Rect windowRect = new(10f, 10f, 200f, 300f);
    public bool showWindow = true;

    // Override OnGUI to draw the window
    public void OnGUI()
    {
        if (!showWindow) return;
        // Set the ID to something unique because it can cause incompatibility with other mods
        // if two mods (plugins) are trying to use the same ID
        windowRect = GUILayout.Window(123, windowRect, (GUI.WindowFunction)WindowFunction, "Window Title");
    }

    // Start is called before the first frame update
    public void Start()
    {
        ImGui.Log.LogInfo("Start() was invoked!");
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
        // Add a label using GUILayout.Label
        GUILayout.Label($"Window ID: {id}");
        // Add a button using GUILayout.Button
        if (GUILayout.Button("Example Button"))
        {
            ImGui.Log.LogInfo($"Example Button was pressed!");
        }

        // Add a toggle using GUILayout.Toggle
        ImGui.IsToggleEnabled = GUILayout.Toggle(ImGui.IsToggleEnabled, "Example Toggle");

        if (ImGui.IsToggleEnabled)
        {
            // Add your logic here for when the toggle is enabled
            // Ideally you want to create a separate method to handle the toggle state
            // For example, you can log a message like below, but it will spam the console
            // ImGui.Log.LogInfo($"Example Toggle is enabled!");
        }
        else
        {
            // ImGui.Log.LogInfo($"Example Toggle is disabled!");
        }

        // Things like sliders or text fields do not work because of stripping

        // Add a slider using GUILayout.HorizontalSlider
        //var sliderValue = GUILayout.HorizontalSlider(0.5f, 0f, 1f);
        //ImGui.Log.LogInfo($"Slider value: {sliderValue}");

        // Add a text field using GUILayout.TextField
        //var textFieldValue = GUILayout.TextField("Text Field");
        //ImGui.Log.LogInfo($"Text field value: {textFieldValue}");

        // Add a text area using GUILayout.TextArea
        //var textAreaValue = GUILayout.TextArea("Text Area");
        //ImGui.Log.LogInfo($"Text area value: {textAreaValue}");


        // Make the window resizable
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Window Narrower"))
        {
            ImGui.Log.LogInfo("Make Window Narrower button was pressed!");
            windowRect.width -= 10f;
        }
        if (GUILayout.Button("Make Window Wider"))
        {
            ImGui.Log.LogInfo("Make Window Wider button was pressed!");
            windowRect.width += 10f;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Make Window Shorter"))
        {
            ImGui.Log.LogInfo("Make Window Shorter button was pressed!");
            windowRect.height -= 10f;
        }
        if (GUILayout.Button("Make Window Taller"))
        {
            ImGui.Log.LogInfo("Make Window Taller button was pressed!");
            windowRect.height += 10f;
        }
        GUILayout.EndHorizontal();

        // Make the window draggable
        GUI.DragWindow();
    }
}
