// File: Assets/Scripts/ShiningTome.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShiningTome : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string puzzleSceneName = "PuzzleScene"; // Tên Scene phải có trong Build Settings
    public float pickupDelay = 0.25f; // Delay trước khi load scene (cho animation/particle)
    public bool onlyOnButtonPress = false; // true nếu muốn bắt người chơi nhấn phím để nhặt
    public KeyCode pickupKey = KeyCode.E; // phím tương tác nếu onlyOnButtonPress = true

    [Header("Effects / References")]
    public ParticleSystem glowParticles; // gán ParticleSystem (child) trong Inspector
    public Animator animator; // gán Animator nếu có animation nhặt

    bool playerInRange = false;
    bool pickedUp = false;

    // Giải thích: Start dùng để fallback nếu bạn quên kéo references trong Inspector.
    void Start()
    {
        if (glowParticles == null)
            glowParticles = GetComponentInChildren<ParticleSystem>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Giải thích: kiểm tra collider để tránh lỗi logic (3D collider ở đây)
        Collider c = GetComponent<Collider>();
        if (c == null)
        {
            Debug.LogWarning("ShiningTome: Không tìm thấy Collider. Thêm BoxCollider (Is Trigger = true).");
        }
        else if (!c.isTrigger)
        {
            Debug.LogWarning("ShiningTome: Collider nên đặt Is Trigger = true để tránh xung đột vật lý.");
        }
    }

    // Giải thích: dùng trigger 3D. Player object phải có Tag \"Player\".
    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartGlow();

            if (!onlyOnButtonPress)
            {
                // auto pickup khi vào trigger
                TryPickup();
            }
            else
            {
                // nếu muốn: show UI hint tại đây (ví dụ: UIManager.ShowHint("Nhấn E để nhặt"))
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopGlow();
            // ẩn UI hint nếu có
        }
    }

    void Update()
    {
        if (pickedUp) return;

        if (onlyOnButtonPress && playerInRange && Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    void StartGlow()
    {
        if (glowParticles != null && !glowParticles.isPlaying)
            glowParticles.Play();

        // Nếu muốn, có thể bật emission của material ở đây
        // Renderer r = GetComponentInChildren<Renderer>(); if (r) r.material.EnableKeyword("_EMISSION");
    }

    void StopGlow()
    {
        if (glowParticles != null && glowParticles.isPlaying)
            glowParticles.Stop();

        // tắt emission nếu đã bật
    }

    void TryPickup()
    {
        if (pickedUp) return;
        pickedUp = true;

        // Play pickup animation nếu có Animator
        if (animator != null)
        {
            animator.SetTrigger("Pickup"); // đảm bảo Animator có trigger parameter "Pickup"
        }

        // Burst particle khi nhặt (nếu muốn)
        if (glowParticles != null)
            glowParticles.Play();

        // Giải thích: delay để animation/ef kịp chạy
        Invoke(nameof(LoadPuzzleScene), pickupDelay);
    }

    void LoadPuzzleScene()
    {
        // Giải thích: tại đây bạn có thể lưu trạng thái (ví dụ: add item vào inventory)
        // Ví dụ: InventorySystem.Instance?.Add("TomeOfPuzzle");

        // Load scene bằng tên (phải có trong Build Settings)
        SceneManager.LoadScene(puzzleSceneName);
    }
}
