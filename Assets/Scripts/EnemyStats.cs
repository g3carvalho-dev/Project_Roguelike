using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float vidaMaxima = 50f;
    public float vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        Debug.Log(gameObject.name + " tomou " + dano + " de dano. Vida: " + vidaAtual);

        if (vidaAtual <= 0)
            Morrer();
    }

    void Morrer()
    {
        Debug.Log(gameObject.name + " morreu!");
        Destroy(gameObject);
    }
}