using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    public float vidaMaxima = 50f;
    public float vidaAtual;
    public bool isMiniChefe = false;

    [Header("Drop de Moedas")]
    public GameObject prefabMoeda;
    public int moedasMin = 1;
    public int moedasMax = 3;

    [Header("Drop de Reliquia")]
    public GameObject prefabReliquiaChao;
    [Range(0f, 1f)] public float chanceDropReliquia = 0.1f;

    [Header("Animacao")]
    // Ajuste esse valor para a duracao do seu clipe InimigoDeath
    public float tempoAnimacaoMorte = 0.5f;

    // public para EnemyAI checar e parar de se mover ao morrer
    [HideInInspector] public bool morreu = false;

    private InimigosSpawner spawner;
    private Animator animator;

    private static readonly int HashTakeHit = Animator.StringToHash("takeHit");
    private static readonly int HashDeath   = Animator.StringToHash("death");

    void Start()
    {
        vidaAtual = vidaMaxima;
        spawner   = FindObjectOfType<InimigosSpawner>();
        animator  = GetComponent<Animator>();
    }

    public void ReceberDano(float dano)
    {
        if (morreu) return;

        vidaAtual -= dano;
        Debug.Log(gameObject.name + " tomou " + dano + " de dano. Vida: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // Só toca takeHit se não morreu
            if (animator != null)
                animator.SetTrigger(HashTakeHit);
        }
    }

    void Morrer()
    {
        if (morreu) return;
        morreu = true;

        // Para o movimento imediatamente
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        DropMoedas();
        DropReliquia();

        if (!isMiniChefe)
        {
            if (spawner != null)
                spawner.InimigoDerrotado();

            // Toca animação de morte e destrói após ela terminar
            if (animator != null)
            {
                animator.SetTrigger(HashDeath);
                Destroy(gameObject, tempoAnimacaoMorte);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Mini chefe: sem animação de morte, destroy imediato
            // para não quebrar o fluxo da loja
            if (GameManager.Instance != null)
                GameManager.Instance.MiniChefeDerrotado();

            Destroy(gameObject);
        }
    }

    void DropMoedas()
    {
        if (prefabMoeda == null) return;

        int quantidade = Random.Range(moedasMin, moedasMax + 1);
        for (int i = 0; i < quantidade; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0);
            Instantiate(prefabMoeda, pos, Quaternion.identity);
        }
    }

    void DropReliquia()
    {
        if (prefabReliquiaChao == null) return;

        float chance = isMiniChefe ? 1f : chanceDropReliquia;

        if (Random.value <= chance)
        {
            Instantiate(prefabReliquiaChao, transform.position, Quaternion.identity);
        }
    }
}