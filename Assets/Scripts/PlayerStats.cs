using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coracoes")]
    public int coracoesMaximos = 3;
    public int coracoesAtuais;

    [Header("Invencibilidade")]
    public bool invencivel = false;

    private PlayerReliquia playerReliquia;

    void Start()
    {
        coracoesAtuais = coracoesMaximos;
        playerReliquia = GetComponent<PlayerReliquia>();
    }

    public void ReceberDano()
    {
        if (invencivel) return;

        // Defesa dá chance de bloquear o golpe sem perder coração
        if (playerReliquia != null && Random.value <= playerReliquia.bonusDefesa)
        {
            Debug.Log("Golpe bloqueado pela Defesa!");
            return;
        }

        coracoesAtuais--;
        Debug.Log("Coracoes: " + coracoesAtuais + "/" + coracoesMaximos);

        PlayerAnimator anim = GetComponent<PlayerAnimator>();
        if (anim != null)
            anim.TriggerDamage();

        if (coracoesAtuais <= 0)
        {
            coracoesAtuais = coracoesMaximos;
            GameManager.Instance.PerderTentativa();
        }
    }
}