---
name: sistemas-gameplay
description: Skill para mecânicas de combate, inimigos, itens, status effects e progressão no Project_Roguelike. Abrange desde a criação de novos tipos de inimigo até sistemas complexos como boss phases e meta-progressão.
---

# Sistemas de Gameplay — Combate, Inimigos, Itens e Progressão

Skill para expandir as mecânicas do jogo. O combate atual é raso (1 inimigo, 2 tipos de ataque, sem itens) — esta skill documenta padrões para criar profundidade.

> Carregue `unity-roguelike` primeiro. Use `dados-configuraveis` se for criar SOs para armas/inimigos/status.

---

## 1. Arquitetura de Inimigos

### Base componentizável
```csharp
public enum TipoInimigo { Melee, Ranged, Charger, Teleporter, Summoner, Exploding }

public class InimigoBase : MonoBehaviour
{
    public InimigoData dados;
    public InimigoEstado estado; // Patrulha, Persegue, Ataca, Stun, Morto
    protected Rigidbody2D rb;
    protected Transform player;
    protected float temporizadorAtaque;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
    }

    public virtual void ReceberDano(float dano)
    {
        dados.vidaAtual -= dano;
        if (dados.vidaAtual <= 0) Morrer();
    }

    protected virtual void Morrer()
    {
        // efeitos, drops, notificar spawner
        Destroy(gameObject);
    }
}
```

### Padrão de comportamentos por tipo

```csharp
// InimigoRanged.cs
public class InimigoRanged : InimigoBase
{
    public GameObject prefabProjetil;
    public float distanciaSeguranca = 4f;

    protected override void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < distanciaSeguranca)
        {
            // Fugir do jogador
            Vector2 dir = (transform.position - player.position).normalized;
            rb.velocity = dir * dados.velocidade;
        }
        else if (dist < dados.distanciaDeteccao)
        {
            // Mirar e atirar
            Vector2 dir = (player.position - transform.position).normalized;
            // rotacionar para direcao
            if (temporizadorAtaque <= 0)
            {
                Atirar(dir);
                temporizadorAtaque = dados.intervaloAtaque;
            }
        }
        temporizadorAtaque -= Time.deltaTime;
    }

    void Atirar(Vector2 dir)
    {
        GameObject p = Instantiate(prefabProjetil, transform.position, Quaternion.identity);
        p.GetComponent<ProjetilInimigo>().Iniciar(dir, dados.danoContato);
    }
}
```

```csharp
// InimigoCharger.cs
public class InimigoCharger : InimigoBase
{
    public float velocidadeCarga = 8f;
    public float tempoPreparacao = 0.5f;
    private bool carregando;

    protected override void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < dados.distanciaDeteccao && !carregando)
            StartCoroutine(Carregar());
    }

    IEnumerator Carregar()
    {
        carregando = true;
        // pausa para avisar jogador
        yield return new WaitForSeconds(tempoPreparacao);
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * velocidadeCarga;
        yield return new WaitForSeconds(0.3f);
        rb.velocity = Vector2.zero;
        carregando = false;
    }
}
```

---

## 2. Bosses com Fases (Phase Transitions)

### Estrutura de boss
```csharp
public class BossBase : MonoBehaviour
{
    public int fases;
    public int faseAtual = 1;
    public float vidaMaxima;
    private float vidaAtual;

    public List<AtaqueBoss> ataquesFase1;
    public List<AtaqueBoss> ataquesFase2;

    void Update()
    {
        if (vidaAtual <= vidaMaxima * 0.5f && faseAtual == 1)
            TransicionarFase(2);

        ExecutarAtaque(GetAtaquesDaFase());
    }

    void TransicionarFase(int novaFase)
    {
        faseAtual = novaFase;
        // Efeito visual, pausa, reset de posicao, invulnerabilidade temporaria
        // Novos ataques, nova velocidade, novos patterns
    }
}

[System.Serializable]
public class AtaqueBoss
{
    public string nome;
    public float dano;
    public float tempoCarga;
    public float cooldown;
    public GameObject prefabProjetil;
    public int projetilQuantidade;
    public float spread; // angulo entre projeteis
}
```

### Padrões de ataque comuns para bosses 2D
- **Rajada em leque**: vários projéteis em ângulos diferentes
- **Ataque telegrafado**: area de dano que aparece antes de cair
- **Investida**: boss se move rapidamente em linha reta
- **Invocação**: spawna inimigos menores
- **Barreira**: fica invulnerável até totens serem destruídos

---

## 3. Sistema de Itens e Pickups

### Hierarquia de itens
```csharp
public enum TipoItem
{
    Cura,           // restaura vida
    BuffTemporario, // aumenta dano/velocidade por X segundos
    Municao,        // para armas ranged (se implementar limite)
    Chave,          // abre portas especiais
    Moeda,          // moeda para shop
    UpgradePermanente, // +vida max, +dano (dentro da run)
    Armadura        // reducao de dano temporaria
}

public class ItemPickup : MonoBehaviour
{
    public ItemData dados;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        AplicarEfeito(other.GetComponent<PlayerStats>());
        Destroy(gameObject);
    }

    void AplicarEfeito(PlayerStats stats)
    {
        switch (dados.tipo)
        {
            case TipoItem.Cura:
                stats.ReceberCura((int)dados.valor);
                break;
            case TipoItem.BuffTemporario:
                stats.AplicarBuff(dados.idBuff, dados.valor, dados.duracao);
                break;
            case TipoItem.Moeda:
                GameManager.Instance.AdicionarMoedas((int)dados.valor);
                break;
            case TipoItem.UpgradePermanente:
                stats.AumentarVidaMaxima((int)dados.valor);
                break;
        }
    }
}
```

### Drop de inimigos
```csharp
[System.Serializable]
public class DropEntry
{
    public ItemData item;
    [Range(0f, 1f)] public float chance = 0.1f;
}

public class EnemyDrops : MonoBehaviour
{
    public List<DropEntry> drops;

    public void OnDeath()
    {
        foreach (DropEntry entry in drops)
        {
            if (Random.value <= entry.chance)
                Instantiate(entry.item.prefab, transform.position, Quaternion.identity);
        }
    }
}
```

---

## 4. Status Effects (Efeitos de Estado)

### Arquitetura genérica de status
```csharp
public enum TipoStatus
{
    Veneno,  // dano ao longo do tempo
    Lentidao, // reduz velocidade
    Stun,     // paralisa completamente
    Queimadura, // dano DOT + visual de fogo
    Sangramento, // dano ao se mover
    Escudo     // reduz dano recebido
}

[System.Serializable]
public class StatusEffect
{
    public TipoStatus tipo;
    public float duracaoRestante;
    public float intensidade;    // dano/tick ou % de reducao
    public float intervalo;      // tick a cada X segundos (DOTs)
    public GameObject efeitoVisual;
    public GameObject efeitoTick; // prefab que spawna a cada tick
}

public interface IStatusReceiver
{
    void AplicarStatus(StatusEffect status);
    void RemoverStatus(TipoStatus tipo);
    bool TemStatus(TipoStatus tipo);
}
```

### Gerenciador de status no player/inimigo
```csharp
public class StatusManager : MonoBehaviour, IStatusReceiver
{
    public List<StatusEffect> statusesAtivos = new List<StatusEffect>();

    void Update()
    {
        for (int i = statusesAtivos.Count - 1; i >= 0; i--)
        {
            StatusEffect s = statusesAtivos[i];
            s.duracaoRestante -= Time.deltaTime;
            if (s.duracaoRestante <= 0)
            {
                RemoverStatus(s.tipo);
                continue;
            }
            ProcessarTick(s);
        }
    }

    void ProcessarTick(StatusEffect s)
    {
        // redutor de chamadas: executar a cada `intervalo` segundos
        // Veneno/Queimadura: causar dano (intensidade * escalador)
        // Sangramento: causar dano extra quando toma dano
        // Lentidao: multiplicar velocidade por (1 - intensidade)
        // Stun: travar movimento
    }

    public void AplicarStatus(StatusEffect status)
    {
        // Se ja existe do mesmo tipo, renovar duracao (ou acumular)
        // Senao, adicionar e instanciar efeito visual
    }
}
```

### Como aplicar um status
```csharp
// Exemplo: arma de fogo causa queimadura
// Em PlayerAttack, ao acertar inimigo:
if (armaAtual.tipo == TipoArma.Ranged && Random.value <= 0.3f)
{
    StatusEffect queimadura = new StatusEffect
    {
        tipo = TipoStatus.Queimadura,
        duracaoRestante = 5f,
        intensidade = 3f,
        intervalo = 1f
    };
    inimigo.GetComponent<StatusManager>().AplicarStatus(queimadura);
}
```

---

## 5. Shop entre Salas

### Sistema de economia
```csharp
// GameManager adiciona:
public int moedas;
public void AdicionarMoedas(int quantia) { moedas += quantia; }
public bool GastarMoedas(int custo)
{
    if (moedas < custo) return false;
    moedas -= custo;
    return true;
}
```

### Sala de shop
- `TipoSala.Shop` na geracao de mapa
- NPC ou pedestal com 3-4 itens aleatorios
- Cada item tem preço em moedas
- Itens possiveis: cura, buff temporario, upgrade permanente, nova arma

---

## 6. Meta-Progresão (Entre Runs)

### Conceito
```
Run 1: morre na sala 3, ganhou 50 moedas de upgrade
       → gasta no hub: +1 coração maximo para TODAS as runs futuras
Run 2: comeca com 4 corações (3 base + 1 upgrade)
       → chega mais longe, ganha mais moedas
```

### Dados de meta-progressão
```csharp
[System.Serializable]
public class DadosMetaProgressao
{
    public int moedasTotais;
    public int bonusVidaMaxima;
    public float bonusDano;
    public List<string> armasDesbloqueadas;
    public bool bossCompleto;    // ja derrotou o boss final?
}
```

### Como salvar
- Separado do save de run: `Application.persistentDataPath + "/meta_progress.json"`
- Carregado ao abrir o jogo (no menu)
- Atualizado ao finalizar ou morrer em uma run

---

## 7. Dicas de Implementação

### Ordem recomendada
1. **Corrigir `Projetil`** para causar dano (bug critico)
2. **Sistema de drops** (moedas + itens basicos)
3. **Inimigo Ranged** (primeira variacao de inimigo)
4. **Sistema de status effects** (reutilizavel para armas e inimigos)
5. **Shop** (da valor as moedas)
6. **Boss com fases** (requer sistema de inimigos maduro)
7. **Meta-progressão** (motivacao para repetir runs)

### Pitfalls
- **Composição vs herança**: prefira composição (`StatusManager` como componente) a herança profunda de inimigos
- **DOT stacking**: definir regra clara (renova duração? acumula intensidade? apenas um por tipo?)
- **Economia**: testar quantas moedas o jogador ganha por sala vs. precos do shop
- **Meta-progressão**: nunca salvar no meio de uma acao critica (sempre no menu ou entre salas)
