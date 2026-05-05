using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject painelMenu;
    public GameObject painelOpcoes;
    public GameObject painelJogar;

    void Start()
    {
        painelMenu.SetActive(true);
        painelOpcoes.SetActive(false);
        painelJogar.SetActive(false);
    }

    public void BotaoJogar()
    {
        Debug.Log("Botao Jogar clicado!");
        painelMenu.SetActive(false);
        painelJogar.SetActive(true);
    }

    public void BotaoOpcoes()
    {
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
        Application.Quit();
    }

    public void IniciarJogo(int slot)
    {
        PlayerPrefs.SetInt("SlotAtual", slot);
        SceneManager.LoadScene("SampleScene");
    }
}