using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Camera")]
    public Camera cam;

    [Header("Gun Settings")]
    public Transform muzzlePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;
    public float fireRate = 0.15f;
    private float fireTimer = 0f;

    [Header("Damage")]
    public int damage = 20;
    public float range = 50f;
    public LayerMask hitLayers;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            fireTimer = fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        // 1) Raycast theo crosshair
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            targetPoint = hit.point;

            // Damage
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
        else
        {
            // Không trúng gì
            targetPoint = ray.GetPoint(50);
        }

        // 2) Hướng đạn
        Vector3 dir = (targetPoint - muzzlePoint.position).normalized;

        // 3) Spawn bullet với rotation đúng hướng bay
        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(dir)
        );
        // Sửa hướng viên đạn bằng cách thêm offset xoay
        bullet.transform.Rotate(90, 0, 0);  // thử xoay trục X trước


        // 4) Add velocity
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * bulletSpeed;

        // 5) Ngăn cho đạn không tự xoay lung tung
        rb.angularVelocity = Vector3.zero;
    }
}
