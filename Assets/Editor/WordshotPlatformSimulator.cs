#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

/// <summary>
/// WORDSHOT PLATFORM SIMULATOR — PRO v5 (FINAL FOR OPTION A)
/// - Auto resolution (PC / Mobile / Console)
/// - Auto GameView selection (chọn đúng tỉ lệ, dù UI Unity có thể giữ label Free Aspect – KHÔNG ẢNH HƯỞNG)
/// - Auto Editor Layout (PC=Default, Mobile=Tall)
/// - No Popup
/// - Unity 6.x LTS safe
/// </summary>
public class WordshotPlatformSimulator : EditorWindow
{
    private enum PlatformGroup { PC, Mobile, Console }
    private PlatformGroup selectedGroup = PlatformGroup.PC;

    [MenuItem("WordshotTools/Platform Simulator")]
    public static void ShowWindow()
    {
        GetWindow<WordshotPlatformSimulator>("Platform Simulator");
    }

    private void OnGUI()
    {
        GUILayout.Label("WORDSHOT – PLATFORM SIMULATOR (PRO v5)", EditorStyles.boldLabel);
        GUILayout.Space(8);

        selectedGroup = (PlatformGroup)GUILayout.Toolbar(
            (int)selectedGroup, new[] { "PC", "Mobile", "Console" });

        GUILayout.Space(12);

        if (GUILayout.Button("Apply Selected Platform Settings", GUILayout.Height(40)))
        {
            ApplyPlatformSettings();
        }

        GUILayout.Space(10);
        GUILayout.Label("Current Mode: " + selectedGroup, EditorStyles.helpBox);
    }

    private void ApplyPlatformSettings()
    {
        switch (selectedGroup)
        {
            case PlatformGroup.PC:
                ApplyPCMode(); break;
            case PlatformGroup.Mobile:
                ApplyMobileMode(); break;
            case PlatformGroup.Console:
                ApplyConsoleMode(); break;
        }

        Debug.Log($"[WordshotSimulator] Applied: {selectedGroup}");
    }

    // =====================================================
    // PLATFORM IMPLEMENTATION
    // =====================================================

    private void ApplyPCMode()
    {
        GameConfig.Platform = PlatformType.PC;

        ForceGameViewSize(1920, 1080, "PC_1920x1080");
        LoadUnityLayoutByName("Default.wlt");
    }

    private void ApplyMobileMode()
    {
        GameConfig.Platform = PlatformType.Mobile;

        ForceGameViewSize(1080, 1920, "Mobile_1080x1920");
        LoadUnityLayoutByName("Tall.wlt");
    }

    private void ApplyConsoleMode()
    {
        GameConfig.Platform = PlatformType.Console;

        ForceGameViewSize(1920, 1080, "Console_1080p");
        LoadUnityLayoutByName("Default.wlt");
    }

    // =====================================================
    // GAMEVIEW REFLECTION API
    // =====================================================

    private void ForceGameViewSize(int width, int height, string label)
    {
        try
        {
            var sizesType = Type.GetType("UnityEditor.GameViewSizes, UnityEditor");
            var singletonType = Type.GetType(
                "UnityEditor.ScriptableSingleton`1[[UnityEditor.GameViewSizes, UnityEditor]], UnityEditor");

            if (sizesType == null || singletonType == null)
            {
                Debug.LogWarning("[WordshotSimulator] GameViewSizes API is not available.");
                return;
            }

            var instance = singletonType.GetProperty("instance").GetValue(null);
            var getGroup = sizesType.GetMethod("GetGroup");

            int groupIndex = GetCurrentGroup();
            var group = getGroup.Invoke(instance, new object[] { groupIndex });

            int index = FindSize(group, width, height, label);
            if (index == -1)
            {
                AddCustomSize(group, width, height, label);
                index = FindSize(group, width, height, label);
            }

            if (index != -1)
                SetGameViewSize(index);

        }
        catch (Exception ex)
        {
            Debug.LogWarning("[WordshotSimulator] ForceGameViewSize failed: " + ex.Message);
        }
    }

    private void SetGameViewSize(int index)
    {
        var gvType = Type.GetType("UnityEditor.GameView,UnityEditor");
        var gameView = EditorWindow.GetWindow(gvType);
        var property = gvType.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        property?.SetValue(gameView, index, null);

        // Force refresh UI
        gameView.Repaint();
        EditorApplication.delayCall += gameView.Repaint;
        EditorApplication.delayCall += () =>
            EditorWindow.FocusWindowIfItsOpen(gvType);
    }

    private int GetCurrentGroup()
    {
#if UNITY_ANDROID
        return 1;
#elif UNITY_IOS
        return 2;
#else
        return 0;
#endif
    }

    private int FindSize(object group, int w, int h, string label)
    {
        var getBuiltin = group.GetType().GetMethod("GetBuiltinCount");
        var getCustom = group.GetType().GetMethod("GetCustomCount");
        var getSize = group.GetType().GetMethod("GetGameViewSize");

        int builtin = (int)getBuiltin.Invoke(group, null);
        int custom = (int)getCustom.Invoke(group, null);
        int total = builtin + custom;

        for (int i = 0; i < total; i++)
        {
            var obj = getSize.Invoke(group, new object[] { i });
            var t = obj.GetType();

            int w2 = (int)t.GetProperty("width").GetValue(obj);
            int h2 = (int)t.GetProperty("height").GetValue(obj);
            string l = (string)t.GetProperty("baseText").GetValue(obj);

            if (w2 == w && h2 == h && l == label)
                return i;
        }
        return -1;
    }

    private void AddCustomSize(object group, int w, int h, string label)
    {
        var add = group.GetType().GetMethod("AddCustomSize");
        var sizeType = Type.GetType("UnityEditor.GameViewSize, UnityEditor");
        var enumType = Type.GetType("UnityEditor.GameViewSizeType, UnityEditor");

        var ctor = sizeType.GetConstructor(new[] {
            enumType, typeof(int), typeof(int), typeof(string)
        });

        var newSize = ctor.Invoke(new object[] { 1, w, h, label });
        add.Invoke(group, new[] { newSize });
    }

    // =====================================================
    // FIND + LOAD LAYOUT
    // =====================================================

    private void LoadUnityLayoutByName(string filename)
    {
        string path = FindLayout(filename);

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                EditorUtility.LoadWindowLayout(path);
                Debug.Log($"[WordshotSimulator] Loaded layout: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[WordshotSimulator] Failed to load layout: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning($"[WordshotSimulator] Layout not found: {filename}");
        }
    }

    private string FindLayout(string filename)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        string[] roots =
        {
            Path.Combine(appData, "Unity", "Editor-5.x", "Preferences", "Layouts"),
            Path.Combine(appData, "Unity", "Editor-5.x", "Layouts"),
            Path.Combine(appData, "Unity", "Layouts"),

            Path.Combine(Application.dataPath, "..", "Library", "Layouts"),

            Path.Combine(
                Path.GetDirectoryName(EditorApplication.applicationPath),
                "Data/Resources/Layouts/Default")
        };

        foreach (var root in roots)
        {
            if (!Directory.Exists(root)) continue;

            var matches = Directory.GetFiles(root, "*.wlt", SearchOption.AllDirectories);
            foreach (var file in matches)
            {
                if (file.EndsWith(filename, StringComparison.OrdinalIgnoreCase))
                    return file;

                if (file.Contains(Path.GetFileNameWithoutExtension(filename)))
                    return file;
            }
        }

        return null;
    }
}
#endif
