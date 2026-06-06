---
name: dados-configuraveis
description: Skill para dados, configuração e persistência no Project_Roguelike. Abrange ScriptableObjects, save/load, localização e balanceamento. Ideal para tarefas que envolvem adicionar conteúdo ou gerenciar estado do jogo.
---

# Dados Configuráveis — Data-Driven Design

Skill para tornar o projeto data-driven. O código atual usa valores hardcoded em tudo — esta skill documenta como migrar para dados externos e configuraveis.

> Carregue `unity-roguelike` primeiro. Esta skill complementa para tarefas de dados e conteúdo.

---

## 1. ScriptableObjects — Data Assets

### Estrutura recomendada

Criar pasta `Assets/Data/` para armazenar todos os ScriptableObjects.

```csharp
// Data/ArmaData.cs
[CreateAssetMenu(fileName = "NovaArma", menuName = "Roguelike/Arma")]
public class ArmaData : ScriptableObject
{
    public string nomeArma;
    public TipoArma tipo;
    public float danoLeve;
    public float danoPesado;
    public float cooldownLeve;
    public float cooldownPesado;
    public float alcance;
    public GameObject prefabProjetil;
    public Sprite icone;
}
```

```csharp
// Data/InimigoData.cs
[CreateAssetMenu(fileName = "NovoInimigo", menuName = "Roguelike/Inimigo")]
public class InimigoData : ScriptableObject
{
    public string nomeInimigo;
    public float vidaMaxima;
    public float velocidade;
    public float danoContato;
    public float distanciaDeteccao;
    public float intervaloAtaque;
    public GameObject prefab;
    public TipoInimigo tipo; // Melee, Ranged, Charger, etc.
}
```

```csharp
// Data/ItemData.cs
[CreateAssetMenu(fileName = "NovoItem", menuName = "Roguelike/Item")]
public class ItemData : ScriptableObject
{
    public string nomeItem;
    public string descricao;
    public TipoItem tipo; // Cura, Buff, Chave, Moeda, Upgrade
    public float valor;
    public float duracao; // 0 = permanente/instantaneo
    public GameObject prefab;
    public Sprite icone;
}
```

### Como usar no lugar de structs

```csharp
// PlayerAttack passa a referenciar ArmaData em vez da struct Arma
public ArmaData armaAtual;

// Criar no Unity: Assets/Data/ → clique direito → Create → Roguelike → Arma
// Arrastar o SO para o campo no Inspector
```

### Vantagens da migração
- **Novos conteúdos**: criar um SO, sem tocar em código
- **Balanceamento**: ajustar valores em um arquivo, sem recompilar
- **Versionamento**: diffs claros no git (mudanças de valor vs mudanças de código)

---

## 2. Save/Load — Persistência de Estado

### Arquitetura recomendada

```csharp
[System.Serializable]
public class DadosSave
{
    public int salaAtual;
    public int coracoes;
    public int tentativas;
    public string nomeArmaEquipada;
    public int moedas;
    public List<string> upgradesPermanentes;
    public long timestamp;
}
```

```csharp
// SistemaDeSave.cs — centraliza leitura/escrita
public static class SistemaDeSave
{
    private static string Caminho(int slot) =>
        Application.persistentDataPath + $"/save_{slot}.json";

    public static void Salvar(int slot, DadosSave dados)
    {
        string json = JsonUtility.ToJson(dados, true);
        File.WriteAllText(Caminho(slot), json);
    }

    public static DadosSave Carregar(int slot)
    {
        string caminho = Caminho(slot);
        if (!File.Exists(caminho)) return null;
        string json = File.ReadAllText(caminho);
        return JsonUtility.FromJson<DadosSave>(json);
    }

    public static bool SlotExiste(int slot) => File.Exists(Caminho(slot));
}
```

### O que salvar em cada momento

| Momento | O que salvar |
|---|---|
| Ao entrar em sala nova | salaAtual, vida, arma |
| Ao pegar item permanente | upgrades, moedas |
| Ao morrer (game over) | não salvar (run perdida) |
| Menu → "Continuar" | carregar último save |

### Correção do sistema atual
```csharp
// MenuController.IniciarJogo() salva o slot, mas nunca é lido.
// GameManager.Awake() precisa ler: PlayerPrefs.GetInt("SlotAtual", 0);
// E carregar SistemaDeSave.Carregar(slot) após a cena carregar.
```

---

## 3. Localização — Textos Externos

### Estrutura de arquivos
```
Assets/Resources/Localization/
├── pt-BR.json
└── en.json
```

```json
{
    "ui_jogar": "Jogar",
    "ui_opcoes": "Opções",
    "ui_sair": "Sair",
    "item_pocao_vida": "Poção de Vida",
    "item_pocao_vida_desc": "Recupera 2 corações",
    "inimigo_goblin": "Goblin",
    "inimigo_esqueleto": "Esqueleto"
}
```

### Sistema de localização
```csharp
public static class Localizacao
{
    private static Dictionary<string, string> textos;

    public static void CarregarIdioma(string codigo)
    {
        TextAsset arquivo = Resources.Load<TextAsset>($"Localization/{codigo}");
        // parse JSON -> textos
    }

    public static string Get(string chave) =>
        textos.TryGetValue(chave, out string valor) ? valor : chave;
}
```

---

## 4. Balanceamento — Dados Centralizados

### Central de balanceamento
Criar um `GameBalanceSettings` como ScriptableObject singleton referenciado pelo `GameManager`:

```csharp
[CreateAssetMenu(fileName = "GameBalance", menuName = "Roguelike/GameBalance")]
public class GameBalanceSettings : ScriptableObject
{
    public float multiplicadorDanoPorSala = 1.1f;
    public float multiplicadorVidaInimigoPorSala = 1.15f;
    public int maxCoracoes = 5;
    public int maxTentativas = 3;
    public int maxMoedasPorRun = 999;
    public int moedasPorInimigo = 5;
    public float tempoInvulnerabilidade = 1f;
}
```

### Curva de dificuldade
```csharp
// InimigosSpawner usa o GameBalance para escalar inimigos
float GetVidaAjustada(float vidaBase, int salaAtual, GameBalanceSettings balance)
{
    return vidaBase * Mathf.Pow(balance.multiplicadorVidaInimigoPorSala, salaAtual - 1);
}
```

---

## 5. Exemplos de Migração Comum

### Tarefa: Adicionar uma arma nova (ex: Pistola)

1. Criar `ArmaData` → "Pistola", Tipo=Ranged, danos, cooldowns, prefabProjetil
2. Criar prefab `ArmaPistola.prefab` com `ArmaChao` referenciando o SO
3. (Opcional) Criar projetil especifico para a pistola

### Tarefa: Adicionar poção de cura como drop

1. Criar `ItemData` → "PocaoVida", Tipo=Cura, valor=2 (coracoes)
2. Criar prefab com trigger + script que usa o `ItemData`
3. Ao colidir com player: `PlayerStats.ReceberCura(item.valor)`

### Tarefa: Adicionar upgrade permanente (ex: +1 coração)

1. Adicionar campo `coracoesBonus` em `DadosSave`
2. Ao completar checkpoints, incrementar
3. `PlayerStats.Awake()` aplica o bonus ao inicializar a run
