using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerDash playerDash;
    private PlayerStats playerStats;

    private bool isDamaged = false;
    private float damageTimer = 0f;
    public float damageDuration = 0.3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDash = GetComponent<PlayerDash>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.jogoPausado) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("SpeedX", x);
        animator.SetFloat("SpeedY", y);
        animator.SetBool("IsDashing", playerDash != null && playerDash.dashando);

        if (isDamaged)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0)
            {
                isDamaged = false;
                animator.SetBool("IsDamaged", false);
            }
        }

        spriteRenderer.flipX = false;
    }

    public void TriggerDamage()
    {
        isDamaged = true;
        damageTimer = damageDuration;
        animator.SetBool("IsDamaged", true);
        Debug.Log("TriggerDamage chamado!");
    }
}