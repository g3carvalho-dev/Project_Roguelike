using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI textoCoracoes;
    public TextMeshProUGUI textoTentativas;

    void Update()
    {
        if (playerStats == null) return;

        textoCoracoes.text = "Coracoes: " + playerStats.coracoesAtuais + "/" + playerStats.coracoesMaximos;
        textoTentativas.text = "Tentativas: " + playerStats.tentativasAtuais;
    }
}