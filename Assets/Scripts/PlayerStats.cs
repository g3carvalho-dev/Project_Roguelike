using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coracoes")]
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