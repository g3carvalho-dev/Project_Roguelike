using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
 
    [Header("Progressao")]
    public int salaAtual = 1;
    public int totalSalas = 9;
 
    [Header("Estado")]
    public bool salaLimpa = false;
    public bool minichefeDerrotado = false;
 
    [Header("Moedas")]
    public int moedas = 0;
    public System.Action onMoedasAtualizadas; // HUD escuta isso
 
    void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        VerificarCheckpoint();
    }
 
    void VerificarCheckpoint()
    {
        if (salaAtual % 3 == 0)
        {
            Debug.Log("Checkpoint atingido na sala " + salaAtual + "!");
 
            if (salaAtual % 3 == 0 && salaAtual != 9)
                Debug.Log("Loja disponivel antes do chefe!");
 
            if (salaAtual == 9)
                Debug.Log("Chefe final!");
        }
 
        AvancarSala();
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
 
    public GameObject prefabMiniChefe;
 
    void SpawnarMiniChefe()
    {
        if (prefabMiniChefe == null)
        {
            Debug.LogError("Prefab do mini chefe nao conectado!");
            return;
        }
 
        Vector3 posicao = new Vector3(20, 24, 0);
        Instantiate(prefabMiniChefe, posicao, Quaternion.identity);
    }
}