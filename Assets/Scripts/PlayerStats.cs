using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coracoes")]
    public int coracoesMaximos = 3;
    public int coracoesAtuais;

    [Header("Tentativas")]
    public int tentativasMaximas = 3;
    public int tentativasAtuais;

    [Header("Invencibilidade")]
    public bool invencivel = false;
    public float duracaoInvencibilidade = 1f;
    private float timerInvencibilidade = 0f;

    void Start()
    {
        coracoesAtuais = coracoesMaximos;
        tentativasAtuais = tentativasMaximas;
    }

    void Update()
    {
        if (timerInvencibilidade > 0)
            timerInvencibilidade -= Time.deltaTime;
    }

    public void ReceberDano()
    {
        if (invencivel || timerInvencibilidade > 0) return;

        coracoesAtuais--;
        timerInvencibilidade = duracaoInvencibilidade;
        Debug.Log("Coracoes: " + coracoesAtuais + "/" + coracoesMaximos);

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
        }
        else
        {
            coracoesAtuais = coracoesMaximos;
            Debug.Log("Voltando para a sala de repouso para trocar equipamentos!");
            GameManager.Instance.VoltarParaSalaDeRepousoTroca();
        }
    }
}