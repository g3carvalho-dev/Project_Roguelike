using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Stats")]
    public PlayerStats playerStats;
    public Image imagemCoracoes;
    [Tooltip("Sprites de 0 a 3 corações (7 imagens: 0, 0.5, 1, 1.5, 2, 2.5, 3)")]
    public Sprite[] spritesCoracoes;
    public TextMeshProUGUI textoTentativas;

    [Header("Moedas")]
    public Image iconeMoeda;
    public TextMeshProUGUI textoMoedas;
    [Header("Animação Moeda")]
    public Sprite[] framesMoeda;
    public float velocidadeAnimacao = 10f;
    private int frameAtual = 0;
    private float timerAnimacao;

    [Header("Inventário - Slot")]
    public PlayerAttack playerAttack;
    public Image slotArma;
    public Image iconeArma;

    [Header("Retícula")]
    public RectTransform reticula;

    void Awake()
    {
        if (playerAttack == null)
            playerAttack = FindObjectOfType<PlayerAttack>();

        if (iconeArma != null)
            iconeArma.preserveAspect = true;

        Cursor.visible = false;
    }

    void Start()
    {
        if (playerAttack != null)
            playerAttack.onInventarioAtualizado += AtualizarSlots;

        if (playerStats != null)
            playerStats.onVidaAtualizada += AtualizarCoracoes;

        AtualizarSlots();
        AtualizarMoedas();
        AtualizarCoracoes();
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

        if (GameManager.Instance != null)
            textoTentativas.text = "Tentativas: " + GameManager.Instance.tentativasAtuais;

        AnimarMoeda();
        MoverReticula();
    }

    void MoverReticula()
    {
        if (reticula == null) return;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        reticula.position = mousePos;
    }

    void AnimarMoeda()
    {
        if (iconeMoeda == null || framesMoeda == null || framesMoeda.Length == 0) return;

        timerAnimacao += Time.deltaTime * velocidadeAnimacao;
        if (timerAnimacao >= 1f)
        {
            timerAnimacao -= 1f;
            frameAtual = (frameAtual + 1) % framesMoeda.Length;
            iconeMoeda.sprite = framesMoeda[frameAtual];
        }
    }

    void OnDestroy()
    {
        Cursor.visible = true;

        if (playerAttack != null)
            playerAttack.onInventarioAtualizado -= AtualizarSlots;

        if (playerStats != null)
            playerStats.onVidaAtualizada -= AtualizarCoracoes;

        if (GameManager.Instance != null)
            GameManager.Instance.onMoedasAtualizadas -= AtualizarMoedas;
    }

    void AtualizarMoedas()
    {
        if (textoMoedas == null || GameManager.Instance == null) return;
        textoMoedas.text = GameManager.Instance.moedas.ToString();
    }

    void AtualizarCoracoes()
    {
        if (playerStats == null || imagemCoracoes == null || spritesCoracoes == null) return;
        if (spritesCoracoes.Length == 0) return;

        int indice = Mathf.RoundToInt(playerStats.coracoesAtuais * 2f);
        indice = Mathf.Clamp(indice, 0, spritesCoracoes.Length - 1);
        imagemCoracoes.sprite = spritesCoracoes[indice];
    }

    void AtualizarSlots()
    {
        if (playerAttack == null) return;

        bool temArma = playerAttack.inventario.Count > 0;

        if (slotArma != null)
            slotArma.gameObject.SetActive(true);

        if (iconeArma != null)
        {
            if (temArma && playerAttack.armaAtual.sprite != null)
            {
                iconeArma.gameObject.SetActive(true);
                iconeArma.sprite = playerAttack.armaAtual.sprite;
                iconeArma.SetNativeSize();
            }
            else
            {
                iconeArma.gameObject.SetActive(false);
            }
        }
    }
}