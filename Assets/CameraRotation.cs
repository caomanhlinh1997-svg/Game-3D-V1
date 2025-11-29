using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform cam;
    public float sensitivity = 150f;

    float rotX = 0f;
    float rotY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;

        rotX = Mathf.Clamp(rotX, -25f, 60f);

        // Xoay CameraRig theo hướng ngang
        transform.rotation = Quaternion.Euler(0f, rotY, 0f);

        // Xoay Camera theo hướng dọc
        cam.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }
}
