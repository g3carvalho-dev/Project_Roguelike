using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Corações")]
    public int coracoesMaximos = 3;
    public int coracoesAtuais;

    [Header("Tentativas")]
    public int tentativasMaximas = 3;
    public int tentativasAtuais;

    [Header("Invencibilidade")]
    public bool invencivel = false;

    void Start()
    {
        coracoesAtuais = coracoesMaximos;
        tentativasAtuais = tentativasMaximas;
    }

    public void ReceberDano()
    {
        if (invencivel) return;

        coracoesAtuais--;
        Debug.Log("Corações: " + coracoesAtuais + "/" + coracoesMaximos);

        if (coracoesAtuais <= 0)
            PerderTentativa();
    }

    void PerderTentativa()
    {
        tentativasAtuais--;
        Debug.Log("Tentativas restantes: " + tentativasAtuais);

        if (tentativasAtuais <= 0)
        {
            Debug.Log("Game Over!");
            // tela de game over depois
        }
        else
        {
            coracoesAtuais = coracoesMaximos;
            Debug.Log("Voltando ao checkpoint!");
            GameManager.Instance.VoltarParaCheckpoint();
        }
    }

    
}