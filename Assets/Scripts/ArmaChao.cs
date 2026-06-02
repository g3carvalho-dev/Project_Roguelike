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
    private PlayerAttack playerAtack;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.salaAtual > 1)
            Destroy(gameObject);
    }

    void Update()
    {
        if (this == null || gameObject == null) return;
        if (playerPerto && Input.GetKeyDown(KeyCode.C))
            Equipar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
            playerAtack = other.GetComponent<PlayerAttack>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
            playerAtack = null;
        }
    }

    void Equipar()
    {
        if (playerAtack == null) return;

        if (playerAtack.armaAtual != null)
        {
            GameObject armaDropada = Instantiate(
                prefabArmaChao,
                transform.position + new Vector3(0.5f, 0, 0),
                Quaternion.identity
            );
            ArmaChao armaChaoScript = armaDropada.GetComponent<ArmaChao>();
            armaChaoScript.nomeArma = playerAtack.armaAtual.nome;
            armaChaoScript.tipoArma = playerAtack.armaAtual.tipo;
            armaChaoScript.dano = playerAtack.armaAtual.dano;
            armaChaoScript.danoHeavy = playerAtack.armaAtual.danoHeavy;
            armaChaoScript.intervaloAtaque = playerAtack.armaAtual.intervaloAtaque;
            armaChaoScript.intervaloAtaqueHeavy = playerAtack.armaAtual.intervaloAtaqueHeavy;
            armaChaoScript.prefabArmaChao = prefabArmaChao;
        }

        Arma novaArma = new Arma();
        novaArma.nome = nomeArma;
        novaArma.tipo = tipoArma;
        novaArma.dano = dano;
        novaArma.danoHeavy = danoHeavy;
        novaArma.intervaloAtaque = intervaloAtaque;
        novaArma.intervaloAtaqueHeavy = intervaloAtaqueHeavy;

        playerAtack.armaAtual = novaArma;
        Debug.Log("Equipou: " + nomeArma);
        Destroy(gameObject);
    }
}