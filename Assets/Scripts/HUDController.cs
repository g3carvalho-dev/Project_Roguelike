using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStats playerStats;
    public TextMeshProUGUI textoCoracoes;
    public TextMeshProUGUI textoTentativas;

    [Header("Moedas")]
    public TextMeshProUGUI textoMoedas;

    [Header("Inventário - Slots")]
    public PlayerAttack playerAttack;
    public Image[] slotsBorda;
    public Color corSlotAtivo   = new Color(1f, 0.85f, 0.2f);
    public Color corSlotInativo = new Color(0.4f, 0.4f, 0.4f);

    void Awake()
    {
        if (playerAttack == null)
            playerAttack = FindObjectOfType<PlayerAttack>();
    }

    void Start()
    {
        if (playerAttack != null)
            playerAttack.onInventarioAtualizado += AtualizarSlots;

        AtualizarSlots();
        AtualizarMoedas();
    }

    private bool inscrito = false;
    void Update()
    {
        if (!inscrito && GameManager.Instance != null)
        {
            GameManager.Instance.onMoedasAtualizadas += AtualizarMoedas;
            inscrito = true;
            AtualizarMoedas();
        }

        // Corações do PlayerStats
        if (playerStats != null)
            textoCoracoes.text = "Coracoes: " + playerStats.coracoesAtuais + "/" + playerStats.coracoesMaximos;

        // Tentativas agora vêm do GameManager
        if (GameManager.Instance != null)
            textoTentativas.text = "Tentativas: " + GameManager.Instance.tentativasAtuais;
    }

    void OnDestroy()
    {
        if (playerAttack != null)
            playerAttack.onInventarioAtualizado -= AtualizarSlots;

        if (GameManager.Instance != null)
            GameManager.Instance.onMoedasAtualizadas -= AtualizarMoedas;
    }

    void AtualizarMoedas()
    {
        if (textoMoedas == null || GameManager.Instance == null) return;
        textoMoedas.text = "Moedas: " + GameManager.Instance.moedas;
    }

    void AtualizarSlots()
    {
        if (playerAttack == null || slotsBorda == null) return;

        for (int i = 0; i < slotsBorda.Length; i++)
        {
            if (slotsBorda[i] == null) continue;
            bool temArma = i < playerAttack.inventario.Count;
            bool isAtivo = temArma && i == playerAttack.indiceAtual;
            slotsBorda[i].color = isAtivo ? corSlotAtivo : corSlotInativo;
        }
    }
}