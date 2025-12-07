using UnityEngine;

public class CursorFree : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;   // thả chuột
        Cursor.visible = true;                    // hiện chuột
    }
}
