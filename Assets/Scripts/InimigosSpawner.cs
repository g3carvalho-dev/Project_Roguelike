using UnityEngine;
using System.Collections;

public class InimigosSpawner : MonoBehaviour
{
    public GameObject prefabInimigo;
    public int quantidadeInimigos = 5;
    public float areaSpawnX = 15f;
    public float areaSpawnY = 15f;
    public Vector2 centroSala;

    public int inimigosMortos = 0;
    private int totalInimigos = 0;
    public bool salaLimpa = false;

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
        totalInimigos = quantidadeInimigos;

        for (int i = 0; i < quantidadeInimigos; i++)
        {
            float x = centroSala.x + Random.Range(-areaSpawnX / 2, areaSpawnX / 2);
            float y = centroSala.y + Random.Range(-areaSpawnY / 2, areaSpawnY / 2);

            Vector3 posicao = new Vector3(x, y, 0);
            GameObject inimigo = Instantiate(prefabInimigo, posicao, Quaternion.identity);

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