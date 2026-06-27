using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float velocidade = 2f;
    public float distanciaDeteccao = 8f;
    public float danoContato = 1f;
    public float intervaloDano = 1f;

    private Rigidbody2D rb;
    private float timerDano = 0f;
    private Animator animator;
    private EnemyStats stats;

    private static readonly int HashRun    = Animator.StringToHash("isRunning");
    private static readonly int HashAttack = Animator.StringToHash("attack");

    void Start()
    {
        rb       = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats    = GetComponent<EnemyStats>();

        if (stats != null && (stats.isMiniChefe || stats.isBoss))
            danoContato = 1f;
        else
            danoContato = 0.5f;
    }

    void Update()
    {
        // Para tudo se o inimigo já morreu
        if (stats != null && stats.morreu) return;
        if (player == null) return;

        timerDano -= Time.deltaTime;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia < distanciaDeteccao)
        {
            Vector2 direcao = (player.position - transform.position).normalized;
            rb.linearVelocity = direcao * velocidade;

            if (animator != null)
                animator.SetBool(HashRun, true);

            if (distancia < 0.1f && timerDano <= 0)
            {
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.ReceberDano(danoContato);
                    timerDano = intervaloDano;

                    if (animator != null)
                        animator.SetTrigger(HashAttack);
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetBool(HashRun, false);
        }
    }
}