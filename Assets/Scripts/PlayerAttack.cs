using UnityEngine;
using System.Collections.Generic;

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
    public GameObject prefabArmaChao; // referência para dropar depois (opcional)
}

public class PlayerAttack : MonoBehaviour
{
    [Header("Inventário")]
    public List<Arma> inventario = new List<Arma>();
    public int indiceAtual = 0;
    public int capacidadeMaxima = 4;

    // Propriedade para manter compatibilidade com ArmaChao
    public Arma armaAtual
    {
        get => inventario.Count > 0 ? inventario[indiceAtual] : null;
        set
        {
            // Mantido para compatibilidade, mas use AdicionarArma()
            if (value != null) AdicionarArma(value);
        }
    }

    public GameObject prefabProjetil;

    private float timerLight;
    private float timerHeavy;

    // Evento chamado sempre que o inventário muda (para a UI escutar)
    public System.Action onInventarioAtualizado;

    void Update()
    {
        LidarComScroll();

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

    void LidarComScroll()
    {
        if (inventario.Count <= 1) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            MudarArma(-1); // scroll pra cima = anterior
        else if (scroll < 0f)
            MudarArma(1);  // scroll pra baixo = próxima

        // Teclas numéricas 1-4 para acesso direto
        for (int i = 0; i < Mathf.Min(inventario.Count, 4); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelecionarArma(i);
        }
    }

    void MudarArma(int direcao)
    {
        indiceAtual = (indiceAtual + direcao + inventario.Count) % inventario.Count;
        onInventarioAtualizado?.Invoke();
        Debug.Log($"Arma selecionada: {armaAtual.nome} [{indiceAtual + 1}/{inventario.Count}]");
    }

    public void SelecionarArma(int indice)
    {
        if (indice < 0 || indice >= inventario.Count) return;
        indiceAtual = indice;
        onInventarioAtualizado?.Invoke();
    }

    public bool AdicionarArma(Arma novaArma)
    {
        if (inventario.Count >= capacidadeMaxima)
        {
            Debug.Log("Inventário cheio!");
            return false;
        }

        inventario.Add(novaArma);
        indiceAtual = inventario.Count - 1; // seleciona a recém coletada
        onInventarioAtualizado?.Invoke();
        Debug.Log($"Coletou: {novaArma.nome} [{inventario.Count}/{capacidadeMaxima}]");
        return true;
    }

    public bool InventarioCheio() => inventario.Count >= capacidadeMaxima;

    // ── Ataque ──────────────────────────────────────────────────────────────

    void Atacar(bool pesado)
    {
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;

        float danoAtual = pesado ? armaAtual.danoHeavy : armaAtual.dano;
        float alcance   = pesado ? 1.8f : 1.2f;
        float area      = pesado ? 1.5f : 1f;

        if (armaAtual.tipo == TipoArma.Ranged)
            AtaqueRanged(direcao);
        else
            AtaqueMelee(direcao, danoAtual, alcance, area);
    }

    void AtaqueRanged(Vector2 direcao)
    {
        GameObject projetil = Instantiate(prefabProjetil, transform.position, Quaternion.identity);
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
        if (Camera.main == null) return;
        Gizmos.color = Color.red;
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;
        Gizmos.DrawWireSphere(transform.position + (Vector3)(direcao * 0.8f), 0.5f);
    }
}