using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Corações")]
    public int coracoesMaximos = 3;
    public int coracoesAtuais;

    [Header("Invencibilidade")]
    public bool invencivel = false;

    void Start()
    {
        coracoesAtuais = coracoesMaximos;
    }

    public void ReceberDano()
    {
        if (invencivel) return;

        coracoesAtuais--;
        Debug.Log("Corações: " + coracoesAtuais + "/" + coracoesMaximos);

        if (coracoesAtuais <= 0)
            GameManager.Instance.PerderTentativa();
    }
}