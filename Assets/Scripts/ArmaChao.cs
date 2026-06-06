using UnityEngine;

public class ArmaChao : MonoBehaviour
{
    public string nomeArma;
    public TipoArma tipoArma;
    public float dano = 25f;
    public float danoHeavy = 50f;
    public float intervaloAtaque = 0.5f;
    public float intervaloAtaqueHeavy = 1f;
    public GameObject prefabArmaChao;

    private bool playerPerto = false;
    private PlayerAttack playerAttack;

    void Update()
    {
        if (this == null || gameObject == null) return;
        if (playerPerto && Input.GetKeyDown(KeyCode.C))
            TentarColetar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
            playerAttack = other.GetComponent<PlayerAttack>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
            playerAttack = null;
        }
    }

    void TentarColetar()
    {
        if (playerAttack == null) return;

        if (playerAttack.InventarioCheio())
        {
            Debug.Log("Inventário cheio! Máximo de " + playerAttack.capacidadeMaxima + " armas.");
            return;
        }

        Arma novaArma = new Arma();
        novaArma.nome              = nomeArma;
        novaArma.tipo              = tipoArma;
        novaArma.dano              = dano;
        novaArma.danoHeavy         = danoHeavy;
        novaArma.intervaloAtaque   = intervaloAtaque;
        novaArma.intervaloAtaqueHeavy = intervaloAtaqueHeavy;
        novaArma.prefabArmaChao    = prefabArmaChao;

        bool coletou = playerAttack.AdicionarArma(novaArma);
        if (coletou)
            Destroy(gameObject);
    }
}