// CameraFollow_Topdown.cs (FULL file - V3, optimized for PlatformCameraSwitcher V8)
// Dùng cho chế độ camera Topdown (Mobile)
// - Offset tuyệt đối (tuỳ chỉnh X,Y,Z và hoạt động đúng như bạn thấy trong Scene)
// - SmoothFollow
// - Snap về vị trí đúng ngay frame đầu khi camera được bật
// - Không normalize offset
// - Tự LookAt player
// - Tương thích 100% với việc bật/tắt từ PlatformCameraSwitcher

using UnityEngine;

public class CameraFollow_Topdown : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target;      // Player hay character chính

    [Header("Camera Offset (tuyệt đối)")]
    public Vector3 offset = new Vector3(0, 35, -25);

    [Header("Smooth")]
    public float smoothTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    private bool firstSnapDone = false;

    void OnEnable()
    {
        // Khi camera topdown được bật bởi PlatformCameraSwitcher
        // → Snap ngay lập tức để tránh giật khung đầu
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }

        firstSnapDone = false; 
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;

        // Frame đầu sau enable → snap hoàn toàn
        if (!firstSnapDone)
        {
            transform.position = desiredPos;
            transform.LookAt(target.position);
            firstSnapDone = true;
            return;
        }

        // Các frame tiếp theo → follow mượt
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);

        // Nhìn về player
        transform.LookAt(target.position);
    }
}
