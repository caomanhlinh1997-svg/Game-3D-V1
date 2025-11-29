using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;

    // hiện trong Inspector nhưng private để tránh bên ngoài gán lung tung.
    [SerializeField] private int currentHP;

    // để đảm bảo Die() chỉ chạy 1 lần
    bool isDead = false;

    Animator anim;
    CharacterController cc;
    PlayerController playerCtrl;

    void Awake()
    {
        // khởi tạo
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        playerCtrl = GetComponent<PlayerController>();
    }

    // cho phép đọc từ code khác, vẫn không cho set trực tiếp
    public int CurrentHP => currentHP;

    // Gọi hàm này khi muốn gây sát thương lên Player
    public void TakeDamage(int dmg)
    {
        if (isDead) return;        // đã chết thì thôi

        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, int.MinValue, maxHP); // chống tràn/giới hạn trên

        Debug.Log("Player HP: " + currentHP);

        // nếu muốn cập nhật UI: gọi event ở đây (mình để comment)
        // OnHPChanged?.Invoke(currentHP);

        if (currentHP <= 0 && !isDead)
        {
            Die();
        }
    }

    // heal (nếu cần)
    public void Heal(int amount)
    {
        if (isDead) return;
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        // OnHPChanged?.Invoke(currentHP);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died!");

        // tắt control
        if (cc != null) cc.enabled = false;
        if (playerCtrl != null) playerCtrl.enabled = false;

        // trigger animation nếu có trigger "die"
        if (anim != null) anim.SetTrigger("die");

        // tùy bạn: show GameOver, respawn, reload scene...
    }

    // --- dùng để reset nhanh khi test trong Inspector (context menu)
    [ContextMenu("ResetHP")]
    void ResetHP()
    {
        currentHP = maxHP;
        isDead = false;
        if (cc != null) cc.enabled = true;
        if (playerCtrl != null) playerCtrl.enabled = true;
    }
}
