using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float velocidade = 2f;
    public float distanciaDeteccao = 8f;
    public float danoContato = 10f;
    public float intervaloDano = 1f;

    private Rigidbody2D rb;
    private float timerDano = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        timerDano -= Time.deltaTime;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia < distanciaDeteccao)
        {
            Vector2 direcao = (player.position - transform.position).normalized;
            rb.linearVelocity = direcao * velocidade;

            if (distancia < 0.6f && timerDano <= 0)
            {
                PlayerStats stats = player.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.ReceberDano();
                    timerDano = intervaloDano;
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}