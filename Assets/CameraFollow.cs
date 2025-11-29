using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            followSpeed * Time.deltaTime
        );
    }
}
