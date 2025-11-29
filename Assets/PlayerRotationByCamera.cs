// PlayerRotationByCamera.cs
// Script này làm cho Player luôn quay theo hướng camera
// Gắn vào Player của bạn (HPCharacter chẳng hạn)

using UnityEngine;

public class PlayerRotationByCamera : MonoBehaviour
{
    public Transform cameraTransform;  // gắn camera vào đây

    public float rotateSpeed = 10f;    // tốc độ quay nhân vật

    void Update()
    {
        // Lấy hướng của camera theo trục Y (không nghiêng lên xuống)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;   // xóa độ nghiêng lên/xuống

        // Nếu camera không xoay đứng (trường hợp hiếm)
        if (camForward.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
