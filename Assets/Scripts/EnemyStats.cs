using UnityEngine;
 
public class EnemyStats : MonoBehaviour
{
    public float vidaMaxima = 50f;
    public float vidaAtual;
 
    [Header("Drop de Moedas")]
    public GameObject prefabMoeda;      // arraste o prefab da moeda aqui
    public int moedasMin = 1;
    public int moedasMax = 3;
 
    void Start()
    {
        vidaAtual = vidaMaxima;
    }
 
    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        Debug.Log(gameObject.name + " tomou " + dano + " de dano. Vida: " + vidaAtual);
 
        if (vidaAtual <= 0)
            Morrer();
    }
 
    void Morrer()
    {
        Debug.Log(gameObject.name + " morreu!");
 
        DropMoedas();
 
        InimigosSpawner spawner = FindObjectOfType<InimigosSpawner>();
        if (spawner != null)
            spawner.InimigoDerrotado();
 
        Destroy(gameObject);
    }
 
    void DropMoedas()
    {
        if (prefabMoeda == null) return;
 
        int quantidade = Random.Range(moedasMin, moedasMax + 1);
        for (int i = 0; i < quantidade; i++)
        {
            // espalha as moedas levemente ao redor do inimigo
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0);
            Instantiate(prefabMoeda, pos, Quaternion.identity);
        }
    }
}