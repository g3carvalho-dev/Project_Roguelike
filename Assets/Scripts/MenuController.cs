using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject painelMenu;
    public GameObject painelOpcoes;
    public GameObject painelJogar;
    public GameObject painelSair;

    void Start()
    {
        painelMenu.SetActive(true);
        painelOpcoes.SetActive(false);
        painelJogar.SetActive(false);
    }

    public void BotaoJogar()
    {
        Debug.Log("Acessando painel de saves");
        painelMenu.SetActive(false);
        painelJogar.SetActive(true);
    }

    public void BotaoOpcoes()
    {
        Debug.Log("Acessando menu de opções");
        painelMenu.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void BotaoVoltar()
    {
        painelMenu.SetActive(true);
        painelOpcoes.SetActive(false);
        painelJogar.SetActive(false);
    }

    public void BotaoSair()
    {
        Debug.Log("Acessando modal de confirmação de saída");
        painelMenu.SetActive(false);
        painelSair.SetActive(true);
    }

    public void BotaoConfirmarSaida()
    {
        Debug.Log("Saindo do jogo");
        Application.Quit();
    }

    public void BotaoCancelarSaida()
    {
        Debug.Log("Cancelando saída do jogo");
        painelSair.SetActive(false);
        painelMenu.SetActive(true);
    }

    public void IniciarJogo(int slot)
    {
        PlayerPrefs.SetInt("SlotAtual", slot);
        SceneManager.LoadScene("SampleScene");
    }
}