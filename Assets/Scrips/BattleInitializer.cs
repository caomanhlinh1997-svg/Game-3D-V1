// BattleInitializer.cs
// Đặt file này vào: Assets/Scripts/Core/BattleInitializer.cs
// Full file — dán nguyên, không patch.
// Mục đích: xử lý khởi tạo scene theo GameConfig.Platform, tránh lỗi compile
using UnityEngine;

public class BattleInitializer : MonoBehaviour
{
    [Header("References (gán trong Inspector)")]
    [Tooltip("Camera controller script (tự tạo hoặc wrapper cho Cinemachine). Nếu không có, để null.")]
    public MonoBehaviour cameraController; // chỉ cần reference để gọi method SetTopDown/SetPCMode nếu có

    [Tooltip("Player controller (script của bạn, dùng để bật/tắt joystick mode).")]
    public MonoBehaviour playerController;

    [Tooltip("Canvas GameObject chứa joystick UI (bật khi mobile).")]
    public GameObject joystickUI;

    [Tooltip("Crosshair UI (bật khi PC/Console).")]
    public GameObject crosshairUI;

    [Tooltip("Optional: GameObject chứa mobile-only HUD (energy, big HP, etc).")]
    public GameObject mobileHUD;

    [Tooltip("Optional: GameObject chứa console-only HUD.")]
    public GameObject consoleHUD;

    void Awake()
    {
        // Nếu muốn chạy script trước các script khác có thể đặt Execution Order.
        // Không làm thay đổi project settings ở bước này.
    }

    void Start()
    {
        // Khi Scene bắt đầu, đọc GameConfig.Platform (do Platform Simulator set)
        // và gọi Setup tương ứng. Nếu GameConfig không tồn tại, mặc định PC.
        try
        {
            switch (GameConfig.Platform)
            {
                case PlatformType.Mobile:
                    SetupMobile();
                    break;
                case PlatformType.Console:
                    SetupConsole();
                    break;
                case PlatformType.PC:
                default:
                    SetupPC();
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[BattleInitializer] Exception during Start: " + ex.Message);
            // fallback an toàn
            SetupPC();
        }
    }

    // ---------------------------
    // FULL IMPLEMENTATION (skeleton + safe defaults)
    // Bạn có thể mở rộng các hàm này sau.
    // ---------------------------

    /// <summary>
    /// Thiết lập cho PC: landscape, mouse + keyboard, crosshair on, joystick off.
    /// </summary>
    public void SetupPC()
    {
        Debug.Log("[BattleInitializer] SetupPC() called.");

        // 1) Màn hình / orientation (chỉ áp dụng trong Editor hoặc standalone)
#if UNITY_EDITOR || UNITY_STANDALONE
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        // SetResolution chỉ dùng trong Editor/Standalone. Không gây hại trong build.
        Screen.SetResolution(1920, 1080, false);
#endif

        // 2) UI
        if (joystickUI != null) joystickUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(true);
        if (mobileHUD != null) mobileHUD.SetActive(false);
        if (consoleHUD != null) consoleHUD.SetActive(false);

        // 3) PlayerController mode flags (nếu script có interface)
        // Attempt to call standard method names via reflection (safe)
        TryCallMethod(playerController, "EnablePCControl");
        TryCallMethod(playerController, "DisableMobileControl");
        TryCallMethod(playerController, "SetUseJoystick", false);

        // 4) Camera
        TryCallMethod(cameraController, "SetPCMode");
        TryCallMethod(cameraController, "SetTPSCamera");

        // 5) Any other settings: disable auto-aim (if exists)
        TryCallMethod(playerController, "SetAutoAim", false);
        TryCallMethod(playerController, "SetAutoShoot", false);
    }

    /// <summary>
    /// Thiết lập cho Mobile: portrait, joystick on, auto-aim enabled, mobile HUD.
    /// </summary>
    public void SetupMobile()
    {
        Debug.Log("[BattleInitializer] SetupMobile() called.");

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.SetResolution(1080, 1920, false);
#endif

        // UI
        if (joystickUI != null) joystickUI.SetActive(true);
        if (crosshairUI != null) crosshairUI.SetActive(false);
        if (mobileHUD != null) mobileHUD.SetActive(true);
        if (consoleHUD != null) consoleHUD.SetActive(false);

        // Player controller
        TryCallMethod(playerController, "EnableMobileControl");
        TryCallMethod(playerController, "SetUseJoystick", true);

        // Camera
        TryCallMethod(cameraController, "SetTopDown");
        TryCallMethod(cameraController, "SetMobileCamera");

        // Auto aim + auto shoot ON for mobile profile
        TryCallMethod(playerController, "SetAutoAim", true);
        TryCallMethod(playerController, "SetAutoShoot", true);
    }

    /// <summary>
    /// Thiết lập cho Console: landscape, controller input, TPS camera, aim assist.
    /// </summary>
    public void SetupConsole()
    {
        Debug.Log("[BattleInitializer] SetupConsole() called.");

#if UNITY_EDITOR || UNITY_STANDALONE
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.SetResolution(1920, 1080, false);
#endif

        // UI
        if (joystickUI != null) joystickUI.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(true);
        if (mobileHUD != null) mobileHUD.SetActive(false);
        if (consoleHUD != null) consoleHUD.SetActive(true);

        // PlayerController -> enable gamepad mode
        TryCallMethod(playerController, "EnableConsoleControl");
        TryCallMethod(playerController, "SetUseJoystick", false);

        // Camera
        TryCallMethod(cameraController, "SetPCMode");
        TryCallMethod(cameraController, "SetTPSCamera");

        // Aim assist on (by default for console)
        TryCallMethod(playerController, "SetAutoAim", true);
        TryCallMethod(playerController, "SetAutoShoot", false);
    }

    // ---------------------------
    // Helper utilities
    // ---------------------------

    /// <summary>
    /// Thử gọi 1 method public nếu tồn tại trên component (dùng reflection, safe).
    /// Nếu method không tồn tại, không throws exception (chỉ log debug).
    /// </summary>
    private void TryCallMethod(MonoBehaviour target, string methodName)
    {
        TryCallMethod(target, methodName, null);
    }

    private void TryCallMethod(MonoBehaviour target, string methodName, object parameter)
    {
        if (target == null) return;

        var t = target.GetType();
        var m = t.GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (m != null)
        {
            try
            {
                if (parameter == null)
                    m.Invoke(target, null);
                else
                    m.Invoke(target, new object[] { parameter });
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[BattleInitializer] Error invoking {methodName} on {target.name}: {ex.Message}");
            }
        }
        else
        {
            // không có method, log debug để bạn biết (bình thường)
            // Debug.Log($"[BattleInitializer] Method {methodName} not found on {target?.name}");
        }
    }
}
