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

        // se o inv tá cheio, ele pega a arma e dropa a ativa.
        if (playerAttack.InventarioCheio())
        {
            Debug.Log("Inventario cheio! Maximo de " + playerAttack.capacidadeMaxima + " armas.");
            return;
        }

        // vai pegar arma.
        ColetarArma();
    }

    void TrocarComArmaAtiva()
    {
        Arma armaAtiva = playerAttack.armaAtual;

        if (armaAtiva == null || armaAtiva.prefabArmaChao == null)
        {
            Debug.Log("[ARMA] Arma ativa sem prefab de chão, não é possível dropar.");
            return;
        }

        // Instancia a arma ativa no chão
        Vector3 posicaoDrop = transform.position + new Vector3(0.5f, 0, 0);
        GameObject armaDropada = Instantiate(armaAtiva.prefabArmaChao, posicaoDrop, Quaternion.identity);

        // Configura os dados da arma dropada
        ArmaChao armaChaoScript = armaDropada.GetComponent<ArmaChao>();
        armaChaoScript.nomeArma               = armaAtiva.nome;
        armaChaoScript.tipoArma               = armaAtiva.tipo;
        armaChaoScript.dano                   = armaAtiva.dano;
        armaChaoScript.danoHeavy              = armaAtiva.danoHeavy;
        armaChaoScript.intervaloAtaque        = armaAtiva.intervaloAtaque;
        armaChaoScript.intervaloAtaqueHeavy   = armaAtiva.intervaloAtaqueHeavy;
        armaChaoScript.prefabArmaChao         = armaAtiva.prefabArmaChao;

        Debug.Log($"[ARMA] Dropou: {armaAtiva.nome} no chão | Slot {playerAttack.indiceAtual + 1}");

        
        playerAttack.RemoverArmaAtiva();
        ColetarArma();
    }

    void ColetarArma()  {//aqui tá sobre a arma que vai ser coletada.
        Arma novaArma = new Arma();
        novaArma.nome = nomeArma;
        novaArma.tipo = tipoArma;
        novaArma.dano = dano;
        novaArma.danoHeavy = danoHeavy;
        novaArma.intervaloAtaque = intervaloAtaque;
        novaArma.intervaloAtaqueHeavy = intervaloAtaqueHeavy;
        novaArma.prefabArmaChao = prefabArmaChao;

        bool coletou = playerAttack.AdicionarArma(novaArma);
                if(coletou)
                    Destroy(gameObject);
    }
}