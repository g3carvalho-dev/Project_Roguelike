using UnityEngine;
using System.Collections.Generic;

public class LojaManager : MonoBehaviour
{
    public static LojaManager Instance;

    [Header("Itens disponiveis na loja")]
    public List<ItemLoja> todosItens = new List<ItemLoja>();

    private List<ItemLoja> itensAtuais = new List<ItemLoja>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AbrirLoja()
    {
        itensAtuais.Clear();
        List<ItemLoja> disponiveis = new List<ItemLoja>(todosItens);

        for (int i = 0; i < 4 && disponiveis.Count > 0; i++)
        {
            int index = Random.Range(0, disponiveis.Count);
            itensAtuais.Add(disponiveis[index]);
            disponiveis.RemoveAt(index);
        }

        Debug.Log("Loja aberta com " + itensAtuais.Count + " itens!");

        LojaUI lojaUI = FindObjectOfType<LojaUI>();
        if (lojaUI != null)
            lojaUI.AbrirLoja(itensAtuais);
    }

    public void ComprarItem(int index)
    {
        if (index >= itensAtuais.Count) return;

        ItemLoja item = itensAtuais[index];

        if (GameManager.Instance.moedas < item.preco)
        {
            Debug.Log("Dinheiro insuficiente!");
            return;
        }

        GameManager.Instance.moedas -= item.preco;
        GameManager.Instance.onMoedasAtualizadas?.Invoke();
        AplicarBonus(item);
        itensAtuais.RemoveAt(index);
        Debug.Log("Comprou: " + item.nome);
    }

    void AplicarBonus(ItemLoja item)
    {
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        PlayerAttack attack = FindObjectOfType<PlayerAttack>();

        switch (item.tipoBonus)
        {
            case TipoBonus.Vida:
                if (stats != null)
                {
                    stats.coracoesAtuais = Mathf.Min(
                        stats.coracoesAtuais + (int)item.valorBonus,
                        stats.coracoesMaximos
                    );
                    Debug.Log("Vida restaurada!");
                }
                break;

            case TipoBonus.Dano:
                if (attack != null && attack.armaAtual != null)
                {
                    attack.armaAtual.dano += item.valorBonus;
                    Debug.Log("Dano aumentado!");
                }
                break;

            case TipoBonus.Velocidade:
                PlayerController controller = FindObjectOfType<PlayerController>();
                if (controller != null)
                {
                    controller.velocidade += item.valorBonus;
                    Debug.Log("Velocidade aumentada!");
                }
                break;
        }
    }
}

[System.Serializable]
public class ItemLoja
{
    public string nome;
    public TipoBonus tipoBonus;
    public float valorBonus;
    public int preco;
}
