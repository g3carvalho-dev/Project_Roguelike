using UnityEngine;

public class InimigosSpawner : MonoBehaviour
{
    public GameObject prefabInimigo;
    public int quantidadeInimigos = 5;
    public float areaSpawnX = 15f;
    public float areaSpawnY = 15f;
    public Vector2 centroSala;

    [Header("Animators de inimigo por sala de combate (0=sala2, 1=sala4, 2=sala5, 3=sala7, 4=sala8)")]
    public RuntimeAnimatorController[] animatorsInimigoPorSala;

    public int inimigosMortos = 0;
    private int totalInimigos = 0;
    public bool salaLimpa = false;

    // Mapeia salaAtual para índice do array (ignora salas de boss 3,6,9)
    int GetIndiceAnimator(int salaAtual)
    {
        switch (salaAtual)
        {
            case 2: return 0;
            case 4: return 1;
            case 5: return 2;
            case 7: return 3;
            case 8: return 4;
            default: return -1; // sala de boss ou inválida
        }
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.salaAtual == 1)
        {
            Debug.Log("Sala de repouso - sem inimigos!");
            return;
        }

        SpawnarInimigos();
    }

    public void SpawnarInimigos()
    {
        // Sala de boss - spawna so o boss, sem inimigos comuns
        if (GameManager.Instance != null && GameManager.Instance.EhSalaDeBoss())
        {
            Debug.Log("Sala de boss - spawning boss diretamente.");
            GameManager.Instance.SpawnarBoss();
            return;
        }

        totalInimigos = quantidadeInimigos;

        RuntimeAnimatorController animatorAtual = null;
        if (animatorsInimigoPorSala != null && animatorsInimigoPorSala.Length > 0)
        {
            int salaAtual = GameManager.Instance != null ? GameManager.Instance.salaAtual : 2;
            int idx = GetIndiceAnimator(salaAtual);
            if (idx >= 0 && idx < animatorsInimigoPorSala.Length)
                animatorAtual = animatorsInimigoPorSala[idx];
        }

        for (int i = 0; i < quantidadeInimigos; i++)
        {
            float x = centroSala.x + Random.Range(-areaSpawnX / 2, areaSpawnX / 2);
            float y = centroSala.y + Random.Range(-areaSpawnY / 2, areaSpawnY / 2);

            GameObject inimigo = Instantiate(prefabInimigo, new Vector3(x, y, 0), Quaternion.identity);

            if (animatorAtual != null)
            {
                Animator anim = inimigo.GetComponent<Animator>();
                if (anim != null) anim.runtimeAnimatorController = animatorAtual;
            }

            EnemyAI ai = inimigo.GetComponent<EnemyAI>();
            if (ai != null)
                ai.player = GameObject.FindWithTag("Player").transform;
        }
    }

    public void InimigoDerrotado()
    {
        if (salaLimpa) return;

        inimigosMortos++;
        Debug.Log("Inimigos mortos: " + inimigosMortos + "/" + totalInimigos);

        if (inimigosMortos >= totalInimigos)
        {
            salaLimpa = true;
            if (GameManager.Instance != null)
                GameManager.Instance.SalaLimpa();
            else
                Debug.LogError("GameManager.Instance esta nulo!");
        }
    }
}