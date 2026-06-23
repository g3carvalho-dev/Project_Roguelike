using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerDash playerDash;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDash = GetComponent<PlayerDash>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("SpeedX", x);
        animator.SetFloat("SpeedY", y);
        animator.SetBool("IsDashing", playerDash != null && playerDash.dashando);

        // sem flip - as animacoes ja tem as direcoes corretas
        spriteRenderer.flipX = false;
    }
}