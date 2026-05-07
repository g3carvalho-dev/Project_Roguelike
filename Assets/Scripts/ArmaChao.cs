using UnityEngine;

public class ArmaChao : MonoBehaviour
{
    public string nomeArma;
    public TipoArma tipoArma;
    public float dano = 25f;
    public float intervaloAtaque = 0.5f;

    public GameObject prefabArmaChao;
    private bool playerPerto = false;
    private PlayerAttack playerAtack;

    void Update()
    {
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

        // se já tem arma, dropa a anterior
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
            armaChaoScript.intervaloAtaque = playerAtack.armaAtual.intervaloAtaque;
            armaChaoScript.prefabArmaChao = prefabArmaChao;
        }

        // equipa a nova arma
        Arma novaArma = new Arma();
        novaArma.nome = nomeArma;
        novaArma.tipo = tipoArma;
        novaArma.dano = dano;
        novaArma.intervaloAtaque = intervaloAtaque;

        playerAtack.armaAtual = novaArma;
        Debug.Log("Equipou: " + nomeArma);
        Destroy(gameObject);
    }
}