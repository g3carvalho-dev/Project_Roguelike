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

    private bool lojaAberta = false;

    void Start()
    {
        painelLoja.SetActive(false);
    }

    void Update()
    {
        if (lojaAberta && Input.GetKeyDown(KeyCode.Escape))
            FecharLoja();
    }

    public void AbrirLoja(System.Collections.Generic.List<ItemLoja> itens)
    {
        Debug.Log("LojaUI.AbrirLoja chamada com " + itens.Count + " itens");
        painelLoja.SetActive(true);
        lojaAberta = true;

        if (GameManager.Instance != null)
            GameManager.Instance.jogoPausado = true;

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
                Debug.Log("Listener adicionado no botao " + i + " para item: " + itens[i].nome);
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
        Debug.Log("Botao clicado! Index: " + index);

        if (LojaManager.Instance == null)
        {
            Debug.LogError("LojaManager.Instance é null!");
            return;
        }

        LojaManager.Instance.ComprarItem(index);
        FecharLoja();
    }

    public void FecharLoja()
    {
        painelLoja.SetActive(false);
        lojaAberta = false;

        if (GameManager.Instance != null)
            GameManager.Instance.jogoPausado = false;
    }
}