using UnityEngine;

public enum TipoArma { Melee, Ranged }

[System.Serializable]
public class Arma
{
    public string nome;
    public TipoArma tipo;
    public float dano;
    public float danoHeavy;
    public float intervaloAtaque;
    public float intervaloAtaqueHeavy;
    public GameObject prefabProjetil;
}

public class PlayerAttack : MonoBehaviour
{
    public Arma armaAtual;
    public GameObject prefabProjetil;
    private float timerLight;
    private float timerHeavy;

    void Start()
    {
        armaAtual = null;
    }

    void Update()
    {
        if (armaAtual == null) return;

        timerLight -= Time.deltaTime;
        timerHeavy -= Time.deltaTime;

        if (Input.GetMouseButton(0) && timerLight <= 0)
        {
            Atacar(false);
            timerLight = armaAtual.intervaloAtaque;
        }

        if (Input.GetMouseButton(1) && timerHeavy <= 0)
        {
            Atacar(true);
            timerHeavy = armaAtual.intervaloAtaqueHeavy;
        }
    }

    void Atacar(bool pesado)
    {
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;

        float danoAtual = pesado ? armaAtual.danoHeavy : armaAtual.dano;
        float alcance = pesado ? 1.8f : 1.2f;
        float area = pesado ? 1.5f : 1f;

        if (armaAtual.tipo == TipoArma.Ranged)
            AtaqueRanged(direcao);
        else
            AtaqueMelee(direcao, danoAtual, alcance, area);
    }

    void AtaqueRanged(Vector2 direcao)
    {
        GameObject projetil = Instantiate(
            prefabProjetil,
            transform.position,
            Quaternion.identity
        );
        projetil.GetComponent<Projetil>().Iniciar(direcao);
    }

    void AtaqueMelee(Vector2 direcao, float dano, float alcance, float area)
    {
        Vector2 posicaoGolpe = (Vector2)transform.position + direcao * alcance;
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(posicaoGolpe, area);

        foreach (Collider2D col in atingidos)
        {
            if (col.CompareTag("Inimigo"))
            {
                EnemyStats stats = col.GetComponent<EnemyStats>();
                if (stats != null)
                    stats.ReceberDano(dano);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 mouseMundo = Camera.main != null
            ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : transform.position + Vector3.right;
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;
        Gizmos.DrawWireSphere(transform.position + (Vector3)(direcao * 0.8f), 0.5f);
    }
}