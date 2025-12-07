// PlatformCameraSwitcher.cs (FULL file - V10)
// Safe version: không tham chiếu trực tiếp tới PlatformConfig type (tránh CS0246)
// - Trong Editor: load asset ScriptableObject bằng AssetDatabase + reflection để đọc "CurrentPlatform"
// - Ngoài Editor: fallback theo build defines (PC vs Mobile)
// - Live update trong Editor nếu LiveEditorAuto = true
// - Attach lên CameraRig, assign tpsCameraRig & topdownCameraRig

using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class PlatformCameraSwitcher : MonoBehaviour
{
    public enum CameraMode { TPS, Topdown }

    [Header("Assign child rigs")]
    public GameObject tpsCameraRig;
    public GameObject topdownCameraRig;

    [Header("Live Editor Auto Update")]
    public bool LiveEditorAuto = true;

    // internal cache
    private string lastPlatformString = null;
    public CameraMode currentMode { get; private set; }

    // path tới asset (theo bạn đặt ở Assets/Editor/PlatformConfig.asset)
    private const string PLATFORM_CONFIG_PATH = "Assets/Editor/PlatformConfig.asset";

    void OnEnable()
    {
        // ensure initial application
        AutoSwitchByPlatform();

#if UNITY_EDITOR
        if (LiveEditorAuto)
        {
            // subscribe editor update to detect changes live
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
#endif
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (!LiveEditorAuto) return;
        AutoSwitchByPlatform();
    }
#endif

    void Update()
    {
        // runtime: keep checking in case platform value changed at runtime (rare)
        if (Application.isPlaying)
            AutoSwitchByPlatform();
    }

    /// <summary>
    /// Core: read platform string (try editor config via reflection, fallback to build defines).
    /// If changed from last tick -> apply appropriate camera mode.
    /// </summary>
    public void AutoSwitchByPlatform()
    {
        string platformString = GetPlatformString();

        // if unchanged -> do nothing
        if (platformString == lastPlatformString) return;

        lastPlatformString = platformString;

        // map to camera mode (case-insensitive)
        var key = (platformString ?? "").Trim().ToLower();

        if (key.Length == 0)
        {
            // fallback if nothing read
            DebugFallback();
            return;
        }

        if (key.Contains("pc") || key.Contains("standalone") || key.Contains("windows") || key.Contains("mac") || key.Contains("linux"))
        {
            SetMode(CameraMode.TPS);
        }
        else if (key.Contains("console"))
        {
            SetMode(CameraMode.TPS);
        }
        else if (key.Contains("mobile") || key.Contains("android") || key.Contains("ios") || key.Contains("iphone") || key.Contains("ipad"))
        {
            SetMode(CameraMode.Topdown);
        }
        else
        {
            // unknown string -> fallback
            DebugFallback();
        }

        Debug.Log($"[PlatformCameraSwitcher V10] Platform='{platformString}' -> Mode={currentMode}");
    }

    /// <summary>
    /// Tries to get platform as string:
    /// 1) If in Editor and asset exists at PLATFORM_CONFIG_PATH, load ScriptableObject and read field/property "CurrentPlatform" (reflection)
    /// 2) Else fallback to build defines (PC vs Mobile)
    /// </summary>
    private string GetPlatformString()
    {
#if UNITY_EDITOR
        // Try load asset at path as ScriptableObject (safe even if type PlatformConfig is editor-only)
        ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(PLATFORM_CONFIG_PATH);
        if (so != null)
        {
            // Try to read field "CurrentPlatform" or property "CurrentPlatform"
            Type t = so.GetType();

            // Field
            FieldInfo f = t.GetField("CurrentPlatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (f != null)
            {
                try
                {
                    object val = f.GetValue(so);
                    if (val != null) return val.ToString();
                }
                catch (Exception ex) { Debug.LogWarning($"[PlatformCameraSwitcher V10] Error reading field CurrentPlatform: {ex.Message}"); }
            }

            // Property
            PropertyInfo p = t.GetProperty("CurrentPlatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            if (p != null)
            {
                try
                {
                    object val = p.GetValue(so, null);
                    if (val != null) return val.ToString();
                }
                catch (Exception ex) { Debug.LogWarning($"[PlatformCameraSwitcher V10] Error reading property CurrentPlatform: {ex.Message}"); }
            }

            // As a last resort, try ToString of the ScriptableObject (not typical)
            try
            {
                return so.ToString();
            }
            catch { }
        }

        // If asset not found, try to find any ScriptableObject named PlatformConfig in loaded assemblies (defensive)
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                foreach (var type in asm.GetTypes())
                {
                    if (!typeof(ScriptableObject).IsAssignableFrom(type)) continue;
                    if (!string.Equals(type.Name, "PlatformConfig", StringComparison.OrdinalIgnoreCase)) continue;

                    // find instances of this type in the project by path
                    // Use AssetDatabase.FindAssets -> load the first
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject " + type.Name);
                    if (guids != null && guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        ScriptableObject found = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                        if (found != null)
                        {
                            // attempt to read field/property
                            FieldInfo ff = type.GetField("CurrentPlatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                            if (ff != null)
                            {
                                object v = ff.GetValue(found);
                                if (v != null) return v.ToString();
                            }
                            PropertyInfo pp = type.GetProperty("CurrentPlatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                            if (pp != null)
                            {
                                object v2 = pp.GetValue(found, null);
                                if (v2 != null) return v2.ToString();
                            }
                        }
                    }
                }
            }
            catch { /* ignore assembly scan exceptions */ }
        }
#endif

        // Fallback (runtime and editor if no config found)
#if UNITY_STANDALONE || UNITY_WEBGL
        return "PC";
#elif UNITY_ANDROID || UNITY_IOS
        return "Mobile";
#else
        return "PC";
#endif
    }

    private void DebugFallback()
    {
#if UNITY_STANDALONE || UNITY_WEBGL
        SetMode(CameraMode.TPS);
#elif UNITY_ANDROID || UNITY_IOS
        SetMode(CameraMode.Topdown);
#else
        SetMode(CameraMode.TPS);
#endif
    }

    /// <summary>
    /// Bật/tắt rigs
    /// </summary>
    public void SetMode(CameraMode newMode)
    {
        currentMode = newMode;

        if (tpsCameraRig != null) tpsCameraRig.SetActive(newMode == CameraMode.TPS);
        if (topdownCameraRig != null) topdownCameraRig.SetActive(newMode == CameraMode.Topdown);
    }

    public void ToggleMode()
    {
        SetMode(currentMode == CameraMode.TPS ? CameraMode.Topdown : CameraMode.TPS);
    }
}
