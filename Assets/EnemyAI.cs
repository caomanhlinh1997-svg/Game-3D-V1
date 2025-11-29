using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    NavMeshAgent agent;
    Animator anim;

    public float detectRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    float lastAttack = -999f;
    bool didDamageThisAttack = false;  // đảm bảo 1 hit = 1 damage

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        // Nếu quá xa → Idle
        if (dist > detectRange)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            return;
        }

        // Nếu trong tầm đánh
        if (dist <= attackRange)
        {
            agent.isStopped = true;
            FacePlayer();

            // Bắt đầu 1 attack mới
            if (Time.time - lastAttack > attackCooldown)
            {
                anim.SetTrigger("attack");
                lastAttack = Time.time;
                didDamageThisAttack = false; // chuẩn bị gây damage
            }

            // Nếu animation mới bắt đầu và damage chưa gây → gây 1 lần
            if (!didDamageThisAttack && Time.time - lastAttack > 0.25f)
            {
                // 0.25f = delay để trùng khung vung chém (tối ưu sau bằng Animation Event)
                player.GetComponent<PlayerHealth>()?.TakeDamage(10);
                didDamageThisAttack = true;
            }

            anim.SetFloat("Speed", 0);
            return;
        }

        // Nếu trong tầm phát hiện → di chuyển
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);

        // Reset damage flag khi rời khỏi trạng thái attack
        didDamageThisAttack = false;
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }
}
