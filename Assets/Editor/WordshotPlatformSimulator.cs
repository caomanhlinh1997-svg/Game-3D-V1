#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class WordshotPlatformSimulator : EditorWindow
{
    private enum PlatformGroup { PC, Mobile, Console }
    private PlatformGroup selectedGroup = PlatformGroup.PC;

    private static readonly string CONFIG_PATH = "Assets/Editor/PlatformConfig.asset";
    private PlatformConfig config;

    // ============================================================
    // INITIALIZATION
    // ============================================================
    [MenuItem("WordshotTools/Platform Simulator")]
    public static void ShowWindow()
    {
        GetWindow<WordshotPlatformSimulator>("Platform Simulator");
    }

    private void OnEnable()
    {
        LoadOrCreateConfig();
        selectedGroup = (PlatformGroup)config.CurrentPlatform;
    }

    private void LoadOrCreateConfig()
    {
        config = AssetDatabase.LoadAssetAtPath<PlatformConfig>(CONFIG_PATH);

        if (config == null)
        {
            config = ScriptableObject.CreateInstance<PlatformConfig>();
            AssetDatabase.CreateAsset(config, CONFIG_PATH);
            AssetDatabase.SaveAssets();
            Debug.Log("[WordshotSimulator] Created new PlatformConfig.asset");
        }
    }

    // ============================================================
    // UI
    // ============================================================
    private void OnGUI()
    {
        GUILayout.Label("WORDSHOT – PLATFORM SIMULATOR (PRO v5.2)", EditorStyles.boldLabel);
        GUILayout.Space(8);

        GUILayout.Label("Select Platform Group", EditorStyles.label);

        selectedGroup = (PlatformGroup)GUILayout.Toolbar(
            (int)selectedGroup,
            new[] { "PC", "Mobile", "Console" }
        );

        GUILayout.Space(12);

        if (GUILayout.Button("Apply Selected Platform Settings", GUILayout.Height(40)))
        {
            ApplyPlatformSettings();
        }

        GUILayout.Space(10);

        GUILayout.Label("Current Chose: " + selectedGroup, EditorStyles.helpBox);
        GUILayout.Label("Current View:  " + config.CurrentPlatform, EditorStyles.helpBox);

        GUILayout.Space(6);
        EditorGUILayout.HelpBox(
            "GameView resolution must be changed manually in Unity 6.3.\n" +
            "Platform simulation features (layout, UI, camera mode, etc.) work normally.",
            MessageType.Info
        );
    }

    // ============================================================
    // APPLY PLATFORM
    // ============================================================
    private void ApplyPlatformSettings()
    {
        switch (selectedGroup)
        {
            case PlatformGroup.PC: ApplyPCMode(); break;
            case PlatformGroup.Mobile: ApplyMobileMode(); break;
            case PlatformGroup.Console: ApplyConsoleMode(); break;
        }

        config.CurrentPlatform = (PlatformType)selectedGroup;
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        Debug.Log($"[WordshotSimulator] Platform applied: {selectedGroup}");
    }

    // ============================================================
    // MODE LOGIC
    // ============================================================
    private void ApplyPCMode()
    {
        LoadUnityLayout("Default.wlt");
    }

    private void ApplyMobileMode()
    {
        LoadUnityLayout("Tall.wlt");
    }

    private void ApplyConsoleMode()
    {
        LoadUnityLayout("Default.wlt");
    }

    // ============================================================
    // LAYOUT LOADER
    // ============================================================
    private void LoadUnityLayout(string filename)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string[] searchPaths =
        {
            Path.Combine(appData, "Unity", "Editor-5.x", "Preferences", "Layouts"),
            Path.Combine(appData, "Unity", "Layouts"),
            Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath),
                         "Data/Resources/Layouts/Default")
        };

        foreach (string root in searchPaths)
        {
            if (!Directory.Exists(root)) continue;

            var files = Directory.GetFiles(root, "*.wlt", SearchOption.AllDirectories);

            foreach (var f in files)
            {
                if (f.EndsWith(filename))
                {
                    try
                    {
                        EditorUtility.LoadWindowLayout(f);
                        Debug.Log("[WordshotSimulator] Loaded layout: " + filename);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Failed loading layout: " + ex.Message);
                    }
                }
            }
        }
    }
}
#endif
