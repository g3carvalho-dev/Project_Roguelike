using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap tilemapChao;
    public Tilemap tilemapParedes;

    [Header("Sprites")]
    public Sprite spriteChao;
    public Sprite spriteParede;

    [Header("Tamanho do Mapa")]
    public int largura = 40;
    public int altura = 40;

    private Tile tileChao;
    private Tile tileParede;

    void Start()
    {
        CriarTiles();
        int[,] mapa = GerarMapa();
        DesenharMapa(mapa);
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

    int[,] GerarMapa()
    {
        int[,] mapa = new int[altura, largura];

        // tudo começa como parede
        for (int y = 0; y < altura; y++)
            for (int x = 0; x < largura; x++)
                mapa[y, x] = 1;

        // sala de repouso (onde o player começa)
        for (int y = 2; y < 12; y++)
            for (int x = 2; x < 22; x++)
                mapa[y, x] = 0;

        // corredor conectando as salas
        for (int y = 11; y < 15; y++)
            for (int x = 10; x < 13; x++)
                mapa[y, x] = 0;

        // sala principal do jogo
        for (int y = 14; y < 35; y++)
            for (int x = 5; x < 35; x++)
                mapa[y, x] = 0;

        return mapa;
    }

    void DesenharMapa(int[,] mapa)
    {
        tilemapChao.ClearAllTiles();
        tilemapParedes.ClearAllTiles();

        for (int y = 0; y < altura; y++)
        {
            for (int x = 0; x < largura; x++)
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