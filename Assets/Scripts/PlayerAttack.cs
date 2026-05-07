using UnityEngine;

public enum TipoArma { Melee, Ranged }

[System.Serializable]
public class Arma
{
    public string nome;
    public TipoArma tipo;
    public float dano;
    public float intervaloAtaque;
    public GameObject prefabProjetil;
}

public class PlayerAttack : MonoBehaviour
{
    public Arma armaAtual;
    public GameObject prefabProjetil;
    private float timer;

    void Start()
    {
        armaAtual = null;
    }

    void Update()
    {
        if (armaAtual == null) return;

        timer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && timer <= 0)
        {
            Atacar();
            timer = armaAtual.intervaloAtaque;
        }
    }

    void Atacar()
    {
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;

        if (armaAtual.tipo == TipoArma.Ranged)
            AtaqueRanged(direcao);
        else
            AtaqueMelee(direcao);
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

    void AtaqueMelee(Vector2 direcao)
    {
        Vector2 posicaoGolpe = (Vector2)transform.position + direcao * 1.2f;
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(posicaoGolpe, 1f);

        foreach (Collider2D col in atingidos)
        {
            if (col.CompareTag("Inimigo"))
                Debug.Log("Acertou " + col.name + " por " + armaAtual.dano + " de dano!");
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