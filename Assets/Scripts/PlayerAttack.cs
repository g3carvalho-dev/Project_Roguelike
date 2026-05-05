using UnityEngine;

public enum TipoArma { Melee, Ranged }

[System.Serializable]
public class Arma
{
    public string nome;
    public TipoArma tipo;
    public float dano;
    public float intervaloAtaque;
    public GameObject prefabProjetil; // só usado se for Ranged
}

public class PlayerAttack : MonoBehaviour
{
    public Arma armaAtual;
    public GameObject prefabProjetil;
    private float timer;

    void Update()
    {
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
        Debug.Log("Posicao do golpe: " + posicaoGolpe);
        Debug.Log("Posicao do inimigo: " + GameObject.Find("Inimigo").transform.position);
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(posicaoGolpe, 1f);
        Debug.Log("Colliders encontrados: " + atingidos.Length);

        foreach (Collider2D col in atingidos)
        {
            Debug.Log("Collider encontrado: " + col.name);
            if (col.CompareTag("Inimigo"))
            {
                Debug.Log("Acertou " + col.name + " por " + armaAtual.dano + " de dano!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // mostra o alcance do melee no editor
        Gizmos.color = Color.red;
        Vector3 mouseMundo = Camera.main != null
            ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : transform.position + Vector3.right;
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;
        Gizmos.DrawWireSphere(transform.position + (Vector3)(direcao * 0.8f), 0.5f);
    }
}