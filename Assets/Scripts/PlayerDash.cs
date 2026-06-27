using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float velocidadeDash = 15f;
    public float duracaoDash = 0.2f;
    public float cooldownDash = 1f;

    public bool dashando = false;
    private float timerDash = 0f;
    private float timerCooldown = 0f;
    private Vector2 direcaoDash;
    private Rigidbody2D rb;
    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.jogoPausado) return;

        timerCooldown -= Time.deltaTime;

        if (dashando)
        {
            timerDash -= Time.deltaTime;
            rb.linearVelocity = direcaoDash * velocidadeDash;

            if (timerDash <= 0)
            {
                dashando = false;
                rb.linearVelocity = Vector2.zero;

                if (playerController != null)
                    playerController.enabled = true;

                PlayerStats stats = GetComponent<PlayerStats>();
                if (stats != null)
                    stats.invencivel = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && timerCooldown <= 0)
            IniciarDash();
    }

    void IniciarDash()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        direcaoDash = new Vector2(x, y).normalized;

        if (direcaoDash == Vector2.zero)
            direcaoDash = Vector2.right;

        dashando = true;
        timerDash = duracaoDash;
        timerCooldown = cooldownDash;

        if (playerController != null)
            playerController.enabled = false;

        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
            stats.invencivel = true;

        Debug.Log("Dash!");
    }
}