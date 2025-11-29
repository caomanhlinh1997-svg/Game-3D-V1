using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTS ---
    Animator anim;           // Animator để điều khiển animation
    CharacterController cc;  // CharacterController để di chuyển

    // --- CAMERA ---
    Camera mainCam;          // Camera dùng làm tham chiếu hướng di chuyển

    // --- THÔNG SỐ CHUYỂN ĐỘNG ---
    [Header("Movement")]
    public float moveSpeed = 4f;       // tốc độ đi/chạy
    public float turnSpeed = 10f;      // tốc độ quay mặt nhân vật (cao = quay nhanh)
    public float gravity = -9.81f;     // trọng lực
    public float jumpForce = 6f;       // lực nhảy

    // --- ANIMATION SMOOTHING ---
    [Header("Animation")]
    [Tooltip("Thời gian damping khi cập nhật Speed cho Animator")]
    public float speedDampTime = 0.05f;

    float velocityY;   // vận tốc y dùng cho gravity

    void Start()
    {
        // lấy component
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        // lấy camera chính (nếu bạn có camera khác, gán thủ công vào mainCam)
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogWarning("PlayerController: Không tìm thấy Camera.main. Hãy gán camera thủ công nếu có nhiều camera.");
    }

    void Update()
    {
        // -----------------
        // 1. LẤY INPUT W A S D (world-space relative to camera)
        // -----------------
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f); // tránh diagonal > 1

        // -----------------
        // 2. CHUẨN HÓA THEO CAMERA (camera-relative movement)
        //    Nếu không có camera (ví dụ trong editor test), fallback về world forward
        // -----------------
        Vector3 move = Vector3.zero;
        if (mainCam != null)
        {
            // hướng camera nhưng bỏ component y (chỉ lấy projection lên mặt phẳng XZ)
            Vector3 camForward = Vector3.ProjectOnPlane(mainCam.transform.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(mainCam.transform.right, Vector3.up).normalized;

            // di chuyển theo hướng camera
            move = camForward * v + camRight * h;
            // nếu input nhỏ thì move sẽ là 0
            if (move.sqrMagnitude > 1f)
                move.Normalize();
        }
        else
        {
            // fallback - dùng world axes
            move = new Vector3(h, 0f, v);
            move = Vector3.ClampMagnitude(move, 1f);
        }

        // -----------------
        // 3. GỬI SPEED CHO ANIMATOR (với damping để tránh lag)
        // -----------------
        // sử dụng magnitude của 'move' (đã là camera-relative)
        anim.SetFloat("Speed", move.magnitude, speedDampTime, Time.deltaTime);

        // -----------------
        // 4. QUAY NHÂN VẬT THEO HƯỚNG DI CHUYỂN (GTA style)
        //    - Nếu có input, quay mượt về hướng move
        // -----------------
        if (move.sqrMagnitude > 0.001f)
        {
            // tạo hướng quay
            Quaternion targetRot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(turnSpeed * Time.deltaTime));
        }

        // -----------------
        // 5. NHẢY: xử lý grounded + set IsJump
        // -----------------
        if (cc.isGrounded)
        {
            // khi trên đất, reset velocityY nếu âm
            if (velocityY < 0)
                velocityY = -1f; // small downward to keep grounded

            anim.SetBool("IsJump", false);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("IsJump", true);
                velocityY = jumpForce;
            }
        }
        else
        {
            // đang trên không, giữ IsJump true (nếu muốn)
            anim.SetBool("IsJump", true);
        }

        // -----------------
        // 6. TÍNH TRỌNG LỰC VÀ DI CHUYỂN
        // -----------------
        velocityY += gravity * Time.deltaTime;

        // final velocity = (horizontal movement * speed) + vertical velocity
        Vector3 finalVel = move * moveSpeed;
        finalVel.y = velocityY;

        // di chuyển character
        cc.Move(finalVel * Time.deltaTime);

        // -----------------
        // 7. CÁC HÀNH ĐỘNG KHÁC (Attack / Reload / Die)
        // -----------------
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Reload");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetTrigger("Die");
        }
    }
}
