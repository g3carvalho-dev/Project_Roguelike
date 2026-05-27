using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public Slider barraVida;
    public PlayerStats playerStats;
    public TextMeshProUGUI textoTentativas;

    void Update()
    {
        if (playerStats == null) return;

        barraVida.value = playerStats.vidaAtual;
        barraVida.maxValue = playerStats.vidaMaxima;
        textoTentativas.text = "Tentativas: " + playerStats.tentativasAtuais;
    }
}