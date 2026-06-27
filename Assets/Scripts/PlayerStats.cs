using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Coracoes")]
    public float coracoesMaximos = 3f;
    public float coracoesAtuais;

    [Header("Invencibilidade")]
    public bool invencivel = false;

    public System.Action onVidaAtualizada;

    private PlayerReliquia playerReliquia;

    void Start()
    {
        if (coracoesMaximos <= 0)
            coracoesMaximos = 3f;

        coracoesAtuais = coracoesMaximos;
        playerReliquia = GetComponent<PlayerReliquia>();
        onVidaAtualizada?.Invoke();
    }

    public void ReceberDano(float dano = 1f)
    {
        if (invencivel) return;

        if (playerReliquia != null && Random.value <= playerReliquia.bonusDefesa)
        {
            Debug.Log("Golpe bloqueado pela Defesa!");
            return;
        }

        Debug.Log("[VIDA] Antes: " + coracoesAtuais + " | Dano: " + dano);
        coracoesAtuais -= dano;
        coracoesAtuais = Mathf.Max(coracoesAtuais, 0f);
        Debug.Log("[VIDA] Depois: " + coracoesAtuais + "/" + coracoesMaximos);

        onVidaAtualizada?.Invoke();

        PlayerAnimator anim = GetComponent<PlayerAnimator>();
        if (anim != null)
            anim.TriggerDamage();

        if (coracoesAtuais <= 0)
        {
            coracoesAtuais = coracoesMaximos;
            GameManager.Instance.PerderTentativa();
            onVidaAtualizada?.Invoke();
        }
    }
}