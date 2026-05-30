using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap tilemapChao;
    public Tilemap tilemapParedes;

    [Header("Sprites")]
    public Sprite spriteChao;
    public Sprite spriteParede;

    [Header("Tamanho da Sala")]
    public int larguraSala = 20;
    public int alturaSala = 20;

    private Tile tileChao;
    private Tile tileParede;

    public static MapGenerator Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CriarTiles();
        GerarSala();
    }

    void CriarTiles()
    {
        tileChao = ScriptableObject.CreateInstance<Tile>();
        tileChao.sprite = spriteChao;
        tileChao.color = new Color(0.7f, 0.7f, 0.7f);

        tileParede = ScriptableObject.CreateInstance<Tile>();
        tileParede.sprite = spriteParede;
        tileParede.color = new Color(0.3f, 0.3f, 0.3f);
    }

    public void GerarSala()
    {
        int[,] mapa = new int[alturaSala, larguraSala];

        
        for (int y = 0; y < alturaSala; y++)
            for (int x = 0; x < larguraSala; x++)
                mapa[y, x] = 0;

        
        for (int x = 0; x < larguraSala; x++)
        {
            mapa[0, x] = 1;
            mapa[alturaSala - 1, x] = 1;
        }
        for (int y = 0; y < alturaSala; y++)
        {
            mapa[y, 0] = 1;
            mapa[y, larguraSala - 1] = 1;
        }

        
        int portaY = alturaSala / 2;
        mapa[portaY, larguraSala - 1] = 0;

        DesenharMapa(mapa);

        
        Porta porta = FindObjectOfType<Porta>();
        if (porta != null)
            porta.transform.position = new Vector3(larguraSala - 1, alturaSala / 2, 0);

        DesenharMapa(mapa);
        
    }

    void DesenharMapa(int[,] mapa)
    {
        tilemapChao.ClearAllTiles();
        tilemapParedes.ClearAllTiles();

        for (int y = 0; y < alturaSala; y++)
        {
            for (int x = 0; x < larguraSala; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (mapa[y, x] == 1)
                    tilemapParedes.SetTile(pos, tileParede);
                else
                    tilemapChao.SetTile(pos, tileChao);
            }
        }
    }
}