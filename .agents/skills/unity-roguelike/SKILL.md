---
name: unity-roguelike
description: Skill fundamental do projeto Project_Roguelike. Deve ser carregada em QUALQUER tarefa. Abrange convenções, arquitetura, padrões de código e pitfalls do código-base Unity 2D roguelike em português.
---

# Unity Roguelike — Skill Fundamental

Skill base para todo o desenvolvimento do **Project_Roguelike** (Unity 2022.3.62f3, URP 2D, pt-BR).

> **Sempre carregue esta skill primeiro.** As demais skills (dados-configuraveis, geracao-mundo, sistemas-gameplay, polimento-apresentacao) complementam para tarefas específicas.

---

## 1. Convenções do Projeto

### Nomenclatura
- **Tudo em pt-BR**: nomes de classes, variáveis, métodos, comentários, logs
- **Sem namespaces**: todos os scripts no global namespace
- **Sem subpastas**: `Assets/Scripts/` é plano — sem subdiretórios
- **Inspector**: campos públicos ou `[SerializeField] private` com `[Header("...")]`
- **Tags**: strings diretas — `"Player"`, `"Inimigo"`, `"Parede"`

### Commits (ver `CONTRIBUTING.md`)
- Português imperativo: `"Adiciona sistema de dano elemental"`
- Branches: `feature/*` a partir de `develop`
- PR com ao menos um review

---

## 2. Arquitetura Geral

```
MenuPrincipal.unity → SelecaoArma.unity → SampleScene.unity
                          └─ (missing no disco!)
```

### Fluxo de dados entre componentes

```
GameManager.Instance (singleton)
  ├─ SalaLimpa() → spawna mini-chefe
  └─ MiniChefeDerrotado() → AvancarSala()

InimigosSpawner
  ├─ Start() → SpawnarInimigos()
  └─ InimigoDerrotado() → quando 0 vivos → GameManager.SalaLimpa()

PlayerController → transform.position (Input.GetAxisRaw)
PlayerDash → Rigidbody2D.velocity (Space)
PlayerAttack → mouse → Melee (OverlapCircleAll) ou Ranged (Projetil)
  └─ Projetil: move em Update, destroi em trigger com "Parede"/"Inimigo"

EnemyAI → Rigidbody2D.velocity → persegue player
EnemyStats → ReceberDano() → Morrer() → FindObjectOfType<InimigosSpawner>()

ArmaChao → OnTriggerEnter2D + C → equipa struct Arma em PlayerAttack
```

### Singleton Pattern (único no projeto)

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
```

---

## 3. Padrões de Código

### Timer-based cooldowns
```csharp
private float temporizador;

void Update()
{
    if (temporizador > 0)
        temporizador -= Time.deltaTime;
}

void Atacar()
{
    if (temporizador > 0) return;
    temporizador = cooldown;
    // executa ataque
}
```

### Busca de componentes
- `FindObjectOfType<T>()` — apenas em inicialização (Start/Awake), nunca em Update
- `GameObject.FindWithTag("Player")` — aceitável em spawners
- `GetComponent<T>()` — preferir cache em Awake/Start

### Tags como strings literais
```csharp
"Player", "Inimigo", "Parede"
```

---

## 4. Regras de Ouro (Pitfalls Conhecidos)

### SEMPRE VERIFIQUE:

1. **`Arma` é struct, não class**
   ```csharp
   public struct Arma { ... }
   // ❌ armaAtual == null NÃO FUNCIONA para structs
   // ✅ usar bool equipada ou Arma? (nullable) ou valor sentinela
   ```

2. **Projétil não causa dano**
   ```csharp
   // Projetil.cs se destroi ao bater em "Inimigo" mas NUNCA chama
   // EnemyStats.ReceberDano(). Sempre adicionar essa chamada.
   ```

3. **Não misturar movimento por transform.position com Rigidbody2D.velocity**
   ```csharp
   // PlayerController usa: transform.position += direcao * speed * Time.deltaTime
   // PlayerDash/EnemyAI usam: Rigidbody2D.velocity = ...
   // Escolha UM padrão por entidade. Preferir Rigidbody2D para consistência.
   ```

4. **Cachear referências obtidas por FindObjectOfType**
   ```csharp
   // ❌ EmemyStats.Morrer() chama FindObjectOfType toda vez que morre
   // ✅ Cachear em Awake: spawner = FindObjectOfType<InimigosSpawner>();
   ```

5. **GameManager precisa de DontDestroyOnLoad**
   ```csharp
   // GameManager não persiste entre cenas. Adicionar:
   // DontDestroyOnLoad(gameObject);
   // em Awake APÓS o singleton check.
   ```

6. **HUD deve usar dirty check**
   ```csharp
   // ❌ HUDController atualiza TextMeshProUGUI todo frame
   // ✅ Comparar valor anterior e só atualizar se mudou
   ```

7. **PlayerDash conflita com PlayerController**
   ```csharp
   // PlayerDash seta Rigidbody2D.velocity mas nunca desabilita
   // PlayerController, que continua movendo via transform.position.
   // Desabilitar PlayerController durante o dash.
   ```

8. **SelecaoArma.unity está no build mas não existe no disco**
   ```csharp
   // Remover do EditorBuildSettings ou criar o arquivo de cena.
   ```

---

## 5. Estrutura de Arquivos

```
Assets/
├── Scenes/
│   ├── MenuPrincipal.unity
│   ├── SampleScene.unity
│   └── ... (futuras salas)
├── Scripts/
│   ├── GameManager.cs
│   ├── PlayerController.cs
│   ├── PlayerAttack.cs
│   ├── PlayerDash.cs
│   ├── PlayerStats.cs
│   ├── EnemyAI.cs
│   ├── EnemyStats.cs
│   ├── ArmaChao.cs
│   ├── InimigosSpawner.cs
│   ├── Projetil.cs
│   ├── CameraFollow.cs
│   ├── HUDController.cs
│   ├── MenuController.cs
│   └── MapGenerator.cs
├── Prefabs/
│   ├── ArmaArco.prefab (Ranged)
│   ├── ArmaEspada.prefab (Melee)
│   ├── ArmaLanca.prefab (Melee)
│   ├── ArmaMachado.prefab (Melee)
│   ├── Inimigo.prefab
│   ├── MiniChefe.prefab
│   └── Projetil.prefab
└── Sprites/
```

---

## 6. Relacionamento com Outras Skills

| Quando a tarefa envolve... | Carregue também |
|---|---|
| Adicionar/modificar armas, inimigos, itens | `dados-configuraveis` |
| Gerar salas, mapas, biomas | `geracao-mundo` |
| Mecânicas de combate, AI, status, shop | `sistemas-gameplay` |
| Áudio, VFX, animações, UI, juice | `polimento-apresentacao` |
