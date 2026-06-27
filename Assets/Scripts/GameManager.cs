using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progressao")]
    public int salaAtual = 1;
    public int totalSalas = 9;
    public int checkpointSala = 1;

    [Header("Estado")]
    public bool salaLimpa = false;
    public bool minichefeDerrotado = false;

    public GameObject prefabMiniChefe;
    public GameObject prefabBoss;

    [Header("Animators do Mini Chefe (0=Sala 2, 1=Sala 4, 2=Sala 5, 3=Sala 7, 4=Sala 8)")]
    public RuntimeAnimatorController[] animatorsMiniChefePorSala;

    [Header("Animators do Boss (índice 0 = sala 3, 1 = sala 6, 2 = sala 9)")]
    public RuntimeAnimatorController[] animatorsBossPorSala;

    [Header("Moedas")]
    public int moedas = 0;
    public System.Action onMoedasAtualizadas;

    [Header("Tentativas")]
    public int tentativasMaximas = 3;
    public int tentativasAtuais;

    [Header("Troca limitada na sala de repouso")]
    public bool podeTrocarArma = true;
    public bool podeTrocarReliquia = true;
    public bool voltandoDeMorte = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        tentativasAtuais = PlayerPrefs.GetInt("Tentativas", tentativasMaximas);
    }

    public void PerderTentativa()
    {
        tentativasAtuais--;
        PlayerPrefs.SetInt("Tentativas", tentativasAtuais);
        Debug.Log("Tentativas restantes: " + tentativasAtuais);

        if (tentativasAtuais <= 0)
        {
            Debug.Log("Game Over!");
            PlayerPrefs.SetInt("Tentativas", tentativasMaximas);
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            Debug.Log("Voltando para sala de repouso para trocar equipamentos (1 vez cada)");
            VoltarParaSalaDeRepousoTroca();
        }
    }

    public void AdicionarMoeda(int quantidade = 1)
    {
        moedas += quantidade;
        Debug.Log("[Moedas] Total: " + moedas);
        onMoedasAtualizadas?.Invoke();
    }

    public void SalaLimpa()
    {
        salaLimpa = true;
        Debug.Log("Sala " + salaAtual + " limpa!");

        // Salas de boss não têm inimigos comuns, não chega aqui
        // Minichefe só nas salas 2,4,5,7,8
        SpawnarMiniChefe();
    }

    public void MiniChefeDerrotado()
    {
        minichefeDerrotado = true;
        Debug.Log("Mini chefe derrotado! Proxima sala liberada.");

        Porta porta = FindObjectOfType<Porta>();
        if (porta != null)
            porta.Desbloquear();

        PlayerFeiticos feiticos = FindObjectOfType<PlayerFeiticos>();
        if (feiticos != null)
        {
            Feitico novoFeitico = new Feitico();
            novoFeitico.nome = "Onda de Choque";
            novoFeitico.tipo = TipoFeitico.Onda;
            novoFeitico.dano = 30f;
            novoFeitico.cooldown = 5f;
            feiticos.DesbloquearFeitico(novoFeitico);
        }

        // Checkpoint sem avançar sala — a porta faz isso ao ser atravessada
        if (salaAtual % 3 == 0)
        {
            checkpointSala = salaAtual;
            PlayerPrefs.SetInt("Checkpoint", checkpointSala);
            Debug.Log("Checkpoint salvo na sala " + checkpointSala + "!");

            if (salaAtual != 9)
            {
                Debug.Log("Loja disponivel antes do chefe!");
                if (LojaManager.Instance != null)
                    LojaManager.Instance.AbrirLoja();
                else
                    Debug.LogError("LojaManager.Instance esta nulo!");
            }
            else
                Debug.Log("Chefe final!");
        }
    }

    void AvancarSala()
    {
        if (salaAtual >= totalSalas)
        {
            Debug.Log("Vitoria!");
            return;
        }

        salaAtual++;
        salaLimpa = false;
        minichefeDerrotado = false;
        Debug.Log("Avancando para sala " + salaAtual);
    }

    public void VoltarParaSalaDeRepousoTroca()
    {
        salaAtual = 1;
        salaLimpa = false;
        minichefeDerrotado = false;
        podeTrocarArma = true;
        podeTrocarReliquia = true;
        voltandoDeMorte = true;

        SceneManager.LoadScene("SampleScene");
    }

    public void VoltarParaCheckpoint()
    {
        voltandoDeMorte = false;
        salaAtual = checkpointSala;
        salaLimpa = false;
        minichefeDerrotado = false;
        Debug.Log("Voltando para checkpoint na sala " + checkpointSala);
        SceneManager.LoadScene("SampleScene");
    }

    public void ResetarJogo()
    {
        tentativasAtuais = tentativasMaximas;
        PlayerPrefs.SetInt("Tentativas", tentativasMaximas);
        moedas = 0;
        salaAtual = 1;
        checkpointSala = 1;
        salaLimpa = false;
        minichefeDerrotado = false;
    }

   void SpawnarMiniChefe()
    {
        if (prefabMiniChefe == null)
        {
            Debug.LogError("Prefab do mini chefe nao conectado!");
            return;
        }

        Vector3 posicao = new Vector3(15, 15, 0);
        GameObject miniChefe = Instantiate(prefabMiniChefe, posicao, Quaternion.identity);

        // Aplica AnimatorController do mini chefe baseado apenas nas salas válidas
        if (animatorsMiniChefePorSala != null && animatorsMiniChefePorSala.Length > 0)
        {
            int idx = -1;
            
            // Mapeia a sala atual para o índice correto no Inspector sem pular números
            switch (salaAtual)
            {
                case 2: idx = 0; break;
                case 4: idx = 1; break;
                case 5: idx = 2; break;
                case 7: idx = 3; break;
                case 8: idx = 4; break;
            }

            // Se for um índice válido, aplica a animação correspondente
            if (idx >= 0 && idx < animatorsMiniChefePorSala.Length)
            {
                RuntimeAnimatorController ac = animatorsMiniChefePorSala[idx];
                if (ac != null)
                {
                    Animator anim = miniChefe.GetComponent<Animator>();
                    if (anim != null) anim.runtimeAnimatorController = ac;
                }
            }
        }

        EnemyAI ai = miniChefe.GetComponent<EnemyAI>();
        if (ai != null)
            ai.player = GameObject.FindWithTag("Player").transform;

        Debug.Log("Mini chefe spawnado!");
    }
    public bool EhSalaDeBoss() => salaAtual == 3 || salaAtual == 6 || salaAtual == 9;

    public void BossDerrotado()
    {
        minichefeDerrotado = true;
        Debug.Log("Boss derrotado! Proxima sala liberada.");

        Porta porta = FindObjectOfType<Porta>();
        if (porta != null)
            porta.Desbloquear();

        checkpointSala = salaAtual;
        PlayerPrefs.SetInt("Checkpoint", checkpointSala);
        Debug.Log("Checkpoint salvo na sala " + checkpointSala + "!");

        if (salaAtual != 9)
        {
            Debug.Log("Loja disponivel apos o boss!");
            if (LojaManager.Instance != null)
                LojaManager.Instance.AbrirLoja();
            else
                Debug.LogError("LojaManager.Instance esta nulo!");
        }
        else
            Debug.Log("Chefe final derrotado! Vitoria!");
    }

    public void SpawnarBoss()
    {
        if (prefabBoss == null)
        {
            Debug.LogError("Prefab do Boss nao conectado!");
            return;
        }

        Vector3 posicao = new Vector3(15, 15, 0);
        GameObject boss = Instantiate(prefabBoss, posicao, Quaternion.identity);

        if (animatorsBossPorSala != null && animatorsBossPorSala.Length > 0)
        {
            int idx = salaAtual == 3 ? 0 : salaAtual == 6 ? 1 : 2;
            idx = Mathf.Clamp(idx, 0, animatorsBossPorSala.Length - 1);
            RuntimeAnimatorController ac = animatorsBossPorSala[idx];
            if (ac != null)
            {
                Animator anim = boss.GetComponent<Animator>();
                if (anim != null) anim.runtimeAnimatorController = ac;
            }
        }

        EnemyAI ai = boss.GetComponent<EnemyAI>();
        if (ai != null)
            ai.player = GameObject.FindWithTag("Player").transform;

        EnemyStats stats = boss.GetComponent<EnemyStats>();
        if (stats != null)
            stats.isBoss = true;

        Debug.Log("Boss spawnado na sala " + salaAtual + "!");
    }

    public void AvancarParaProximaSala()
    {
        salaAtual++;
        salaLimpa = false;
        minichefeDerrotado = false;
        Debug.Log("Entrando na sala " + salaAtual);

        MapGenerator.Instance.GerarSala();

        GameObject salaRepouso = GameObject.Find("SalaRepouso");
        if (salaRepouso != null)
            salaRepouso.SetActive(false);

        ArmaChao[] armasChao = FindObjectsByType<ArmaChao>(FindObjectsSortMode.None);
        foreach (ArmaChao arma in armasChao)
            Destroy(arma.gameObject);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = new Vector3(2, 10, 0);

        Porta porta = FindObjectOfType<Porta>();
        if (porta != null)
            porta.Resetar();

        InimigosSpawner spawner = FindObjectOfType<InimigosSpawner>();
        if (spawner != null)
        {
            spawner.salaLimpa = false;
            spawner.inimigosMortos = 0;
            spawner.centroSala = new Vector2(15, 15);
            spawner.areaSpawnX = 10;
            spawner.areaSpawnY = 10;
            spawner.SpawnarInimigos();
        }
    }
}