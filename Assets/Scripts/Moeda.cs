using UnityEngine;

public class Moeda : MonoBehaviour
{
    private bool coletada = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (coletada) return;

        if (other.CompareTag("Player"))
        {
            coletada = true;
            GameManager.Instance?.AdicionarMoeda(1);
            Destroy(gameObject);
        }
    }
}
