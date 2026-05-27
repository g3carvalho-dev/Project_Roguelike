using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 100f;
    public float vidaAtual;

    [Header("Tentativas")]
    public int tentativasMaximas = 3;
    public int tentativasAtuais;

    void Start()
    {
        vidaAtual = vidaMaxima;
        tentativasAtuais = tentativasMaximas;
    }

    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        Debug.Log("Player tomou " + dano + " de dano. Vida: " + vidaAtual);

        if (vidaAtual <= 0)
            Morrer();
    }

    void Morrer()
    {
        tentativasAtuais--;
        Debug.Log("Player morreu! Tentativas restantes: " + tentativasAtuais);

        if (tentativasAtuais <= 0)
        {
            Debug.Log("Game Over!");
            // aqui vai chamar a tela de game over depois
        }
        else
        {
            // volta ao checkpoint com vida cheia
            vidaAtual = vidaMaxima;
            Debug.Log("Voltando ao checkpoint...");
        }
    }
}