using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 10f;
    private Vector2 direcao;

    public void Iniciar(Vector2 dir)
    {
        direcao = dir.normalized;
    }

    void Update()
    {
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
                stats.ReceberDano(10f);
            Destroy(gameObject);
        }
    }
}