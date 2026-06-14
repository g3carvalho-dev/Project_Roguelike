using UnityEngine;

public class ReliquiaChao : MonoBehaviour
{
    public string nomeReliquia;
    public string descricao;
    public TipoBonusReliquia tipoBonus;
    public float valorBonus;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerReliquia playerReliquia = other.GetComponent<PlayerReliquia>();
            if (playerReliquia != null)
            {
                Reliquia novaReliquia = new Reliquia();
                novaReliquia.nome = nomeReliquia;
                novaReliquia.descricao = descricao;
                novaReliquia.tipo = tipoBonus;
                novaReliquia.valor = valorBonus;

                playerReliquia.EquiparReliquia(novaReliquia);
                Destroy(gameObject);
            }
        }
    }
}