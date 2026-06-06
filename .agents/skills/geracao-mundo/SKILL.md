---
name: geracao-mundo
description: Skill para geração procedural de salas, mapas e biomas no Project_Roguelike. Abrange desde a migração do MapGenerator hardcoded até sistemas completos de layout procedural com conectividade garantida.
---

# Geração de Mundo — Salas, Mapas e Biomas

Skill para evoluir o `MapGenerator` atual (100% hardcoded) para um sistema procedural real. Ideal para adicionar variedade de salas, biomas, e escalonamento de dificuldade.

> Carregue `unity-roguelike` primeiro. Use `dados-configuraveis` se for criar SOs para salas/biomas.

---

## 1. Arquitetura de Geração

### Componentes do sistema
```
GeradorDeMapa (singleton)
  ├── SeedManager (semente deterministica ou aleatoria)
  ├── SalaDatabase (lista de prefabs de salas + regras)
  ├── LayoutGenerator (grafo de salas + conectividade)
  ├── SpawnDistributor (posicoes de inimigos/itens por sala)
  └── BiomeController (tema visual por bloco de salas)

  └── Resultado: matriz de tiles + lista de GameObjects (salas)
```

### Fluxo de geração
```
1. SeedManager determina semente da run
2. LayoutGenerator cria grafo: quantas salas, conexoes
3. Para cada sala no grafo:
   a. SalaDatabase escolhe prefab/template via regras
   b. SpawnDistributor posiciona inimigos/itens (dificuldade escalada)
   c. BiomeController aplica paleta de cores/tiles
4. Tilemap é desenhado
5. Portas/conexoes entre salas sao abertas
```

---

## 2. Sistema de Salas

### Tipos de Sala
```csharp
public enum TipoSala
{
    Normal,       // combate padrao
    Descanso,     // sem inimigos, baú/cura
    Chefao,       // boss obrigatorio
    Shop,         // loja com itens
    Segredo,      // entrada oculta, recompensa especial
    Inicio,       // sala inicial da run
    Saida         // portal para proxima fase
}
```

### Templates vs Tilemap procedural
- **Prefabs de sala** (recomendado para primeiro momento):
  - Pré-montadas no Editor, com colliders, spawn points, iluminação
  - Variações de cada tipo (ex: `SalaNormal_01`, `SalaNormal_02`)
  - Mais rápido de produzir, mais previsível
- **Tilemap procedural** (para maturidade do projeto):
  - `MapGenerator` modificado para gerar divisões irregulares
  - Algoritmo BSP (Binary Space Partition) ou Drunkard's Walk
  - Paredes, chão, decoração por ruído/perlin

### Data para SalaDatabase
```csharp
[CreateAssetMenu(fileName = "SalaTemplate", menuName = "Roguelike/SalaTemplate")]
public class SalaTemplate : ScriptableObject
{
    public TipoSala tipo;
    public GameObject prefabSala;
    public int pesoSelecao;        // probabilidade relativa
    public int nivelMinimo;        // so aparece a partir da sala N
    public int nivelMaximo;        // so aparece ate a sala N
    public Vector2 tamanho;        // para calculo de posicionamento
    public bool permiteConexaoNorte, permiteConexaoSul;
    public bool permiteConexaoLeste, permiteConexaoOeste;
}
```

---

## 3. Conectividade Garantida

### Algoritmo de layout: grafo de salas
```csharp
public class LayoutGenerator
{
    public List<SalaNode> GerarLayout(int quantidadeSalas, int seed)
    {
        // 1. Posicionar primeira sala no centro
        // 2. Para cada sala nova:
        //    a. Escolher sala existente aleatoria com espaco
        //    b. Adicionar sala vizinha em direcao aleatoria (N/S/L/O)
        //    c. Verificar sobreposicao: se sobrepor, tentar outra direcao
        // 3. Garantir que todas as salas sao alcancaveis (BFS/DFS)
        // 4. Identificar sala mais distante como "Saida" ou "Chefao"
        // 5. Retornar lista de SalaNode com posicoes e conexoes
    }
}

public class SalaNode
{
    public Vector2Int posicao;
    public TipoSala tipo;
    public List<Vector2Int> vizinhos;
    public SalaTemplate template;
    public bool visitada;
}
```

### Validação de caminho (BFS)
```csharp
public static bool TodasSalasConectadas(List<SalaNode> salas)
{
    if (salas.Count == 0) return true;
    Queue<SalaNode> fila = new Queue<SalaNode>();
    HashSet<SalaNode> visitados = new HashSet<SalaNode>();
    fila.Enqueue(salas[0]);
    visitados.Add(salas[0]);
    while (fila.Count > 0)
    {
        SalaNode atual = fila.Dequeue();
        foreach (Vector2Int vizinho in atual.vizinhos)
        {
            SalaNode noVizinho = salas.Find(s => s.posicao == vizinho);
            if (noVizinho != null && !visitados.Contains(noVizinho))
            {
                visitados.Add(noVizinho);
                fila.Enqueue(noVizinho);
            }
        }
    }
    return visitados.Count == salas.Count;
}
```

---

## 4. Spawn Balanceado de Inimigos

### Regras de dificuldade
```csharp
[System.Serializable]
public class RegraSpawnSala
{
    public TipoSala tipoSala;
    public int inimigosMin;
    public int inimigosMax;
    public float chanceInimigoRanged;
    public float chanceElite;          // inimigo mais forte
    public int quantidadeBaus;
    public bool permiteShop;
}
```

### Distribuição por sala
```csharp
public void DistribuirInimigos(SalaNode sala, int salaAtual)
{
    RegraSpawnSala regra = GetRegra(sala.tipo);
    int quantidade = Random.Range(regra.inimigosMin, regra.inimigosMax + 1);

    // Escalar com progressao
    float escalador = 1f + (salaAtual * 0.1f);
    int vidaExtra = Mathf.FloorToInt(vidaBase * (escalador - 1f));

    for (int i = 0; i < quantidade; i++)
    {
        // Posicao aleatoria dentro da sala (evitando overlap)
        Vector3 pos = GetPosicaoValida(sala);
        GameObject inimigo = Instantiate(prefabInimigo, pos, Quaternion.identity);
        EnemyStats stats = inimigo.GetComponent<EnemyStats>();
        stats.vidaMaxima += vidaExtra;
        stats.vidaAtual = stats.vidaMaxima;
    }
}
```

---

## 5. Biomas / Temas Visuais

### Estrutura de bioma
```csharp
[CreateAssetMenu(fileName = "Bioma", menuName = "Roguelike/Bioma")]
public class BiomaData : ScriptableObject
{
    public string nomeBioma;
    public int salaInicio;         // primeira sala do bioma
    public int salaFim;            // ultima sala do bioma
    public Color corChao;
    public Color corParede;
    public Sprite spriteChao;
    public Sprite spriteParede;
    public Color ambienteTint;     // cor da luz ambiente 2D
    public List<SalaTemplate> salasDisponiveis;
    public AudioClip musicaFundo;
    public List<RegraSpawnSala> regrasSpawn;
}
```

### Transição entre biomas
- A cada N salas (ex: 3), troca de bioma
- Efeito visual de transição (escurecer/clarear tilemap)
- Aumento de dificuldade: inimigos mais fortes, novos tipos

---

## 6. Dicas de Implementação

### Ordem recomendada de implementação

1. **Criar `SalaDatabase` com SOs** (templates)
2. **Criar `GeradorDeMapa`** que seleciona templates
3. **Conectividade**: grafo + BFS
4. **Spawn balanceado** com regras por tipo de sala
5. **Portas/transições** entre salas (trigger → carregar/instanciar proxima sala)
6. **Minimapa** baseado no grafo gerado
7. **Biomas** com paletas e regras diferentes

### Pitfalls comuns

- **Sobreposição**: sempre validar bounding boxes das salas
- **Sala sem saída**: rodar BFS após cada adição
- **Dificuldade desbalanceada**: usar fórmulas testáveis (não chutes)
- **Seed differente no mesmo slot**: salvar seed junto com o save
