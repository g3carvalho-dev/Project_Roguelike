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

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.salaAtual > 1)
            Destroy(gameObject);
    }

    void Update()
    {
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
            TrocarComArmaAtiva();
            Debug.Log("Inventario cheio! Maximo de " + playerAttack.capacidadeMaxima + " armas.");
            return;
        }

        ColetarArma();
    }

    void TrocarComArmaAtiva()
    {
        Arma armaAtiva = playerAttack.armaAtual;
        GameObject prefab = playerAttack.GetPrefabArma(armaAtiva.nome);

        if (armaAtiva == null || prefab == null)
        {
            Debug.Log("[ARMA] Arma ativa sem prefab de chao, nao e possivel dropar.");
            return;
        }

        Vector3 posicaoDrop = transform.position + new Vector3(0.5f, 0, 0);
        GameObject armaDropada = Instantiate(prefab, posicaoDrop, Quaternion.identity);

        ArmaChao armaChaoScript = armaDropada.GetComponent<ArmaChao>();
        armaChaoScript.nomeArma = armaAtiva.nome;
        armaChaoScript.tipoArma = armaAtiva.tipo;
        armaChaoScript.dano = armaAtiva.dano;
        armaChaoScript.danoHeavy = armaAtiva.danoHeavy;
        armaChaoScript.intervaloAtaque = armaAtiva.intervaloAtaque;
        armaChaoScript.intervaloAtaqueHeavy = armaAtiva.intervaloAtaqueHeavy;
        armaChaoScript.prefabArmaChao = prefab;

        if (armaAtiva.sprite != null)
        {
            SpriteRenderer sr = armaDropada.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = armaAtiva.sprite;
        }

        Debug.Log($"[ARMA] Dropou: {armaAtiva.nome} no chao");

        playerAttack.RemoverArmaAtiva();
        ColetarArma();
    }

    void ColetarArma()
    {
        Arma novaArma = new Arma();
        novaArma.nome = nomeArma;
        novaArma.tipo = tipoArma;
        novaArma.dano = dano;
        novaArma.danoHeavy = danoHeavy;
        novaArma.intervaloAtaque = intervaloAtaque;
        novaArma.intervaloAtaqueHeavy = intervaloAtaqueHeavy;
        novaArma.prefabArmaChao = prefabArmaChao;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            novaArma.sprite = sr.sprite;

        bool coletou = playerAttack.AdicionarArma(novaArma);

        if (coletou)
        {
            Destroy(gameObject);

            if (GameManager.Instance != null && GameManager.Instance.voltandoDeMorte)
            {
                GameManager.Instance.podeTrocarArma = false;
                GameManager.Instance.VoltarParaCheckpoint();
            }
        }
    }
}