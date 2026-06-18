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

    [Header("Moedas")]
    public int moedas = 0;
    public System.Action onMoedasAtualizadas;

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
        Debug.Log("Sala " + salaAtual + " limpa! Spawnando mini chefe...");
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

        VerificarCheckpoint();
    }

    void VerificarCheckpoint()
    {
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

        Debug.Log("Voltando para sala de repouso para trocar equipamentos (1 vez cada)");
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

    void SpawnarMiniChefe()
    {
        if (prefabMiniChefe == null)
        {
            Debug.LogError("Prefab do mini chefe nao conectado!");
            return;
        }

        Vector3 posicao = new Vector3(15, 15, 0);
        GameObject miniChefe = Instantiate(prefabMiniChefe, posicao, Quaternion.identity);

        EnemyAI ai = miniChefe.GetComponent<EnemyAI>();
        if (ai != null)
            ai.player = GameObject.FindWithTag("Player").transform;

        Debug.Log("Mini chefe spawnado!");
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