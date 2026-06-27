using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 10f;
    public float dano = 10f;
    private Vector2 direcao;

    public void Iniciar(Vector2 dir)
    {
        direcao = dir.normalized;

        // Rotaciona o projétil para apontar na direção do movimento
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.jogoPausado) return;

        transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Parede"))
        {
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Inimigo"))
        {
            EnemyStats stats = other.GetComponent<EnemyStats>();
            if (stats != null)
                stats.ReceberDano(dano);
            Destroy(gameObject);
        }
    }
}