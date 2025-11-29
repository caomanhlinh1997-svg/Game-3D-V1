using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 30;

    [SerializeField] private int currentHP;   // ← HIỆN TRONG INSPECTOR
    private bool isDead = false;

    Animator anim;
    UnityEngine.AI.NavMeshAgent agent;
    EnemyAI ai;

    void Awake()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        ai = GetComponent<EnemyAI>();
    }

    public int CurrentHP => currentHP; // đọc HP từ code khác nếu cần

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        anim.SetTrigger("hit");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        anim.SetTrigger("die");

        if (ai != null) ai.enabled = false;
        if (agent != null) agent.enabled = false;

        Destroy(gameObject, 2f);
    }
}
