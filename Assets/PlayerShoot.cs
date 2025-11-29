using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public int damage = 20;
    public float range = 50f;

    public LayerMask hitLayers;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitLayers))
        {
            // Nếu trúng enemy, gọi damage
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // hiệu ứng trúng đạn (sẽ làm sau)
            Debug.Log("Hit: " + hit.collider.name);
        }
    }
}
