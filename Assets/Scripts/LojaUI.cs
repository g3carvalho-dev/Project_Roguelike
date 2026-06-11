using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LojaUI : MonoBehaviour
{
    public GameObject painelLoja;

    [Header("Slots")]
    public TextMeshProUGUI[] nomesItens;
    public TextMeshProUGUI[] precosItens;
    public Button[] botoesComprar;

    void Start()
    {
        painelLoja.SetActive(false);
    }

    public void AbrirLoja(System.Collections.Generic.List<ItemLoja> itens)
    {
        painelLoja.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo

        for (int i = 0; i < nomesItens.Length; i++)
        {
            if (i < itens.Count)
            {
                nomesItens[i].text = itens[i].nome;
                precosItens[i].text = itens[i].preco + " moedas";
                botoesComprar[i].gameObject.SetActive(true);

                int index = i;
                botoesComprar[i].onClick.RemoveAllListeners();
                botoesComprar[i].onClick.AddListener(() => Comprar(index));
            }
            else
            {
                nomesItens[i].text = "";
                precosItens[i].text = "";
                botoesComprar[i].gameObject.SetActive(false);
            }
        }
    }

    void Comprar(int index)
    {
        LojaManager.Instance.ComprarItem(index);
        FecharLoja();
    }

    public void FecharLoja()
    {
        painelLoja.SetActive(false);
        Time.timeScale = 1f;
    }
}