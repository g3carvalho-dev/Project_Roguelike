using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float velocidade = 2f;
    public float distanciaDeteccao = 8f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia < distanciaDeteccao)
        {
            Vector2 direcao = (player.position - transform.position).normalized;
            rb.velocity = direcao * velocidade;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}