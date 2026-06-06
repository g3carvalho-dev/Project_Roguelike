using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [Header("Painéis do Menu Manager")]
    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelJogar;
    [SerializeField] private GameObject painelSair;

    // Pilha para manter histório de painéis acessados
    private Stack<GameObject> historicoPaineis = new Stack<GameObject>();
    private GameObject currentPanel;

    void Start()
    {
        painelMenu.SetActive(true);
        if (painelOpcoes != null) painelOpcoes.SetActive(false);
        if (painelJogar != null) painelJogar.SetActive(false);
        if (painelSair != null) painelSair.SetActive(false);

        currentPanel = painelMenu;
    }

    public void BotaoJogar()
    {
        Debug.Log("Acessando painel de saves");
        NavegarPara(painelJogar);
    }

    public void BotaoOpcoes()
    {
        Debug.Log("Acessando menu de opções");
        NavegarPara(painelOpcoes);
    }

    public void BotaoSair()
    {
        Debug.Log("Acessando modal de confirmação de saída");
        NavegarPara(painelSair);
    }

    public void IniciarJogo(int slot)
    {
        PlayerPrefs.SetInt("SlotAtual", slot);
        SceneManager.LoadScene("SampleScene");
    }

    private void NavegarPara(GameObject proximoPainel)
    {
        if (proximoPainel == null) return;

        if (currentPanel != null)
        {
            historicoPaineis.Push(currentPanel); // Guarda o painel atual no histórico
            currentPanel.SetActive(false);       // Desativa o painel atual
        }

        currentPanel = proximoPainel;
        currentPanel.SetActive(true);            // Ativa o novo painel
    }

    public void BotaoVoltar()
    {
        if (historicoPaineis.Count > 0)
        {
            if (currentPanel != null)
            {
                currentPanel.SetActive(false);   
            }

            currentPanel = historicoPaineis.Pop();
            currentPanel.SetActive(true);          
        }
    }

    public void BotaoConfirmarSaida()
    {
        Debug.Log("Saindo do jogo");
        Application.Quit();
    }

}