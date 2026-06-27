using UnityEngine;

public class PlayerReliquia : MonoBehaviour
{
    public Reliquia reliquiaEquipada;

    [Header("Bônus acumulados (aplicados em qualquer arma)")]
    public float bonusDano = 0f;
    public float bonusVelocidade = 0f;
    public float bonusDefesa = 0f; // % de redução de dano (0 a 1)

    private PlayerStats stats;
    private PlayerAttack attack;
    private PlayerController controller;

    void Start()
    {
        stats      = GetComponent<PlayerStats>();
        attack     = GetComponent<PlayerAttack>();
        controller = GetComponent<PlayerController>();
    }

    public bool TemReliquia()
    {
        return reliquiaEquipada != null && !string.IsNullOrEmpty(reliquiaEquipada.nome);
    }

    public void EquiparReliquia(Reliquia novaReliquia)
    {
        if (TemReliquia())
            RemoverBonus(reliquiaEquipada);

        reliquiaEquipada = novaReliquia;
        AplicarBonus(reliquiaEquipada);

        Debug.Log("Reliquia equipada: " + novaReliquia.nome);
    }

    void AplicarBonus(Reliquia reliquia)
    {
        switch (reliquia.tipo)
        {
            case TipoBonusReliquia.Dano:
                bonusDano += reliquia.valor;
                break;

            case TipoBonusReliquia.Velocidade:
                bonusVelocidade += reliquia.valor;
                if (controller != null)
                    controller.velocidade += reliquia.valor;
                break;

            case TipoBonusReliquia.VidaMaxima:
                if (stats != null)
                {
                    stats.coracoesMaximos += reliquia.valor;
                    stats.coracoesAtuais += reliquia.valor;
                }
                break;

            case TipoBonusReliquia.Defesa:
                bonusDefesa += reliquia.valor;
                Debug.Log("Defesa total: " + (bonusDefesa * 100f) + "%");
                break;
        }
    }

    void RemoverBonus(Reliquia reliquia)
    {
        switch (reliquia.tipo)
        {
            case TipoBonusReliquia.Dano:
                bonusDano -= reliquia.valor;
                break;

            case TipoBonusReliquia.Velocidade:
                bonusVelocidade -= reliquia.valor;
                if (controller != null)
                    controller.velocidade -= reliquia.valor;
                break;

            case TipoBonusReliquia.VidaMaxima:
                if (stats != null)
                {
                    stats.coracoesMaximos -= reliquia.valor;
                    stats.coracoesAtuais = Mathf.Min(stats.coracoesAtuais, stats.coracoesMaximos);
                }
                break;

            case TipoBonusReliquia.Defesa:
                bonusDefesa -= reliquia.valor;
                break;
        }
    }

    // Chamado pelo PlayerAttack para aplicar o bônus de dano na hora do golpe
    public float AplicarBonusDano(float danoBase) => danoBase + bonusDano;

    // Chamado pelo PlayerStats para reduzir dano recebido
    public float AplicarDefesa(float danoRecebido) => danoRecebido * (1f - Mathf.Clamp01(bonusDefesa));
}