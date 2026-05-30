---
name: polimento-apresentacao
description: Skill para áudio, VFX, animações, UI e juice visual no Project_Roguelike. Abrange desde sistemas de SFX e partículas até animações de spritesheet e polimento de câmera/HUD.
---

# Polimento e Apresentação — Áudio, VFX, Animações, UI e Juice

Skill para dar vida ao jogo. O projeto atual é completamente silencioso, sem animações, sem VFX, e com UI mínima. Esta skill documenta padrões para implementar áudio, partículas, animações e polimento visual.

> Carregue `unity-roguelike` primeiro. Esta skill é puramente de apresentação — não modifica lógica de gameplay.

---

## 1. Sistema de Áudio

### Gerenciador de áudio centralizado
```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource ambientSource;

    [Header("SFX")]
    public AudioClip sfxAtaqueEspada;
    public AudioClip sfxAtaqueArco;
    public AudioClip sfxDanoPlayer;
    public AudioClip sfxDanoInimigo;
    public AudioClip sfxMoeda;
    public AudioClip sfxItem;
    public AudioClip sfxPortaAbrir;
    public AudioClip sfxBotaoUI;

    [Header("Musica")]
    public AudioClip musicaExploracao;
    public AudioClip musicaBoss;
    public AudioClip musicaMenu;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void TocarSFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void TocarMusica(AudioClip clip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeMusica(clip, fadeDuration));
    }

    private IEnumerator FadeMusica(AudioClip novaMusica, float duracao)
    {
        float tempo = 0;
        float volumeInicial = musicSource.volume;
        while (tempo < duracao)
        {
            musicSource.volume = Mathf.Lerp(volumeInicial, 0, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = 0;
        musicSource.clip = novaMusica;
        musicSource.Play();
        tempo = 0;
        while (tempo < duracao)
        {
            musicSource.volume = Mathf.Lerp(0, volumeInicial, tempo / duracao);
            tempo += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = volumeInicial;
    }
}
```

### Pooling de AudioSource (muitos SFX simultâneos)
```csharp
public class AudioPool : MonoBehaviour
{
    private Queue<AudioSource> pool = new Queue<AudioSource>();
    public int tamanhoInicial = 5;

    void Start()
    {
        for (int i = 0; i < tamanhoInicial; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            pool.Enqueue(source);
        }
    }

    public AudioSource Obter()
    {
        if (pool.Count == 0)
        {
            var extra = gameObject.AddComponent<AudioSource>();
            extra.playOnAwake = false;
            return extra;
        }
        return pool.Dequeue();
    }

    public void Devolver(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        pool.Enqueue(source);
    }
}
```

### Mixer (controle de volume)
- Criar `AudioMixer` no Unity (Window → Audio → Audio Mixer)
- Grupos: `Master`, `SFX`, `Musica`, `Ambiente`
- Expor parâmetros: `sfxVolume`, `musicaVolume`, `masterVolume`
- Sliders na UI de opções ajustam `AudioMixer.SetFloat("sfxVolume", Mathf.Log10(slider.value) * 20)`

---

## 2. Sistema de VFX (Partículas)

### Gerenciador de partículas com pooling
```csharp
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Prefabs de Efeitos")]
    public GameObject vfxDanoInimigo;
    public GameObject vfxMorteInimigo;
    public GameObject vfxDanoPlayer;
    public GameObject vfxDashTrail;
    public GameObject vfxColetaItem;
    public GameObject vfxAbrirPorta;
    public GameObject vfxExplosao;

    private Dictionary<GameObject, Queue<ParticleSystem>> poolVFX;

    void Awake()
    {
        Instance = this;
        poolVFX = new Dictionary<GameObject, Queue<ParticleSystem>>();
    }

    public void TocarVFX(GameObject prefab, Vector3 posicao, float duracao = 1f)
    {
        if (!poolVFX.ContainsKey(prefab))
            poolVFX[prefab] = new Queue<ParticleSystem>();

        Queue<ParticleSystem> fila = poolVFX[prefab];
        ParticleSystem ps;

        if (fila.Count > 0 && fila.Peek().isStopped)
        {
            ps = fila.Dequeue();
            ps.gameObject.transform.position = posicao;
        }
        else
        {
            ps = Instantiate(prefab, posicao, Quaternion.identity).GetComponent<ParticleSystem>();
        }

        ps.Play();
        fila.Enqueue(ps);

        if (duracao > 0)
            StartCoroutine(DevolverApos(ps, duracao));
    }

    private IEnumerator DevolverApos(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.gameObject.SetActive(false);
    }
}
```

### Onde tocar cada VFX
```
Ataque acertou inimigo   → VFXManager.TocarVFX(danoInimigo, pontoImpacto)
Inimigo morreu           → VFXManager.TocarVFX(morteInimigo, posInimigo)
Player tomou dano        → VFXManager.TocarVFX(danoPlayer, posPlayer)
Player dando dash        → VFXManager.TocarVFX(dashTrail, posPlayer)
Item coletado            → VFXManager.TocarVFX(coletaItem, posItem)
```

---

## 3. Screen Shake

```csharp
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Transform camTransform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.2f;
    private Vector3 posicaoOriginal;

    void Awake()
    {
        Instance = this;
        camTransform = Camera.main.transform;
        posicaoOriginal = camTransform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = posicaoOriginal + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = posicaoOriginal;
        }
    }

    public void Tocar(float duracao = 0.15f, float magnitude = 0.2f)
    {
        shakeDuration = duracao;
        shakeMagnitude = magnitude;
    }
}
```

### Quando usar shake
```
Ataque pesado acerta     → CameraShake.Tocar(0.15f, 0.3f)
Explosao                 → CameraShake.Tocar(0.3f, 0.5f)
Boss bate no chao        → CameraShake.Tocar(0.2f, 0.4f)
Player toma dano         → CameraShake.Tocar(0.1f, 0.15f)
```

---

## 4. Sistema de Animações (Spritesheet)

### Estrutura de Animator Controller
```
Controller: Jogador.controller
  ├─ Idle       ←─┬─→ Walk (blend tree por direcao)
  ├─ Walk       ←──┤
  ├─ Attack     (trigger "Atacar")
  ├─ Dash       (trigger "Dash")
  ├─ Hit        (trigger "Dano")
  └─ Death      (trigger "Morrer")
```

### Animation Events (sync som/dano com animação)
```csharp
// No frame de impacto da animacao de ataque:
public class AnimacaoAtaqueHandler : MonoBehaviour
{
    public void EventoDano()
    {
        // Executar OverlapCircle do ataque (mesmo codigo do PlayerAttack)
        PlayerAttack.Instance.ExecutarDanoMelee();
    }

    public void EventoSom()
    {
        AudioManager.Instance.TocarSFX(audioManager.sfxAtaqueEspada);
    }

    public void EventoVFX()
    {
        VFXManager.Instance.TocarVFX(vfxManager.vfxDanoInimigo, pontoImpacto);
    }
}
```

### Animações de inimigos
```csharp
// EnemyAI pode controlar o Animator:
public class EnemyAnimator : MonoBehaviour
{
    private Animator anim;
    private EnemyAI ai;
    private Rigidbody2D rb;

    void Start()
    {
        anim = GetComponent<Animator>();
        ai = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        anim.SetFloat("Velocidade", rb.velocity.magnitude);
        anim.SetFloat("Horizontal", rb.velocity.x);
        anim.SetFloat("Vertical", rb.velocity.y);
        anim.SetBool("Atacando", ai.estaAtacando);
        anim.SetBool("Morto", ai.estaMorto);
    }
}
```

---

## 5. Polimento de UI / HUD

### Dirty check no HUD (otimização)
```csharp
public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI textoCoracoes;
    public TextMeshProUGUI textoTentativas;
    public PlayerStats stats;

    private int ultimosCoracoes = -1;
    private int ultimasTentativas = -1;

    void Update()
    {
        if (stats.coracoesAtuais != ultimosCoracoes)
        {
            textoCoracoes.text = $"♥ {stats.coracoesAtuais}/{stats.coracoesMaximos}";
            ultimosCoracoes = stats.coracoesAtuais;
        }
        if (stats.tentativasAtuais != ultimasTentativas)
        {
            textoTentativas.text = $"Runs: {stats.tentativasAtuais}";
            ultimasTentativas = stats.tentativasAtuais;
        }
    }
}
```

### Transições de tela
```csharp
public class TransicaoTela : MonoBehaviour
{
    public Image painelPreto;
    public float duracao = 0.5f;

    public IEnumerator FadeIn()
    {
        float t = 0;
        Color cor = painelPreto.color;
        while (t < duracao)
        {
            cor.a = Mathf.Lerp(1, 0, t / duracao);
            painelPreto.color = cor;
            t += Time.deltaTime;
            yield return null;
        }
        cor.a = 0;
        painelPreto.color = cor;
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        Color cor = painelPreto.color;
        while (t < duracao)
        {
            cor.a = Mathf.Lerp(0, 1, t / duracao);
            painelPreto.color = cor;
            t += Time.deltaTime;
            yield return null;
        }
        cor.a = 1;
        painelPreto.color = cor;
    }
}
```

### Health bars nos inimigos
```csharp
public class HealthBarInimigo : MonoBehaviour
{
    public Transform barraVerde;
    public EnemyStats stats;
    private float larguraOriginal;

    void Start()
    {
        larguraOriginal = barraVerde.localScale.x;
    }

    void Update()
    {
        float percentual = stats.vidaAtual / stats.vidaMaxima;
        barraVerde.localScale = new Vector3(larguraOriginal * percentual, 1, 1);
    }
}
```

### Damage numbers flutuantes
```csharp
public class DamageNumber : MonoBehaviour
{
    public TextMeshPro texto;
    public float velocidade = 2f;
    public float duracao = 0.8f;

    public void Iniciar(float dano, Vector3 pos, Color cor)
    {
        transform.position = pos + Vector3.up * 0.5f;
        texto.text = dano.ToString("F0");
        texto.color = cor;
        Destroy(gameObject, duracao);
    }

    void Update()
    {
        transform.position += Vector3.up * velocidade * Time.deltaTime;
        texto.color = new Color(texto.color.r, texto.color.g, texto.color.b,
                                texto.color.a - Time.deltaTime / duracao);
    }
}
```

---

## 6. Juice (Hit Stop / Freeze Frame)

```csharp
public class HitStop : MonoBehaviour
{
    public static HitStop Instance;
    private Coroutine rotina;

    void Awake() { Instance = this; }

    public void Pausar(float duracao = 0.05f)
    {
        if (rotina != null) StopCoroutine(rotina);
        rotina = StartCoroutine(ExecutarPausa(duracao));
    }

    private IEnumerator ExecutarPausa(float duracao)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duracao);
        Time.timeScale = 1;
    }
}
```

### Uso: quando um ataque pesado acerta o inimigo
```csharp
// Em PlayerAttack ou EnemyStats:
HitStop.Instance.Pausar(0.05f);
CameraShake.Instance.Tocar(0.1f, 0.2f);
AudioManager.Instance.TocarSFX(audioManager.sfxImpactoPesado);
VFXManager.Instance.TocarVFX(vfxManager.vfxDanoInimigo, pontoImpacto);
```

---

## 7. Ordem Recomendada de Implementação

| Passo | O que | Impacto |
|---|---|---|
| 1 | `AudioManager` com SFX + Music Source | Jogo deixa de ser silencioso |
| 2 | Pool de AudioSource | Evita estouro de sons |
| 3 | `VFXManager` com pool | Feedback visual basico |
| 4 | `CameraShake` | Sensação de impacto |
| 5 | Animator Controller do player | Movimento visivel |
| 6 | Animation Events (som/dano sync) | Ataque com feedback |
| 7 | `HitStop` | Juice de impacto |
| 8 | Health bars + damage numbers | Clareza de combate |
| 9 | Transições de tela (fade in/out) | Profissionalismo |
| 10 | Animações de inimigos | Coesão visual |
| 11 | AudioMixer com sliders | Controle do jogador |

---

## 8. Pitfalls

- **Pool de AudioSource vs PlayOneShot**: `PlayOneShot` é suficiente para poucos sons simultâneos. Pool é necessário quando há 10+ sons ao mesmo tempo (ex: varios projeteis).
- **Animações e colliders**: Animation Events são mais confiáveis que `Update()` para sincronizar hitboxes com frames de ataque.
- **Time.timeScale e UI**: UI animada com `Time.timeScale = 0` congela. Use `Time.unscaledDeltaTime` para animacoes de UI durante hitstop.
- **Shader vs Particle**: Para efeitos simples (flash de dano), um script que altera `SpriteRenderer.color` é mais leve que instanciar um ParticleSystem.
- **Fade de musica**: Tocar `FadeMusica` no inicio de sala nova, nao no meio de combate.
