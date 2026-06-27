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
    public GameObject prefabArmaChao;
    public Sprite sprite;
}

public class PlayerAttack : MonoBehaviour
{
    [Header("Inventário")]
    public List<Arma> inventario = new List<Arma>();
    public int indiceAtual = 0;
    public int capacidadeMaxima = 2;

    public Arma armaAtual => inventario.Count > 0 ? inventario[indiceAtual] : null;

    public GameObject prefabProjetil;

    [Header("Prefabs das Armas")]
    public GameObject prefabArmaEspada;
    public GameObject prefabArmaMachado;
    public GameObject prefabArmaLanca;
    public GameObject prefabArmaArco;

    private float timerLight;
    private float timerHeavy;
    private float scrollCooldown = 0f;

    private PlayerReliquia playerReliquia;

    public System.Action onInventarioAtualizado;

    void Start()
    {
        playerReliquia = GetComponent<PlayerReliquia>();
    }

    public GameObject GetPrefabArma(string nomeArma)
    {
        string caminho = "";
        switch (nomeArma)
        {
            case "Espada":  caminho = "ArmaEspada";  break;
            case "Machado": caminho = "ArmaMachado"; break;
            case "Lanca":   caminho = "ArmaLanca";   break;
            case "Arco":    caminho = "ArmaArco";    break;
            default:
                Debug.LogError("Prefab nao encontrado para: " + nomeArma);
                return null;
        }

        GameObject prefab = Resources.Load<GameObject>(caminho);
        if (prefab == null)
            Debug.LogError("Resources.Load falhou para: " + caminho);

        return prefab;
    }

    void Update()
    {
        LidarComTroca();

        if (armaAtual == null) return;

        timerLight -= Time.deltaTime;
        timerHeavy -= Time.deltaTime;
        scrollCooldown -= Time.deltaTime;

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

        if (Input.GetKeyDown(KeyCode.O))
            DroparArmaAtiva();
    }

    public void DroparArmaAtiva()
    {
        if (armaAtual == null) return;

        GameObject prefab = GetPrefabArma(armaAtual.nome);
        if (prefab == null)
        {
            Debug.Log("Sem prefab para dropar: " + armaAtual.nome);
            return;
        }

        Vector3 posicaoDrop = transform.position + new Vector3(1f, 0, 0);
        GameObject armaDropada = Instantiate(prefab, posicaoDrop, Quaternion.identity);

        ArmaChao armaChaoScript = armaDropada.GetComponent<ArmaChao>();
        armaChaoScript.nomeArma = armaAtual.nome;
        armaChaoScript.tipoArma = armaAtual.tipo;
        armaChaoScript.dano = armaAtual.dano;
        armaChaoScript.danoHeavy = armaAtual.danoHeavy;
        armaChaoScript.intervaloAtaque = armaAtual.intervaloAtaque;
        armaChaoScript.intervaloAtaqueHeavy = armaAtual.intervaloAtaqueHeavy;
        armaChaoScript.prefabArmaChao = prefab;

        if (armaAtual.sprite != null)
        {
            SpriteRenderer sr = armaDropada.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = armaAtual.sprite;
        }

        Debug.Log("Dropou: " + armaAtual.nome);
        RemoverArmaAtiva();
    }

    void LidarComTroca()
    {
        if (inventario.Count <= 1) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scrollCooldown <= 0f)
        {
            if (scroll > 0.05f)
            {
                Alternar();
                scrollCooldown = 0.2f;
            }
            else if (scroll < -0.05f)
            {
                Alternar();
                scrollCooldown = 0.2f;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
            Alternar();

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelecionarArma(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelecionarArma(1);
    }

    void Alternar()
    {
        int proximo = (indiceAtual + 1) % inventario.Count;
        SelecionarArma(proximo);
    }

    public void SelecionarArma(int indice)
    {
        if (indice < 0 || indice >= inventario.Count) return;
        if (indice == indiceAtual) return;
        indiceAtual = indice;
        onInventarioAtualizado?.Invoke();
        Debug.Log($"[ARMA] Slot {indice + 1} → {armaAtual.nome}");
    }

    public bool AdicionarArma(Arma novaArma)
    {
        if (inventario.Count >= capacidadeMaxima)
        {
            Debug.Log($"[ARMA] Inventário cheio!");
            return false;
        }
        inventario.Add(novaArma);
        indiceAtual = inventario.Count - 1;
        onInventarioAtualizado?.Invoke();
        Debug.Log($"[ARMA] Coletou: {novaArma.nome} → Slot {indiceAtual + 1}/{capacidadeMaxima}");
        return true;
    }

    public void RemoverArmaAtiva()
    {
        if (inventario.Count == 0) return;
        string nomeRemovida = armaAtual.nome;
        inventario.RemoveAt(indiceAtual);
        indiceAtual = Mathf.Clamp(indiceAtual, 0, Mathf.Max(0, inventario.Count - 1));
        onInventarioAtualizado?.Invoke();
        Debug.Log($"[ARMA] Removeu: {nomeRemovida}");
    }

    public bool InventarioCheio() => inventario.Count >= capacidadeMaxima;

    void Atacar(bool pesado)
    {
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;

        float danoBase  = pesado ? armaAtual.danoHeavy : armaAtual.dano;
        float danoAtual = playerReliquia != null ? playerReliquia.AplicarBonusDano(danoBase) : danoBase;

        float alcance = pesado ? 1.8f : 1.2f;
        float area    = pesado ? 1.5f : 1f;

        if (armaAtual.tipo == TipoArma.Ranged)
            AtaqueRanged(direcao, danoAtual);
        else
            AtaqueMelee(direcao, danoAtual, alcance, area);
    }

    void AtaqueRanged(Vector2 direcao, float dano)
    {
        GameObject projetil = Instantiate(prefabProjetil, transform.position, Quaternion.identity);
        Projetil p = projetil.GetComponent<Projetil>();
        p.Iniciar(direcao);
        p.dano = dano;
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