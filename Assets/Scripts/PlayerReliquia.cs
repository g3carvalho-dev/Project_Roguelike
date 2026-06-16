using UnityEngine;

public class PlayerReliquia : MonoBehaviour
{
    public Reliquia reliquiaEquipada;

    private PlayerStats stats;
    private PlayerAttack attack;
    private PlayerController controller;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        attack = GetComponent<PlayerAttack>();
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
                if (attack != null && attack.armaAtual != null)
                {
                    attack.armaAtual.dano += reliquia.valor;
                    attack.armaAtual.danoHeavy += reliquia.valor;
                }
                break;

            case TipoBonusReliquia.Velocidade:
                if (controller != null)
                    controller.velocidade += reliquia.valor;
                break;

            case TipoBonusReliquia.VidaMaxima:
                if (stats != null)
                {
                    stats.coracoesMaximos += (int)reliquia.valor;
                    stats.coracoesAtuais += (int)reliquia.valor;
                }
                break;

            case TipoBonusReliquia.Defesa:
                Debug.Log("Defesa aumentada em " + reliquia.valor);
                break;
        }
    }

    void RemoverBonus(Reliquia reliquia)
    {
        switch (reliquia.tipo)
        {
            case TipoBonusReliquia.Dano:
                if (attack != null && attack.armaAtual != null)
                {
                    attack.armaAtual.dano -= reliquia.valor;
                    attack.armaAtual.danoHeavy -= reliquia.valor;
                }
                break;

            case TipoBonusReliquia.Velocidade:
                if (controller != null)
                    controller.velocidade -= reliquia.valor;
                break;

            case TipoBonusReliquia.VidaMaxima:
                if (stats != null)
                {
                    stats.coracoesMaximos -= (int)reliquia.valor;
                    stats.coracoesAtuais = Mathf.Min(stats.coracoesAtuais, stats.coracoesMaximos);
                }
                break;

            case TipoBonusReliquia.Defesa:
                Debug.Log("Defesa removida: " + reliquia.valor);
                break;
        }
    }
}